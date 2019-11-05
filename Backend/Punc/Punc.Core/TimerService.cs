using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Punc.Api.ViewModels;
using Punc.Data;

namespace Punc
{
    public class TimersService : ITimersService
    {
        private readonly IRefereeCodesService _refereeCodesService;
        private readonly IRouteService _routeService;
        private readonly StripeService _stripeService;
        private readonly ITimerEmailsService _timerEmailsService;
        private readonly ITimersRepository _repository;

        public TimersService(
            IRefereeCodesService refereeCodesService,
            ITimerEmailsService timerEmailsService,
            IRouteService routeService, 
            ITimersRepository repository,
            StripeService stripeService)
        {
            _refereeCodesService = refereeCodesService;
            _repository = repository;
            _routeService = routeService;
            _stripeService = stripeService;
            _timerEmailsService = timerEmailsService;
        }

        public async Task<bool> ConfirmTimerOnTimeStatusAsync(string refereeCode, bool onTime)
        {
            //get timer by code
            var timer = await GetTimerByRefereeCode(refereeCode);


            if (timer == null 
                || timer.Status == TimerStatus.Closed
                || !timer.ExpertMode)
            {
                return false;
            }

            // make sure timer status is valid to be modified
            if(timer.Status != TimerStatus.Active
               && timer.Status != TimerStatus.UnconfirmedLate
               && timer.Status != TimerStatus.AwaitingConfirmation
               && timer.Status != TimerStatus.Enroute
               && timer.Status != TimerStatus.TimeToLeave)
            {
                return false;
            }

            //if ontime, return true
            if (onTime)
            {
                //update
                timer.Status = TimerStatus.OnTime;
                await _repository.UpdateTimerAsync(timer);

                //release charge
                var res = await _stripeService.ReleasePaymentIntentAsync(timer.PaymentIntentId);
                return res;
            }
            else
            {
                //update
                timer.Status = TimerStatus.ConfirmedLate;
                await _repository.UpdateTimerAsync(timer);

                //charge
                var res = await _stripeService.CapturePaymentIntentAsync(timer.PaymentIntentId, timer.CustomerEmail);
                return res;
            }

        }

        public async Task<Timer> CreateTimerAsync(CreateTimerRequest req)
        {
            //create timer and set base properties
            var res = new Timer()
            {
                Id = Guid.NewGuid(),
                ExpertMode = req.ExpertMode,
                ArrivalTimeUtc = req.ArrivalTimeUtc,
                Destination = req.Destination,
                Origin = req.Origin,
                Status = TimerStatus.Active,
                TravelMode = req.TravelMode
            };

            //validate and set expert mode specific information and properties
            if (req.ExpertMode)
            {
                var expertRes = await GetAndSetExpertModeProperties(req, res);
                if (!expertRes)
                {
                    return res;
                }
            }

            //get route information and set route info properties
            var routeRes = await GetAndSetRouteInfoAsync(res, req);
            if (!routeRes)
            {
                return res;
            }

            //store timer in memory, expire after 2 days
            res = await _repository.CreateTimerAsync(res);
            
            //return timer
            return res;
        }

        public async Task<RefereeTimer> GetRefereeTimerAsync(string refereeCode)
        {
            var timer = await GetTimerByRefereeCode(refereeCode);

            return (RefereeTimer) timer;
        }

        public async Task<Timer> GetTimerAsync(Guid id)
        {
            var timer = await _repository.GetTimerAsync(id);
            if(timer!=null)
            {
                //check if timer is ready for an update
                if(timer.ExpertMode 
                    && timer.Status == TimerStatus.Active
                    && (DateTime.UtcNow - timer.LastUpdateUtc) > TimeSpan.FromMinutes(15))
                {
                    await GetAndSetRouteInfoAsync(timer);

                    //update the repo
                    await _repository.UpdateTimerAsync(timer);
                }

                return timer;
            }
            else
            {
                return null;
            }
        }



        /// <summary>
        /// Validate expert mode pro
        /// </summary>
        /// <param name="req"></param>
        /// <param name="timer"></param>
        /// <returns></returns>
        private async Task<bool> GetAndSetExpertModeProperties(CreateTimerRequest req, Timer timer)
        {
            var paymentValid = await _stripeService.ValidatePaymentIntentAsync(req.PaymentIntentId);
            if (!paymentValid)
            {
                timer.Status = TimerStatus.Failed;
                timer.Errors = TimerErrors.PaymentError;
                return false;
            }

            timer.ConfirmationMethod = req.ConfirmationMethod;
            timer.CustomerName = req.CustomerName;
            timer.CustomerEmail = new MailAddress(req.CustomerEmail);
            timer.PaymentIntentId = req.PaymentIntentId;

            //store referee email and create a code if confirmation method type is link confirmation
            if (req.ConfirmationMethod == TimerConfirmationMethod.LinkConfirmation)
            {
                timer.RefereeEmail = new MailAddress(req.RefereeEmail);

                //email referee
                var emailSuccess = await _timerEmailsService.SendRefereeEmailAsync(timer);
                if (!emailSuccess)
                {
                    timer.Status = TimerStatus.Failed;
                    timer.Errors = TimerErrors.RefereeEmailSendError;
                    return false;
                }
            }

            return true;
        }
       
        /// <summary>
        /// Gets the route information for a request and copies properties to the timer
        /// </summary>
        /// <param name="req"></param>
        /// <param name="timer"></param>
        /// <returns></returns>
        private async Task<bool> GetAndSetRouteInfoAsync(Timer timer)
        {
            return await GetAndSetRouteInfoAsync(timer, timer.Origin, timer.Destination, timer.TravelMode,
                timer.ArrivalTimeUtc);
        }

        /// <summary>
        /// Gets the route information for a request and copies properties to the timer
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        private async Task<bool> GetAndSetRouteInfoAsync(Timer timer, CreateTimerRequest req)
        {
            return await GetAndSetRouteInfoAsync(timer, req.Origin, req.Destination, req.TravelMode,
                req.ArrivalTimeUtc);
        }

        /// <summary>
        /// Gets the route information for a request and copies properties to the timer
        /// </summary>
        /// <param name="req"></param>
        /// <param name="timer"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="travelMode"></param>
        /// <param name="arrivalTimeUtc"></param>
        /// <returns></returns>
        private async Task<bool> GetAndSetRouteInfoAsync(Timer timer, string origin, string destination, TravelMode travelMode, DateTime arrivalTimeUtc)
        {
            //get route information
            var routeInfo =
                await _routeService.GetRouteInformationAsync(origin, destination, travelMode,
                    arrivalTimeUtc.ToUnixEpoch());

            //if cant get route, return fail!
            if (!routeInfo.Success)
            {
                timer.Status = TimerStatus.Failed;
                timer.Errors = TimerErrors.RouteError;
                return false;
            }

            //Set route information
            timer.DepartureTimeUtc = DateTimeExtensions.UnixEpochToDateTimeUtc(routeInfo.DepartureTime);
            timer.EstimatedArrivalTimeUtc = DateTimeExtensions.UnixEpochToDateTimeUtc(routeInfo.ArrivalTime);
            timer.EstimatedArrivalTimeUtc = DateTimeExtensions.UnixEpochToDateTimeUtc(routeInfo.ArrivalTime);
            timer.TravelDuration = routeInfo.TravelDuration;
            timer.TravelDistance = routeInfo.TravelDistance;

            //set time of last update to now
            timer.LastUpdateUtc = DateTime.UtcNow;

            //return success
            return true;
        }

        private async Task<Timer> GetTimerByRefereeCode(string refereeCode)
        {
            if (String.IsNullOrWhiteSpace(refereeCode))
                return null;

            var timerId = await _refereeCodesService.GetRefereeTimerIdAsync(refereeCode);
            if (timerId == null)
                return null;

            var timer = await GetTimerAsync((Guid)timerId);
            return timer;
        }
    }
}

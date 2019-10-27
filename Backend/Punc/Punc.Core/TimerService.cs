using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Punc.Api.ViewModels;

namespace Punc
{
    public class TimersService : ITimersService
    {
        private readonly IConfirmationService _confirmationService;
        private readonly IRouteService _routeService;
        private readonly StripeService _stripeService;
        IMemoryCache _cache;

        public TimersService(
            IConfirmationService confirmationService,
            IRouteService routeService, 
            StripeService stripeService,
            IMemoryCache cache)
        {
            _cache = cache;
            _confirmationService = confirmationService;
            _routeService = routeService;
            _stripeService = stripeService;
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
            _cache.Set(res.Id, res, TimeSpan.FromDays(2));

            //return timer
            return res;
        }

        public async Task<Timer> GetTimerAsync(Guid id)
        {
            if(_cache.TryGetValue(id, out Timer timer))
            {
                //check if timer is ready for an update
                if(timer.ExpertMode 
                    && timer.Status == TimerStatus.Active
                    && (DateTime.UtcNow - timer.LastUpdate) > TimeSpan.FromMinutes(15))
                {
                    await GetAndSetRouteInfoAsync(timer);
                }

                //update the cache
                _cache.Set(id, timer);

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
            var paymentValid = await _stripeService.ValidatePaymentIntent(req.PaymentIntentId);
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
                var emailSuccess = await _confirmationService.SendRefereeEmailAsync(timer);
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
            timer.LastUpdate = DateTime.UtcNow;

            //return success
            return true;
        }
    }
}

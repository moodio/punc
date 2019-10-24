using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Punc.Api.ViewModels;

namespace Punc
{
    public class TimersService : ITimersService
    {
        private readonly IRouteService _routeService;
        private readonly StripeService _stripeService;
        IMemoryCache _cache;

        public TimersService(IRouteService routeService, 
            StripeService stripeService,
            IMemoryCache cache)
        {
            _cache = cache;
            _routeService = routeService;
            _stripeService = stripeService;
        }

        public async Task<Timer> CreateTimerAsync(CreateTimerRequest req)
        {
            //confirm payment intent id is valid
            if (req.ExpertMode)
            {
                var paymentValid = await _stripeService.ValidatePaymentIntent(req.PaymentIntentId);
                if (!paymentValid)
                {
                    return new Timer()
                    {
                        Status = TimerStatus.Failed,
                        Errors = TimerErrors.PaymentError
                    };
                }
            }

            //get route information
            var routeInfo = await _routeService.GetRouteInformationAsync(req.Location, req.Destination, 
                req.TransportMode, req.ArrivalTimeUtc.ToUnixEpoch());

            //if cant get route, return fail!
            if (!routeInfo.Success)
            {
                return new Timer()
                {
                    Status = TimerStatus.Failed,
                    Errors = TimerErrors.RouteError
                };
            }

            //create new timer
            var res = new Timer()
            {
                Id = Guid.NewGuid(),
                ArrivalTimeEpoch = req.ArrivalTimeUtc.ToUnixEpoch(),
                ArrivalTimeUtc = req.ArrivalTimeUtc,
                EstimatedArrivalTimeEpoch = routeInfo.ArrivalTime,
                EstimatedArrivalTimeUtc = DateTimeExtensions.UnixEpochToDateTimeUtc(routeInfo.ArrivalTime),
                ConfirmationMethod = req.ConfirmationMethod,
                Destination = req.Destination,
                DepartureTimeEpoch = routeInfo.DepartureTime,
                ExpertMode = req.ExpertMode,
                LastUpdate = DateTime.UtcNow,
                Location = req.Location,
                PaymentIntentId = req.PaymentIntentId,
                Status = TimerStatus.Active,
                TransportMethod = req.TransportMode,
                TravelDuration = routeInfo.TravelDuration,
                TravelDistance = routeInfo.TravelDistance
            };

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
                    var routeInfo = await _routeService.GetRouteInformationAsync(timer.Location, timer.Destination,
                        timer.TransportMethod, timer.ArrivalTimeUtc.ToUnixEpoch());

                    timer.LastUpdate = DateTime.UtcNow;
                    timer.EstimatedArrivalTimeEpoch = routeInfo.ArrivalTime;
                    timer.EstimatedArrivalTimeUtc = DateTimeExtensions.UnixEpochToDateTimeUtc(routeInfo.ArrivalTime);
                    timer.DepartureTimeEpoch = routeInfo.DepartureTime;
                    timer.TravelDuration = routeInfo.TravelDuration;
                    timer.TravelDistance = routeInfo.TravelDistance;
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
    }
}

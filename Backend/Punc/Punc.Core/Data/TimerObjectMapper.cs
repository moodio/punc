using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Punc.Data
{
    public static class TimerObjectMapper
    {

        internal static Timer Map(TimerDataModel obj)
        {
            return new Timer()
            {
                Id = obj.Id,
                ArrivalTimeUtc = obj.ArrivalTimeUtc,
                ConfirmationMethod = obj.ConfirmationMethod,
                CustomerEmail = String.IsNullOrWhiteSpace(obj.CustomerEmail)
                    ? null
                    : new MailAddress(obj.CustomerEmail, obj.CustomerName),
                CustomerName = obj.CustomerName,
                DepartureTimeUtc = obj.DepartureTimeUtc,
                Destination = obj.Destination,
                Errors = obj.Errors,
                EstimatedArrivalTimeUtc = obj.EstimatedArrivalTimeUtc,
                ExpertMode = obj.ExpertMode,
                LastUpdateUtc = obj.LastUpdateUtc,
                Origin = obj.Origin,
                RefereeEmail = String.IsNullOrWhiteSpace(obj.RefereeEmail) ? null : new MailAddress(obj.RefereeEmail),
                TravelMode = obj.TravelMode,
                TravelDistance = obj.TravelDistance,
                TravelDuration = obj.TravelDuration,
                PaymentIntentId = obj.PaymentIntentId,
                Status = obj.Status
            };

        }

        internal static TimerDataModel Map(Timer obj)
        {
            return new TimerDataModel()
            {
                Id = obj.Id,
                ArrivalTimeUtc = obj.ArrivalTimeUtc,
                ConfirmationMethod = obj.ConfirmationMethod,
                CustomerEmail = obj.CustomerEmail?.Address,
                CustomerName = obj.CustomerName,
                DepartureTimeUtc = obj.DepartureTimeUtc,
                Destination = obj.Destination,
                Errors = obj.Errors,
                EstimatedArrivalTimeUtc = obj.EstimatedArrivalTimeUtc,
                ExpertMode = obj.ExpertMode,
                LastUpdateUtc = obj.LastUpdateUtc,
                Origin = obj.Origin,
                RefereeEmail = obj.RefereeEmail?.Address,
                TravelMode = obj.TravelMode,
                TravelDistance = obj.TravelDistance,
                TravelDuration = obj.TravelDuration,
                PaymentIntentId = obj.PaymentIntentId,
                Status = obj.Status
            };
        }
    }
}

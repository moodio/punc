using System;
using NUnit.Framework;

namespace Punc.Tests
{
    public class TimerStatusTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TimerStatusSetsToActiveWhenActive()
        {
            var timer = new Timer()
            {
                Status = TimerStatus.Active,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(40),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(20)).ToUnixEpoch()
            };

            Assert.AreEqual(TimerStatus.Active, timer.Status);
        }


        [Test]
        public void TimerStatusSetsToTimeToLeaveWhenWithinBuffer()
        {
            var timer = new Timer()
            {
                Status = TimerStatus.Active,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(40),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(4)).ToUnixEpoch()
            };

            Assert.AreEqual(TimerStatus.TimeToLeave, timer.Status);
        }

        [Test]
        public void TimerStatusSetsToEnrouteWhenShouldHaveLeft()
        {
            var timer = new Timer()
            {
                Status = TimerStatus.Active,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(20),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(-4)).ToUnixEpoch()
            };

            Assert.AreEqual(TimerStatus.Enroute, timer.Status);
        }

        [Test]
        public void TimerStatusSetsAwaitWhenPastArrivalTime()
        {
            var timer = new Timer()
            {
                Status = TimerStatus.Active,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(-20),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(-40)).ToUnixEpoch()
            };

            Assert.AreEqual(TimerStatus.AwaitingConfirmation, timer.Status);
        }

        [Test]
        public void TimerNotChangedWhenOtherStatus()
        {
            var timer1 = new Timer()
            {
                Status = TimerStatus.OnTime,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(-20),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(-40)).ToUnixEpoch()
            };

            var timer2 = new Timer()
            {
                Status = TimerStatus.Late,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(40),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(3)).ToUnixEpoch()
            };

            var timer3 = new Timer()
            {
                Status = TimerStatus.Cancelled,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(-20),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(-40)).ToUnixEpoch()
            };

            var timer4 = new Timer()
            {
                Status = TimerStatus.Failed,
                ArrivalTimeUtc = DateTime.UtcNow + TimeSpan.FromMinutes(40),
                DepartureTimeEpoch = (DateTime.UtcNow + TimeSpan.FromMinutes(10)).ToUnixEpoch()
            };

            Assert.AreEqual(TimerStatus.OnTime, timer1.Status);
            Assert.AreEqual(TimerStatus.Late, timer2.Status);
            Assert.AreEqual(TimerStatus.Cancelled, timer3.Status);
            Assert.AreEqual(TimerStatus.Failed, timer4.Status);

        }
    }
}
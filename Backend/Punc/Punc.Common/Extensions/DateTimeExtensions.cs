namespace System
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 01, 01, 0, 0, 0,DateTimeKind.Utc);
        public static int ToUnixEpoch(this DateTime dateTime)
        {
            return (int)(dateTime - UnixEpoch).TotalSeconds;
        }

        public static DateTime UnixEpochToDateTimeUtc(int timestamp)
        {
            var res = UnixEpoch + new TimeSpan(0, 0, timestamp);
            return res;
        }
    }
}

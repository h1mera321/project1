using System;

namespace bridgefluence.Providers
{
    public interface IDateTimeProvider
    {
        DateTime Now();
        DateTime UtcNow();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
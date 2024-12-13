namespace FeatureManagement.TimeBased
{
    public class MockTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public MockTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public DateTimeOffset Now => _utcNow;
        public DateTimeOffset UtcNow => _utcNow;
    }
}

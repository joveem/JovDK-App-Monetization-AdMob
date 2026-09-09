namespace JovDK.App.Monetization.AdMob
{
    public sealed class RewardedLoadRetry
    {
        static readonly int[] Delays = { 2, 5, 15 };
        int _attempt;
        public int NextDelay() => _attempt < Delays.Length ? Delays[_attempt++] : -1;
        public void Reset() => _attempt = 0;
    }
    public enum RewardedAdState { Initializing, Loading, RetryWaiting, Available, Showing, Unavailable, Disposed }
}

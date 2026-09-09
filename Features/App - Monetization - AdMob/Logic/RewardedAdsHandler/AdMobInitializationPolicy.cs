using GoogleMobileAds.Api;
namespace JovDK.App.Monetization.AdMob
{
    public static class AdMobInitializationPolicy
    {
        // Readiness affects availability of individual adapters, not permission to request an ad.
        public static bool AllowsLoading(InitializationStatus status) => status != null;
    }
}

using UnityEngine;
using GoogleMobileAds.Api;

namespace JovDK.App.Monetization.AdMob
{
    public partial class RewardedAdsHandler
    {
        void OnInitializationCompleted(InitializationStatus status)
        {
            if (_destroyed || IsInitialized) return;
            // Adapter readiness is diagnostic only. A valid completion permits a load attempt.
            if (!_session.TryInitialize(AdMobInitializationPolicy.AllowsLoading(status)))
            { SetState(RewardedAdState.Unavailable); Debug.LogWarning("AdMob initialization returned no status."); return; }
            OnInitializationFinishCallback?.Invoke();
            LoadRewardedAd();
        }
        void OnRewardedAdLoaded(int generation, RewardedAd ad, LoadAdError error)
        {
            bool success = error == null && ad != null;
            if (!_session.TryCompleteLoad(generation, success))
            {
                if (ad != null && !ReferenceEquals(ad, _currentRewardedAd)) ad.Destroy();
                return;
            }
            if (!success)
            {
                if (ad != null) ad.Destroy();
                Debug.LogWarning("AdMob rewarded ad unavailable: " + (error == null ? "empty result" : error.GetMessage()));
                ScheduleRetry(); return;
            }
            _retry.Reset();
            _currentRewardedAd = ad;
            ad.OnAdFullScreenContentClosed += () => Dispatch(() => FinishPresentation(generation));
            ad.OnAdFullScreenContentFailed += errorInfo => Dispatch(() => FinishPresentation(generation));
            SetState(RewardedAdState.Available);
            OnAdAvailabilityUpdate(true);
        }
        void FinishPresentation(int generation)
        {
            if (!_session.TryFinishShow(generation)) return;
            PresentationFinished?.Invoke();
            if (_modePending) { _modePending = false; _session.InvalidatePending(); }
            LoadRewardedAd();
        }
        void OnAdAvailabilityUpdate(bool available)
        {
            if (_destroyed || _hasAvailableAd == available) return;
            _hasAvailableAd = available;
            OnAdAvailabilityUpdateCallback?.Invoke("n/a", available);
        }
    }
}

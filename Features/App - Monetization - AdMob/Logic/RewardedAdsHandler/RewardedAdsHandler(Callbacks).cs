using System.Linq;
using UnityEngine;
using GoogleMobileAds.Api;

namespace JovDK.App.Monetization.AdMob
{
    public partial class RewardedAdsHandler
    {
        void OnInitializationCompleted(InitializationStatus status)
        {
            if (_destroyed || IsInitialized) return;
            var adapters = status == null ? null : status.getAdapterStatusMap();
            if (adapters == null || !adapters.Values.Any(value => value != null && value.InitializationState == AdapterState.Ready))
            {
                Debug.LogWarning("AdMob initialization completed without a ready adapter.");
                return;
            }
            if (!_session.TryInitialize(true)) return;
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
                OnAdAvailabilityUpdate(false);
                Debug.LogWarning("AdMob rewarded ad load failed: " + (error == null ? "empty result" : error.ToString()));
                return;
            }
            _currentRewardedAd = ad;
            ad.OnAdFullScreenContentClosed += () => Dispatch(() => FinishPresentation(generation));
            ad.OnAdFullScreenContentFailed += errorInfo => Dispatch(() => FinishPresentation(generation));
            OnAdAvailabilityUpdate(true);
        }

        void FinishPresentation(int generation)
        {
            if (!_session.TryFinishShow(generation)) return;
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

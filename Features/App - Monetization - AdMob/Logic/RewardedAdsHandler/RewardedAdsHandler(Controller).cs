using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace JovDK.App.Monetization.AdMob
{
    public partial class RewardedAdsHandler
    {
        string GetRewardedAdId()
        {
#if UNITY_ANDROID
            return Debug.isDebugBuild ? "ca-app-pub-3940256099942544/5224354917" : _androidAdUnitId;
#elif UNITY_IPHONE
            return Debug.isDebugBuild ? "ca-app-pub-3940256099942544/1712485313" : _iOSAdUnitId;
#else
            return "unused";
#endif
        }

        public void LoadRewardedAd()
        {
            if (!IsInitialized || !_session.TryBeginLoad(out var generation)) return;
            var old = _currentRewardedAd;
            _currentRewardedAd = null;
            if (old != null) old.Destroy();
            OnAdAvailabilityUpdate(false);
            try
            {
                RewardedAd.Load(GetRewardedAdId(), new AdRequest(),
                    (ad, error) => Dispatch(() => OnRewardedAdLoaded(generation, ad, error)));
            }
            catch (Exception error)
            {
                _session.TryCompleteLoad(generation, false);
                Debug.LogWarning("AdMob rewarded ad request failed: " + error.Message);
            }
        }

        public void ShowRewardedAd()
        {
            var ad = _currentRewardedAd;
            if (ad == null || _destroyed) return;
            if (!ad.CanShowAd()) { OnAdAvailabilityUpdate(false); LoadRewardedAd(); return; }
            if (!_session.TryBeginShow(out var generation)) return;
            OnAdAvailabilityUpdate(false);
            try
            {
                ad.Show(reward => Dispatch(() =>
                {
                    if (reward != null && _session.TryReward(generation)) OnVideoRewardCloseCallback?.Invoke(reward);
                }));
            }
            catch (Exception error)
            {
                Debug.LogWarning("AdMob rewarded ad presentation failed: " + error.Message);
                FinishPresentation(generation);
            }
        }
    }
}

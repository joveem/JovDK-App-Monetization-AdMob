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
            return _testMode ? "ca-app-pub-3940256099942544/5224354917" : _androidAdUnitId;
#elif UNITY_IPHONE
            return _testMode ? "ca-app-pub-3940256099942544/1712485313" : _iOSAdUnitId;
#else
            return "unused";
#endif
        }
        public void LoadRewardedAd()
        {
            if (!_session.TryBeginLoad(out var generation)) return;
            DestroyCurrentAd();
            _loadingGeneration = generation;
            _deadline = Time.realtimeSinceStartupAsDouble + 30;
            SetState(RewardedAdState.Loading);
            OnAdAvailabilityUpdate(false);
            try
            {
                RewardedAd.Load(GetRewardedAdId(), new AdRequest(),
                    (ad, error) => Dispatch(() => OnRewardedAdLoaded(generation, ad, error)));
            }
            catch (Exception error)
            {
                if (_session.TryCompleteLoad(generation, false)) ScheduleRetry();
                Debug.LogWarning("AdMob rewarded request failed: " + error.Message);
            }
        }
        public void ShowRewardedAd() => TryShowRewardedAd();
        public bool TryShowRewardedAd()
        {
            var ad = _currentRewardedAd;
            if (ad == null || _destroyed) return false;
            if (!ad.CanShowAd()) { OnAdAvailabilityUpdate(false); LoadRewardedAd(); return false; }
            if (!_session.TryBeginShow(out var generation)) return false;
            SetState(RewardedAdState.Showing);
            OnAdAvailabilityUpdate(false);
            try
            {
                ad.Show(reward => Dispatch(() =>
                {
                    if (reward != null && _session.TryReward(generation)) OnVideoRewardCloseCallback?.Invoke(reward);
                }));
                return true;
            }
            catch (Exception error)
            {
                Debug.LogWarning("AdMob rewarded presentation failed: " + error.Message);
                FinishPresentation(generation); return false;
            }
        }
    }
}

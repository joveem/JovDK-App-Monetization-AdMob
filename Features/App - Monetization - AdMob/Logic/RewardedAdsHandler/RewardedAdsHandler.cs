using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace JovDK.App.Monetization.AdMob
{
    // Legacy serialized SDK host; the session policy is plain Logic, and the existing
    // scene Assembler retains ownership of Display subscriptions and routing.
    public partial class RewardedAdsHandler : MonoBehaviour
    {
        bool _hasAvailableAd;
        bool _destroyed;
        readonly RewardedAdSession _session = new RewardedAdSession();
        RewardedAd _currentRewardedAd;
        public bool IsInitialized { get { return _session.IsInitialized; } }
        public bool HasAvailableAd { get { return _hasAvailableAd; } }
        public Action OnInitializationFinishCallback;
        public Action<string, bool> OnAdAvailabilityUpdateCallback;
        public Action<Reward> OnVideoRewardCloseCallback;
        [SerializeField] string _androidAdUnitId = "UNDEFINED";
        [SerializeField] string _iOSAdUnitId = "UNDEFINED";

        void Start()
        {
            MobileAds.Initialize(status => Dispatch(() => OnInitializationCompleted(status)));
        }

        static void Dispatch(Action callback)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(callback);
        }

        void OnDestroy()
        {
            _destroyed = true;
            _session.Dispose();
            _hasAvailableAd = false;
            var ad = _currentRewardedAd;
            _currentRewardedAd = null;
            if (ad != null) ad.Destroy();
        }
    }
}

using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace JovDK.App.Monetization.AdMob
{
    // Serialized SDK adapter. Rules are isolated in RewardedAdSession/RewardedLoadRetry.
    public partial class RewardedAdsHandler : MonoBehaviour
    {
        static bool _initializationRequested;
        static bool _initializationCompleted;
        static InitializationStatus _initializationStatus;
        static event Action<InitializationStatus> InitializationCompleted;
        bool _hasAvailableAd;
        bool _destroyed;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool _testMode = true;
#else
        bool _testMode;
#endif
        bool _modePending;
        double _deadline;
        int _loadingGeneration;
        readonly RewardedAdSession _session = new RewardedAdSession();
        readonly RewardedLoadRetry _retry = new RewardedLoadRetry();
        RewardedAd _currentRewardedAd;
        public bool IsInitialized => _session.IsInitialized;
        public bool HasAvailableAd => _hasAvailableAd;
        public RewardedAdState State { get; private set; } = RewardedAdState.Initializing;
        public Action OnInitializationFinishCallback;
        public Action<string, bool> OnAdAvailabilityUpdateCallback;
        public Action<Reward> OnVideoRewardCloseCallback;
        public event Action StateChanged;
        public event Action PresentationFinished;
        [SerializeField] string _androidAdUnitId = "UNDEFINED";
        [SerializeField] string _iOSAdUnitId = "UNDEFINED";

        bool _requestsAllowed = true;
        bool _started;
        public event Action<int> LoadFailed;
        public void SetRequestsAllowed(bool allowed)
        {
            if (_destroyed || _requestsAllowed == allowed) return;
            _requestsAllowed = allowed;
            _session.SetRequestsAllowed(allowed);
            if (!allowed)
            {
                if (!_session.IsShowing) { _session.InvalidatePending(); DestroyCurrentAd(); }
                OnAdAvailabilityUpdate(false);
                SetState(RewardedAdState.Unavailable);
            }
            else if (_started)
            {
                if (IsInitialized) { _retry.Reset(); LoadRewardedAd(); }
                else BeginInitialization();
            }
        }
        void Start() { _started = true; if (_requestsAllowed) BeginInitialization(); }
        void BeginInitialization()
        {
            _deadline = Time.realtimeSinceStartupAsDouble + 40;
            if (_initializationCompleted) { OnInitializationCompleted(_initializationStatus); return; }
            InitializationCompleted -= OnInitializationCompleted;
            InitializationCompleted += OnInitializationCompleted;
            if (_initializationRequested) return;
            _initializationRequested = true;
            try { MobileAds.Initialize(status => Dispatch(() => CompleteInitialization(status))); }
            catch (Exception) { CompleteInitialization(null); }
        }
        static void CompleteInitialization(InitializationStatus status)
        {
            if (_initializationCompleted) return;
            _initializationCompleted = true;
            _initializationStatus = status;
            InitializationCompleted?.Invoke(status);
            InitializationCompleted = null;
        }
        void Update()
        {
            if (_destroyed || !_requestsAllowed || Time.realtimeSinceStartupAsDouble < _deadline) return;
            if (State == RewardedAdState.RetryWaiting) LoadRewardedAd();
            else if (State == RewardedAdState.Loading && _session.TryCompleteLoad(_loadingGeneration, false)) ScheduleRetry();
            else if (State == RewardedAdState.Initializing) SetState(RewardedAdState.Unavailable);
        }
        static void Dispatch(Action callback) => MobileAdsEventExecutor.ExecuteInUpdate(callback);
        void SetState(RewardedAdState state) { if (State == state) return; State = state; StateChanged?.Invoke(); }
        public void SetTestAdsEnabled(bool enabled)
        {
            // Development remains a safe superset even for consumers without a Level 3 policy.
            enabled = enabled || Debug.isDebugBuild;
            if (_testMode == enabled) return;
            _testMode = enabled;
            if (_session.IsShowing) { _modePending = true; return; }
            _session.InvalidatePending();
            DestroyCurrentAd();
            OnAdAvailabilityUpdate(false);
            _retry.Reset();
            if (IsInitialized) LoadRewardedAd();
        }
        public void RetryLoading()
        {
            if (State != RewardedAdState.Unavailable || !IsInitialized) return;
            _retry.Reset(); LoadRewardedAd();
        }
        void ScheduleRetry()
        {
            OnAdAvailabilityUpdate(false);
            var delay = _retry.NextDelay();
            _deadline = Time.realtimeSinceStartupAsDouble + System.Math.Max(0, delay);
            SetState(delay < 0 ? RewardedAdState.Unavailable : RewardedAdState.RetryWaiting);
        }
        void DestroyCurrentAd()
        {
            var old = _currentRewardedAd; _currentRewardedAd = null;
            if (old != null) old.Destroy();
        }
        void OnDestroy()
        {
            _destroyed = true;
            InitializationCompleted -= OnInitializationCompleted;
            _session.Dispose(); _hasAvailableAd = false;
            DestroyCurrentAd(); State = RewardedAdState.Disposed;
        }
    }
}

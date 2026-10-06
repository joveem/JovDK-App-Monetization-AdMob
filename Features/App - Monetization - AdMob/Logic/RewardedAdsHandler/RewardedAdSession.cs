namespace JovDK.App.Monetization.AdMob
{
    /// <summary>Owns one load/presentation lifecycle; SDK callbacks must be serialized by its adapter.</summary>
    public sealed class RewardedAdSession
    {
        enum Phase { Idle, Loading, Ready, Showing, Disposed }
        Phase _phase;
        int _generation;
        bool _rewarded;
        bool _requestsAllowed = true;
        public bool IsInitialized { get; private set; }

        public void SetRequestsAllowed(bool allowed)
        {
            if (_requestsAllowed == allowed) return;
            _requestsAllowed = allowed;
            if (!allowed) InvalidatePending();
        }

        public bool TryInitialize(bool initializationCompleted)
        {
            if (!initializationCompleted || IsInitialized || _phase == Phase.Disposed) return false;
            IsInitialized = true;
            return true;
        }

        public bool TryBeginLoad(out int generation)
        {
            generation = _generation;
            if (!_requestsAllowed || !IsInitialized || _phase == Phase.Loading || _phase == Phase.Showing || _phase == Phase.Disposed) return false;
            generation = ++_generation;
            _phase = Phase.Loading;
            _rewarded = false;
            return true;
        }

        public bool TryCompleteLoad(int generation, bool success)
        {
            if (_phase != Phase.Loading || generation != _generation) return false;
            _phase = success ? Phase.Ready : Phase.Idle;
            return true;
        }

        public bool TryBeginShow(out int generation)
        {
            generation = _generation;
            if (!_requestsAllowed || _phase != Phase.Ready) return false;
            _phase = Phase.Showing;
            return true;
        }

        public bool TryReward(int generation)
        {
            if (_phase != Phase.Showing || generation != _generation || _rewarded) return false;
            _rewarded = true;
            return true;
        }

        public bool TryFinishShow(int generation)
        {
            if (_phase != Phase.Showing || generation != _generation) return false;
            _phase = Phase.Idle;
            return true;
        }

        public bool IsShowing => _phase == Phase.Showing;
        public bool InvalidatePending()
        {
            if (_phase == Phase.Showing || _phase == Phase.Disposed) return false;
            ++_generation; _phase = Phase.Idle; _rewarded = false; return true;
        }

        public void Dispose() { _phase = Phase.Disposed; IsInitialized = false; }
    }
}

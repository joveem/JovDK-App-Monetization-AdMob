using NUnit.Framework;
using JovDK.App.Monetization.AdMob;

public sealed class RewardedAdSessionTests
{
    [Test]
    public void ConsentRevocationInvalidatesPendingAndBlocksNewLoadsUntilAllowed()
    {
        var session = InitializedSession();
        session.TryBeginLoad(out var old);
        session.SetRequestsAllowed(false);
        Assert.That(session.TryCompleteLoad(old, true), Is.False);
        Assert.That(session.TryBeginLoad(out _), Is.False);
        Assert.That(session.TryBeginShow(out _), Is.False);
        session.SetRequestsAllowed(true);
        Assert.That(session.TryBeginLoad(out var current), Is.True);
        Assert.That(session.TryCompleteLoad(current, true), Is.True);
        Assert.That(session.TryBeginShow(out _), Is.True);
    }
    [Test]
    public void RevocationDuringPresentationAllowsOneRewardButNoReload()
    {
        var session = InitializedSession();
        session.TryBeginLoad(out var id); session.TryCompleteLoad(id, true); session.TryBeginShow(out _);
        session.SetRequestsAllowed(false);
        Assert.That(session.TryReward(id), Is.True);
        Assert.That(session.TryReward(id), Is.False);
        Assert.That(session.TryFinishShow(id), Is.True);
        Assert.That(session.TryBeginLoad(out _), Is.False);
    }
    [Test]
    public void DevelopmentEditor_UsesOfficialRewardedTestUnit()
    {
        var owner = new UnityEngine.GameObject("Rewarded ad configuration test");
        try
        {
            var handler = owner.AddComponent<RewardedAdsHandler>();
            var method = typeof(RewardedAdsHandler).GetMethod("GetRewardedAdId", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
#if UNITY_ANDROID
            Assert.That(method.Invoke(handler, null), Is.EqualTo("ca-app-pub-3940256099942544/5224354917"));
#elif UNITY_IPHONE
            Assert.That(method.Invoke(handler, null), Is.EqualTo("ca-app-pub-3940256099942544/1712485313"));
#else
            Assert.That(method.Invoke(handler, null), Is.EqualTo("unused"));
#endif
        }
        finally { UnityEngine.Object.DestroyImmediate(owner); }
    }

    static RewardedAdSession InitializedSession()
    {
        var session = new RewardedAdSession();
        Assert.That(session.TryInitialize(true), Is.True);
        return session;
    }

    [Test]
    public void Initialization_RejectsInvalidCompletionAndDuplicateCompletion()
    {
        var session = new RewardedAdSession();
        Assert.That(session.TryBeginLoad(out _), Is.False);
        Assert.That(session.TryInitialize(false), Is.False);
        Assert.That(session.IsInitialized, Is.False);
        Assert.That(session.TryInitialize(true), Is.True);
        Assert.That(session.TryInitialize(true), Is.False);
        Assert.That(session.TryBeginLoad(out _), Is.True);
        session.Dispose();
        Assert.That(session.IsInitialized, Is.False);
        Assert.That(session.TryInitialize(true), Is.False);
    }

    [Test]
    public void V1_Reward_IsAcceptedOnce_OnlyDuringSuccessfulPresentation()
    {
        var session = InitializedSession();
        Assert.That(session.TryReward(0), Is.False);
        Assert.That(session.TryBeginLoad(out var id), Is.True);
        Assert.That(session.TryBeginShow(out _), Is.False);
        Assert.That(session.TryCompleteLoad(id, true), Is.True);
        Assert.That(session.TryReward(id), Is.False);
        Assert.That(session.TryBeginShow(out var shown), Is.True);
        Assert.That(shown, Is.EqualTo(id));
        Assert.That(session.TryReward(id), Is.True);
        Assert.That(session.TryReward(id), Is.False);
        Assert.That(session.TryFinishShow(id), Is.True);
        Assert.That(session.TryReward(id), Is.False);
    }

    [Test]
    public void RepeatedLoadAndShow_DoNotCreateConcurrentOperations()
    {
        var session = InitializedSession();
        session.TryBeginLoad(out var id);
        Assert.That(session.TryBeginLoad(out _), Is.False);
        session.TryCompleteLoad(id, true);
        Assert.That(session.TryBeginShow(out _), Is.True);
        Assert.That(session.TryBeginShow(out _), Is.False);
        Assert.That(session.TryBeginLoad(out _), Is.False);
        Assert.That(session.TryFinishShow(id), Is.True);
        Assert.That(session.TryFinishShow(id), Is.False);
        Assert.That(session.TryBeginLoad(out var next), Is.True);
        Assert.That(next, Is.Not.EqualTo(id));
    }

    [Test]
    public void FailedLoad_AllowsRetry_RejectingDuplicateAndStaleResults()
    {
        var session = InitializedSession();
        session.TryBeginLoad(out var old);
        Assert.That(session.TryCompleteLoad(old, false), Is.True);
        Assert.That(session.TryBeginShow(out _), Is.False);
        session.TryBeginLoad(out var current);
        Assert.That(session.TryCompleteLoad(old, true), Is.False);
        Assert.That(session.TryCompleteLoad(current, true), Is.True);
        Assert.That(session.TryCompleteLoad(current, false), Is.False);
        session.TryBeginShow(out _);
        Assert.That(session.TryReward(old), Is.False);
        Assert.That(session.TryFinishShow(old), Is.False);
        Assert.That(session.TryReward(current), Is.True);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void Dispose_RejectsAllLaterCallbacksAndRequests(bool presenting)
    {
        var session = InitializedSession();
        session.TryBeginLoad(out var id);
        if (presenting) { session.TryCompleteLoad(id, true); session.TryBeginShow(out _); }
        session.Dispose(); session.Dispose();
        Assert.That(session.TryBeginLoad(out _), Is.False);
        Assert.That(session.TryCompleteLoad(id, true), Is.False);
        Assert.That(session.TryBeginShow(out _), Is.False);
        Assert.That(session.TryReward(id), Is.False);
        Assert.That(session.TryFinishShow(id), Is.False);
    }
}

// system / unity
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// third
using TMPro;
using GoogleMobileAds;
using GoogleMobileAds.Api;

// from company
using JovDK.Debugging;
using JovDK.SafeActions;
using JovDK.SerializingTools.Json;

// from project
// ...


namespace JovDK.App.Monetization.AdMob
{
    public partial class RewardedAdsHandler : MonoBehaviour
    {
        string GetRewardedAdId()
        {
            string value = "";

            // Android testing ad id = "ca-app-pub-3940256099942544/5224354917"
            // iOS testing ad id = "ca-app-pub-3940256099942544/1712485313"

#if UNITY_ANDROID
            value = "ca-app-pub-8716863197536867/7258711711"; // TODO: REVIEW THIS!
#elif UNITY_IPHONE
            value = "ca-app-pub-3940256099942544/1712485313"; // TODO: REVIEW THIS!
#else
            value = "unused";
#endif

            return value;
        }


        /// <summary>
        /// Loads the rewarded ad.
        /// </summary>
        public void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            if (_currentRewardedAd != null)
            {
                _currentRewardedAd.Destroy();
                _currentRewardedAd = null;
            }

            // Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            AdRequest adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(GetRewardedAdId(), adRequest, OnRewardedAdLoaded);
        }

        public void ShowRewardedAd()
        {
            if (_currentRewardedAd != null && _currentRewardedAd.CanShowAd())
            {
                OnAdAvailabilityUpdate("n/a", false);
                _currentRewardedAd.Show(OnVideoRewardClose);
            }
        }
        void RegisterEventHandlers(RewardedAd rewardedAd)
        {
            // Raised when the ad is estimated to have earned money.
            rewardedAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            rewardedAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            rewardedAd.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            rewardedAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError(
                    "Rewarded ad failed to open full screen content " +
                    "with error : " + error);
            };
        }

        void RegisterReloadHandler(RewardedAd ad)
        {
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded Ad full screen content closed.");

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError(
                    "Rewarded ad failed to open full screen content " +
                    "with error : " + error);

                // Reload the ad so that we can show another as soon as possible.
                LoadRewardedAd();
            };
        }
    }
}

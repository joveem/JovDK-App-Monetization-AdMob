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
using System.Linq;

// from project
// ...


namespace JovDK.App.Monetization.AdMob
{
    public partial class RewardedAdsHandler : MonoBehaviour
    {
        void OnInitializationCompleted(InitializationStatus initializationStatus)
        {
            DebugExtension.NDLog("initializationStatus = " + initializationStatus.SerializeObjectToJSON());
            DebugExtension.NDLog("adapterStatusMap = " + initializationStatus.getAdapterStatusMap().SerializeObjectToJSON(true));

            AdapterStatus adapterStatus = initializationStatus.getAdapterStatusMap().Values.ToList()[0];

            if (adapterStatus.InitializationState == AdapterState.Ready)
            {
                _isInitialized = true;
                OnInitializationFinish();
            }
        }

        void OnRewardedAdLoaded(RewardedAd rewardedAd, LoadAdError error)
        {
            // if error is not null, the load request failed
            if (error == null && rewardedAd != null)
            {
                string debugText =
                    "#> ".ToColor(GoodColors.Pink) +
                    "OnRewardedAdLoaded" + "\n" +
                    "Rewarded ad loaded with response : " +
                    rewardedAd.GetResponseInfo().GetResponseId().SerializeObjectToJSON(true) + "\n" +
                    "Rewarded ad loaded with response : " +
                    rewardedAd.GetResponseInfo().GetLoadedAdapterResponseInfo().SerializeObjectToJSON(true) + "\n" +
                    "Rewarded ad loaded with response : " +
                    rewardedAd.GetResponseInfo().GetMediationAdapterClassName().SerializeObjectToJSON(true) + "\n" +
                    "Rewarded ad loaded with response : " +
                    rewardedAd.GetResponseInfo().GetType().SerializeObjectToJSON(true) + "\n" +
                    "Rewarded ad loaded with response : " +
                    rewardedAd.GetResponseInfo().GetResponseExtras().SerializeObjectToJSON(true) + "\n" +
                    "\n" +
                    "rewardedAd = " + "\n" +
                    rewardedAd.SerializeObjectToJSON(true);
                DebugExtension.NDLog(debugText);

                _currentRewardedAd = rewardedAd;
                OnAdAvailabilityUpdate("n/a", true);
            }
            else
            {
                string debugText =
                    "$ > ".ToColor(GoodColors.Red) +
                    "ERROR trying to load RewardedAd!";

                if (error != null)
                    debugText += "\n" + "error = " + error;

                if (rewardedAd == null)
                    debugText += "\n" + "rewardedAd IS NULL!";

                DebugExtension.NDLogWarning(debugText);
            }
        }

        void OnInitializationFinish()
        {
            OnInitializationFinishCallback?.Invoke();
            LoadRewardedAd();
        }

        void OnVideoRewardClose(Reward reward)
        {
            DebugExtension.NDLog(
                "#> ".ToColor(GoodColors.Pink) +
                "OnVideoRewardClose" + "\n" +
                "reward.Type = " + reward.Type + "\n" +
                "reward.Amount = " + reward.Amount + "\n" +
                "reward = " + "\n" +
                reward.SerializeObjectToJSON(true) + "\n" +
                "");

            OnVideoRewardCloseCallback(reward);
        }

        void OnAdAvailabilityUpdate(string adName, bool isAvailable)
        {
            _hasAvailableAd = isAvailable;
            OnAdAvailabilityUpdateCallback?.Invoke(adName, isAvailable);
        }
    }
}

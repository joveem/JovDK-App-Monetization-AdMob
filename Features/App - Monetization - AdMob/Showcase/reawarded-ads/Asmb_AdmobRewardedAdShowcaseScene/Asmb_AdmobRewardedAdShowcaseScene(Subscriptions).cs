// system / unity
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// third
using TMPro;

// from company
using JovDK.Debugging;
using JovDK.SafeActions;
using JovDK.SerializingTools.Json;

// from project
// ...


namespace JovDK.App.Monetization.AdMob.Testing.Showcase
{
    public partial class Asmb_AdmobRewardedAdShowcaseScene : MonoBehaviour
    {
        void SubscribeAllListeners()
        {
            _rewardedAdsHandler.DoIfNotNull(() => _rewardedAdsHandler.OnAdAvailabilityUpdateCallback += OnAdAvailabilityUpdate);
        }

        void OnAdAvailabilityUpdate(string adName, bool isAvailable)
        {
            DebugExtension.NDLog(
                "#> " +
                "OnAdAvailabilityUpdate | " +
                "adName = \"" + adName + "\" | " +
                "isAvailable = " + isAvailable);

            _hasAvailableAd = isAvailable;
            RefreshButtonState();
        }

        void RefreshButtonState()
        {
            _whatchAdButton.interactable = _hasAvailableAd;
        }
    }
}

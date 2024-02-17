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
        void SetupAllButtons()
        {
            _whatchAdButton.SetOnClickIfNotNull(WhatchAdButton);
        }

        void WhatchAdButton()
        {
            DebugExtension.NDLog("> ".ToColor(GoodColors.Orange) + "WhatchAdButton");

            _rewardedAdsHandler.ShowRewardedAd();
        }
    }
}

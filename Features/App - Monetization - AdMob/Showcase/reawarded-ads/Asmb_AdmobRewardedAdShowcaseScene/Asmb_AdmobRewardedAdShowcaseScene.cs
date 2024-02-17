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


namespace PackageName.MajorContext.MinorContext
{
    public partial class Asmb_AdmobRewardedAdShowcaseScene : MonoBehaviour
    {

        [Space(5), Header("[ Dependencies ]"), Space(10)]

        [SerializeField] RewardedAdsHandler _rewardedAdsHandler;


        [Space(5), Header("[ State ]"), Space(10)]

        bool _hasAvailableAd = false;


        [Space(5), Header("[ Parts ]"), Space(10)]

        [SerializeField] Button _whatchAdButton;


        // [Space(5), Header("[ Configs ]"), Space(10)]

        // bool _configs;


        void Awake()
        {
            SetupAllButtons();
            SubscribeAllListeners();

            UpdateAdData();
            RefreshButtonState();
        }

        void UpdateAdData()
        {
            _hasAvailableAd = _rewardedAdsHandler.HasAvailableAd;
        }

        // void Start()
        // {

        // }

        // void Update()
        // {

        // }

        // void FixedUpdate()
        // {

        // }
    }
}

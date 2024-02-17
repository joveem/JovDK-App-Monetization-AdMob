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


namespace PackageName.MajorContext.MinorContext
{
    public partial class RewardedAdsHandler : MonoBehaviour
    {

        // [Space(5), Header("[ Dependencies ]"), Space(10)]

        // bool _dependencies;


        [Space(5), Header("[ State ]"), Space(10)]

        bool _isInitialized = false;
        public bool IsInitialized { get => _isInitialized; }
        bool _hasAvailableAd = false;
        public bool HasAvailableAd { get => _hasAvailableAd; }

        // InitializationStatus _currentInitializationStatus = null;
        RewardedAd _currentRewardedAd;

        public Action OnInitializationFinishCallback = null;
        public Action<string, bool> OnAdAvailabilityUpdateCallback = null;



        // [Space(5), Header("[ Parts ]"), Space(10)]

        // bool _parts;


        // [Space(5), Header("[ Configs ]"), Space(10)]

        // bool _configs;


        // void Awake()
        // {

        // }

        void Start()
        {
            MobileAds.Initialize(OnInitializationCompleted);
        }

        // void Update()
        // {

        // }

        // void FixedUpdate()
        // {

        // }
    }
}

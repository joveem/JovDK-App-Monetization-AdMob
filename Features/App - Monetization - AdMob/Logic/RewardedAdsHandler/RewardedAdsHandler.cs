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
        public Action<Reward> OnVideoRewardCloseCallback = null;



        // [Space(5), Header("[ Parts ]"), Space(10)]

        // bool _parts;


        [Space(5), Header("[ Configs ]"), Space(10)]

        // Android testing ad id = "ca-app-pub-3940256099942544/5224354917"
        // iOS testing ad id = "ca-app-pub-3940256099942544/1712485313"
        [SerializeField] string _androidAdUnitId = "UNDEFINED";
        [SerializeField] string _iOSAdUnitId = "UNDEFINED";
        string _adUnitId = null; // This will remain null for unsupported platforms


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

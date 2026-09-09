#  JovDK.App.Monetization.AdMob
---

## About the project

This repo is a submodule of [JovDK](https://github.com/joveem/JovDK)

JovDK is Unity **D**evelopment **K**it that groups tools and implementations that are not single-project-specifics. All Code on this kit need to have the intent of being used on any project with litle or none dependencies

JovDK.App.Monetization.AdMob is module that contains all AdMob related stuffs


---

### Getting Started:

To use this development kit, first check the [Dependencies](#dependencies) section. After that, clone this repo as a git submodule of JovDK submodule

##### Dependencies:

Make sure to have all that before installation

[:link: Google Mobile Ads Unity Plugin](https://github.com/googleads/googleads-mobile-unity/releases)

[:link: JovDK-Core](https://github.com/googleads/googleads-mobile-unity/releases)

``` sh
# adding the JovDK-Core submodule (dependecie):
git submodule add "https://github.com/joveem/JovDK" "Assets/_JovDK/_JovDK-Core"
#
```

##### Adding repo as git-submodule:

``` sh
# adding the submodule:
git submodule add "https://github.com/joveem/JovDK-App-Monetization-AdMob" "Assets/_JovDK/JovDK-App-Monetization-AdMob"
#
```


##### If the project already has the submodule:

``` sh
# Install dependencies:
git submodule update --recursive --init
#
```

## Rewarded-ad integration

`RewardedAdsHandler` retains the existing serialized IDs, availability properties and public callbacks. It adapts SDK callbacks onto Unity's main thread; `RewardedAdSession` owns initialization readiness, load/presentation generations, duplicate/stale completion guards and at-most-once rewards. The existing scene Assembler owns Display subscriptions. Destruction invalidates pending callbacks and releases the loaded ad.

Editor and development builds use Google's official rewarded test units. Release builds use the serialized Android/iOS IDs. No production identifiers are replaced in scenes or prefabs.

The host compiles against Google Mobile Ads Unity 8.7.0 and 11.5.0. Keep SDK/native dependencies coordinated in the consuming Unity project. The current adapter accepts rewards during the active presentation only; validate callback ordering for any mediation adapter before adopting it. Initialization with no ready adapter leaves ads unavailable; loading failure can be retried through `LoadRewardedAd`.

[Editor tests](Tests/Editor/RewardedAdSessionTests.cs) cover readiness, duplicate operations, late callbacks, retries, disposal, reward uniqueness and development test-unit selection. These are technical gates, not proof of consent UX, live-network behavior or device QA. The consuming app remains responsible for its consent/privacy flow and store test-account setup.

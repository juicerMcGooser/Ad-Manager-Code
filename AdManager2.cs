using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace WordConnect
{
    public class AdManager2 : MonoBehaviour, IInterstitialAdListener, IBannerAdListener,
            IRewardedVideoAdListener, IMrecAdListener, IAppodealInitializationListener
    {
        #region Instance
        public static AdManager2 instance;
        #endregion Elements

        [SerializeField] private int coinsToReward;
        [SerializeField] public string appKey = "fcb443747070ea8afe9734eef714538ef5dbdc57414d4eea";
        [SerializeField] bool isTesting;

        public GameObject consentPanel = null;
        private int consentShown;
        private int consentAnswer;
        private int removedAds;
        static int interstitialCount;
        public int interstitialShow;

        void Start()
        {
            consentShown = PlayerPrefs.GetInt("consentV", 0);
            consentAnswer = PlayerPrefs.GetInt("consentA", 1);
            removedAds = PlayerPrefs.GetInt("removedAds", 0);

            if (PlayerPrefs.GetInt("consentV", consentShown) == 0)
            {
                consentPanel.SetActive(true);
            }
            else
            {
                Initialize();
            }

            instance = this;
        }

        public void RemoveAds()
        {
            Appodeal.destroy(Appodeal.BANNER);
            Appodeal.destroy(Appodeal.INTERSTITIAL);
            if (PlayerPrefs.GetInt("removedAds", removedAds) == 0)
                PlayerPrefs.SetInt("removedAds", 1);
            Initialize();
        }

        public void Initialize()
        {
            if (PlayerPrefs.GetInt("removedAds", removedAds) == 0)
            {
                Debug.Log("YOU UNDERESTIMATE MY POWER!!!");
                if (PlayerPrefs.GetInt("consentV", consentShown) == 0)
                    PlayerPrefs.SetInt("consentV", 1);

                if (PlayerPrefs.GetInt("consentA", consentAnswer) == 1)
                {
                    int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO | Appodeal.MREC;
                    Appodeal.initialize(appKey, adTypes, (IAppodealInitializationListener)this);
                    Appodeal.updateCcpaConsent(Appodeal.CcpaUserConsent.OptIn);
                    Appodeal.updateGdprConsent(Appodeal.GdprUserConsent.Personalized);
                }
                if (PlayerPrefs.GetInt("consentA", consentAnswer) == 0)
                {
                    int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO | Appodeal.MREC;
                    Appodeal.initialize(appKey, adTypes, (IAppodealInitializationListener)this);
                    Appodeal.updateCcpaConsent(Appodeal.CcpaUserConsent.OptOut);
                    Appodeal.updateGdprConsent(Appodeal.GdprUserConsent.NonPersonalized);
                }
                Appodeal.setBannerCallbacks(this);
                Appodeal.setInterstitialCallbacks(this);
                Appodeal.setRewardedVideoCallbacks(this);
                Appodeal.setMrecCallbacks(this);

                Appodeal.muteVideosIfCallsMuted(true);
                Appodeal.setAutoCache(Appodeal.INTERSTITIAL, false);
                Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
                Appodeal.setUseSafeArea(true);

                Appodeal.disableLocationPermissionCheck();
                Appodeal.setTriggerOnLoadedOnPrecache(Appodeal.INTERSTITIAL, true);
                Appodeal.setSmartBanners(true);
                Appodeal.setBannerAnimation(true);
                Appodeal.setTabletBanners(true);
                Appodeal.setChildDirectedTreatment(false);
                Appodeal.setTesting(isTesting);
            }
            if (PlayerPrefs.GetInt("removedAds", removedAds) == 1)
            {

                Debug.Log("I have the HIGH GROUND!!!");

                if (PlayerPrefs.GetInt("consentV", consentShown) == 0)
                    PlayerPrefs.SetInt("consentV", 1);

                if (PlayerPrefs.GetInt("consentA", consentAnswer) == 1)
                {
                    int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO | Appodeal.MREC;
                    Appodeal.initialize(appKey, adTypes, (IAppodealInitializationListener)this);
                    Appodeal.updateCcpaConsent(Appodeal.CcpaUserConsent.OptIn);
                    Appodeal.updateGdprConsent(Appodeal.GdprUserConsent.Personalized);
                }
                if (PlayerPrefs.GetInt("consentA", consentAnswer) == 0)
                {
                    int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO | Appodeal.MREC;
                    Appodeal.initialize(appKey, adTypes, (IAppodealInitializationListener)this);
                    Appodeal.updateCcpaConsent(Appodeal.CcpaUserConsent.OptOut);
                    Appodeal.updateGdprConsent(Appodeal.GdprUserConsent.NonPersonalized);
                }

                Appodeal.setRewardedVideoCallbacks(this);
                Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
                Appodeal.setUseSafeArea(true);

                Appodeal.disableLocationPermissionCheck();
                Appodeal.setChildDirectedTreatment(false);
                Appodeal.setTesting(isTesting);
            }
        }

        #region Personal Consent Window
        public void AcceptTerms()
        {
            PlayerPrefs.SetInt("consentA", 1);
            Initialize();
            consentPanel.SetActive(false);
        }

        public void DeclineTerms()
        {
            PlayerPrefs.SetInt("consentA", 0);
            Initialize();
            consentPanel.SetActive(false);
        }

        public void DataPrivacy()
        {
            consentPanel.SetActive(true);
        }
        #endregion

        #region Ad Functions
        public void ShowInterstitial()
        {
            if(PlayerPrefs.GetInt("removedAds", removedAds) == 0)
            {
                interstitialCount++;

                if (Appodeal.isLoaded(Appodeal.INTERSTITIAL) && Appodeal.canShow(Appodeal.INTERSTITIAL, "gameOverPlacement") && interstitialCount >= interstitialShow)
                {
                    interstitialCount = 0;
                    Appodeal.show(Appodeal.INTERSTITIAL);
                }
                else
                {
                    Appodeal.cache(Appodeal.INTERSTITIAL);
                }
            }
        }

        public void ShowRewardedVideo()
        {
            if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO) && Appodeal.canShow(Appodeal.REWARDED_VIDEO, "default"))
            {
                Appodeal.show(Appodeal.REWARDED_VIDEO);
            }
            else
            {
                Appodeal.cache(Appodeal.REWARDED_VIDEO);
            }
        }

        public void OnDestroy()
        {
            Appodeal.destroy(Appodeal.BANNER);
        }

        public void ShowBannerBottom()
        {
            if (PlayerPrefs.GetInt("removedAds", removedAds) == 0)
                Appodeal.show(Appodeal.BANNER_BOTTOM, "default");
            else
            {
                Debug.Log("I lOVE you!!!");
            }
        }

        public void ShowBannerTop()
        {
            if (PlayerPrefs.GetInt("removedAds", removedAds) == 0)
                Appodeal.show(Appodeal.BANNER_TOP, "default");
        }

        public void HideBanner()
        {
            if (PlayerPrefs.GetInt("removedAds", removedAds) == 0)
                Appodeal.hide(Appodeal.BANNER);
            else
            {
                Debug.Log("I LOVE you!!!");
            }
        }

        public void HideBannerView()
        {
            Appodeal.hideBannerView();
        }

        public void HideMrecView()
        {
            Appodeal.hideMrecView();
        }

        public void ShowBannerLeft()
        {
            Appodeal.show(Appodeal.BANNER_LEFT);
        }

        public void ShowBannerRight()
        {
            Appodeal.show(Appodeal.BANNER_RIGHT);
        }

        public void OnRewardAdGranted()
        {
            // Get the current amount of coins
            int animateFromCoins = WordConnect.GameController.Instance.Coins;

            // Give the amount of coins
            WordConnect.GameController.Instance.GiveCoins(coinsToReward, false);

            // Get the amount of coins now after giving them
            int animateToCoins = WordConnect.GameController.Instance.Coins;

            // Show the popup to the user so they know they got the coins
            BBG.PopupManager.Instance.Show("reward_ad_granted", new object[] { coinsToReward, animateFromCoins, animateToCoins });
        }

        public void ShowMrecView()
        {
            Appodeal.showMrecView(Screen.currentResolution.height - Screen.currentResolution.height / 10,
                Appodeal.BANNER_HORIZONTAL_CENTER, "default");
        }
        #endregion
        #region Banner callback handlers

        public void onBannerLoaded(int height, bool precache)
        {
            Debug.Log("onBannerLoaded");
            Debug.Log($"Banner height - {height}");
            Debug.Log($"Banner precache - {precache}");
            Debug.Log($"getPredictedEcpm(): {Appodeal.getPredictedEcpm(Appodeal.BANNER)}");
        }

        public void onBannerFailedToLoad()
        {
            Debug.Log("onBannerFailedToLoad");
        }

        public void onBannerShown()
        {
            Debug.Log("onBannerShown");
        }

        public void onBannerShowFailed()
        {
            Debug.Log("onBannerShowFailed");
        }

        public void onBannerClicked()
        {
            Debug.Log("onBannerClicked");
        }

        public void onBannerExpired()
        {
            Debug.Log("onBannerExpired");
        }

        #endregion
        #region Interstitial callback handlers

        // Called when interstitial was loaded (precache flag shows if the loaded ad is precache)
        public void onInterstitialLoaded(bool isPrecache)
        {
            print("Interstitial loaded");
        }

        // Called when interstitial failed to load
        public void onInterstitialFailedToLoad()
        {
            print("Interstitial failed");
        }

        // Called when interstitial was loaded, but cannot be shown (internal network errors, placement settings, or incorrect creative)
        public void onInterstitialShowFailed()
        {
            print("Interstitial show failed");
        }

        // Called when interstitial is shown
        public void onInterstitialShown()
        {
            print("Interstitial opened");
        }

        // Called when interstitial is closed
        public void onInterstitialClosed()
        {
            print("Interstitial closed");
        }

        // Called when interstitial is clicked
        public void onInterstitialClicked()
        {
            print("Interstitial clicked");
        }

        // Called when interstitial is expired and can not be shown
        public void onInterstitialExpired()
        {
            print("Interstitial expired");
        }

        #endregion
        #region Rewarded Video callback handlers

        //Called when rewarded video was loaded (precache flag shows if the loaded ad is precache).
        public void onRewardedVideoLoaded(bool isPrecache)
        {
            print("Video loaded");
        }

        // Called when rewarded video failed to load
        public void onRewardedVideoFailedToLoad()
        {
            print("Video failed");
        }

        // Called when rewarded video was loaded, but cannot be shown (internal network errors, placement settings, or incorrect creative)
        public void onRewardedVideoShowFailed()
        {
            print("Video show failed");
        }

        // Called when rewarded video is shown
        public void onRewardedVideoShown()
        {
            print("Video shown");
        }

        // Called when reward video is clicked
        public void onRewardedVideoClicked()
        {
            print("Video clicked");
        }

        // Called when rewarded video is closed
        public void onRewardedVideoClosed(bool finished)
        {
            print("Video closed");
        }

        // Called when rewarded video is viewed until the end
        public void onRewardedVideoFinished(double amount, string name)
        {
            print("Reward: " + amount + " " + name);
            OnRewardAdGranted();
        }

        //Called when rewarded video is expired and can not be shown
        public void onRewardedVideoExpired()
        {
            print("Video expired");
        }
        #endregion
        #region Mrec callback handlers

        public void onMrecLoaded(bool precache)
        {
            Debug.Log($"onMrecLoaded. Precache - {precache}");
            Debug.Log($"getPredictedEcpm(): {Appodeal.getPredictedEcpm(Appodeal.MREC)}");
        }

        public void onMrecFailedToLoad()
        {
            Debug.Log("onMrecFailedToLoad");
        }

        public void onMrecShown()
        {
            Debug.Log("onMrecShown");
        }

        public void onMrecShowFailed()
        {
            Debug.Log("onMrecShowFailed");
        }

        public void onMrecClicked()
        {
            Debug.Log("onMrecClicked");
        }

        public void onMrecExpired()
        {
            Debug.Log("onMrecExpired");
        }

        #endregion
        #region AppodealInitializeListener

        public void onInitializationFinished(List<string> errors)
        {
            string output = errors == null ? string.Empty : string.Join(", ", errors);
            Debug.Log($"onInitializationFinished(errors:[{output}])");

            Debug.Log($"isAutoCacheEnabled() for banner: {Appodeal.isAutoCacheEnabled(Appodeal.BANNER)}");
            Debug.Log($"isInitialized() for banner: {Appodeal.isInitialized(Appodeal.BANNER)}");
            Debug.Log($"isSmartBannersEnabled(): {Appodeal.isSmartBannersEnabled()}");
            Debug.Log($"getUserId(): {Appodeal.getUserId()}");
            Debug.Log($"getSegmentId(): {Appodeal.getSegmentId()}");
            Debug.Log($"getRewardParameters(): {Appodeal.getRewardParameters()}");
            Debug.Log($"getNativeSDKVersion(): {Appodeal.getNativeSDKVersion()}");

            var networksList = Appodeal.getNetworks(Appodeal.REWARDED_VIDEO);
            output = networksList == null ? string.Empty : string.Join(", ", (networksList.ToArray()));
            Debug.Log($"getNetworks() for RV: {output}");
        }

        #endregion
    }
}
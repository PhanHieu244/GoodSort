using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System;
using UnityEngine.Advertisements;
/*
 * 
 * Document for Unity Ads : https://docs.unity.com/ads/ImplementingBasicAdsUnity.html
 */
/*
 * 
 * Document for Google Admob : https://developers.google.com/admob/unity/quick-start
 */

public class AdsControl : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{

    private static AdsControl instance;

    //for Admob
    #region ADMOB_KEY
    public string Android_AppID, IOS_AppID;

    public string Android_Banner_Key, IOS_Banner_Key;

    public string Android_Interestital_Key, IOS_Interestital_Key;

    public string Android_RW_Key, IOS_RW_Key;

    #endregion

    #region UNITY_ADS_KEY
    public string androidUnityGameId;
    public string iOSUnityGameId;
    public string androidUnityAdUnitId;
    public string iOSUnityAdUnitId;

    [HideInInspector]
    public string adUnityUnitId;

    public string androidUnityRWAdUnitId;
    public string iOSUnityRWAdUnitId;

    [HideInInspector]
    public string adUnityRWUnitId = null; // This will remain null for unsupported platforms

    public bool testMode;
    private string unityGameId;
    #endregion

    public enum ADS_TYPE
    {
        ADMOB,
        UNITY,
        MEDIATION
    }

    public ADS_TYPE currentAdsType;

    private bool isShowingAppOpenAd;

    public static AdsControl Instance { get { return instance; } }

    void Awake()
    {
        if (FindObjectsOfType(typeof(AdsControl)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }


        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
       
       
    }

    private void Update()
    {

    }

   
    #region UNITY_ADS
    public void InitializeUnityAds()
    {
#if UNITY_IOS
        unityGameId = iOSUnityGameId;
#elif UNITY_ANDROID
        unityGameId = androidUnityGameId;
#elif UNITY_EDITOR
            unityGameId = androidUnityGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(unityGameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadUnityAd();
        LoadUnityRWAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Load content to the Ad Unit:
    public void LoadUnityAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + adUnityUnitId);
        Advertisement.Load(adUnityUnitId, this);
    }

    // Show the loaded content in the Ad Unit:
    public void ShowUnityAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + adUnityUnitId);
        Advertisement.Show(adUnityUnitId, this);
    }



    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }


    //public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) { }


    // Call this public method when you want to get an ad ready to show.
    public void LoadUnityRWAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + adUnityRWUnitId);
        Advertisement.Load(adUnityRWUnitId, this);
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowUnityRWAd()
    {

        // Then show the ad:
        Advertisement.Show(adUnityRWUnitId, this);
    }

    private Action<string, UnityAdsShowCompletionState> OnRewardAdsComplete = delegate (string ID, UnityAdsShowCompletionState callBackState) { };

    public void PlayUnityVideoAd(Action<string, UnityAdsShowCompletionState> OnAdsComplete)
    {
        Advertisement.Show(adUnityRWUnitId, this);
        OnRewardAdsComplete = OnAdsComplete;
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(adUnityRWUnitId))
        {

        }
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(adUnityRWUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {

            LoadUnityRWAd();
            OnRewardAdsComplete.Invoke(adUnitId, showCompletionState);
        }

        if (adUnitId.Equals(adUnityUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            LoadUnityAd();
        }
    }

    #endregion

    public void ShowInterstital()
    {
        if (IsRemoveAds())
            return;

        
        /*
        int numberShow = PlayerPrefs.GetInt("ShowAds");

        if (numberShow < 1)
        {
            numberShow++;
            PlayerPrefs.SetInt("ShowAds", numberShow);
            return;
        }
        else
        {
            numberShow = 0;
            PlayerPrefs.SetInt("ShowAds", numberShow);

            if (currentAdsType == ADS_TYPE.ADMOB)
            {
                if (interstitialAd != null && interstitialAd.CanShowAd())
                {
                    interstitialAd.Show();
                }
            }
            else if (currentAdsType == ADS_TYPE.UNITY)
            {
                ShowUnityAd();
            }
            else if (currentAdsType == ADS_TYPE.MEDIATION)
            {
                if (interstitialAd != null && interstitialAd.CanShowAd())
                {
                    interstitialAd.Show();
                }
                else
                {
                    ShowUnityAd();
                }
            }

        }
        */
    }

    public void RemoveAds()
    {
        

       
    }

    public bool IsRemoveAds()
    {
        if (!PlayerPrefs.HasKey("removeAds"))
        {
            return false;
        }
        else
        {
            if (PlayerPrefs.GetInt("removeAds") == 1)
            {
                return true;
            }
        }
        return false;
    }
}


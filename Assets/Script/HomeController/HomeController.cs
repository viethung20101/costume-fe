using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
public class HomeController : MonoBehaviour
{
    [Header("Information User")]
    public TMP_Text TitleText;
    private UserProfile userProfile;

    [Header("Vertical Scroll")]
    public Transform EraContent;
    public GameObject EraItemPrefab;

    [Header("Horizontal Scroll")]
    public Transform CostumeContent;
    public GameObject CostumeItemPrefab;

    private string selectedEraId;

    void Awake()
    {
        CheckAuthentication();
        LoadUserProfile();
        GetAllErasActive();
    }
    void CheckAuthentication()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogWarning("No access token found, redirecting to Login scene.");
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Auth");
        }
    }

    void LoadUserProfile()
    {
        string userProfileJson = PlayerPrefs.GetString("userProfile", "");
        if (!string.IsNullOrEmpty(userProfileJson))
        {
            userProfile = JsonUtility.FromJson<UserProfile>(userProfileJson);
            TitleText.text = "Welcome, " + userProfile.name + "!";
        }
        else
        {
            Debug.LogWarning("No user profile found in PlayerPrefs.");
        }
    }

    /////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////
    #region APIs
    void GetAllErasActive()
    {
        StartCoroutine(new EraAPIs().GetAllErasRequest(
            (result) =>
            {

                if (EraManager.Instance.eras.Count > 0)
                {
                    PlayerPrefs.SetString("selectedEraId", EraManager.Instance.eras[3].id);
                    PlayerPrefs.Save();
                    selectedEraId = EraManager.Instance.eras[3].id;
                }

                GenerateEraMenu(EraManager.Instance.eras);
                GetCostumesByEraId(selectedEraId);
            },
            (error) =>
            {
                Debug.LogError("Error fetching active eras: " + error);
            }
        ));
    }
    void GetCostumesByEraId(string eraId)
    {
        StartCoroutine(new CostumeAPIs().GetCostumesByEraIdRequest(eraId,
           (result) =>
           {
               var json = JsonConvert.DeserializeObject<CostumeResponse>(result);
               GenerateCostumeMenu(json.data);
           },
           (error) =>
           {
               Debug.LogError("Error fetching costumes: " + error);
           }
       ));
    }
    #endregion
    /////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////

    #region Generate Menu
    void GenerateEraMenu(List<EraModel> eras) // Only call one
    {
        // Re-initialize ScrollSnapEffect
        ScrollSnapEffectHorizontal scrollSnap = FindFirstObjectByType<ScrollSnapEffectHorizontal>();
        foreach (var era in eras)
        {
            GameObject eraItem = Instantiate(EraItemPrefab, EraContent);
            eraItem.GetComponentInChildren<TMP_Text>().text = era.name;
            eraItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("selectedEraId", era.id);
                PlayerPrefs.Save();
                selectedEraId = era.id;
                GetCostumesByEraId(selectedEraId);
                if (scrollSnap != null)
                {
                    RectTransform itemRect = eraItem.GetComponent<RectTransform>();
                    scrollSnap.SnapToItem(itemRect);
                }
            });

        }
    }
    void GenerateCostumeMenu(List<CostumeModel> costumes) // Call every time select era
    {
        // Re-initialize ScrollSnapEffect
        ScrollSnapEffect scrollSnap = FindFirstObjectByType<ScrollSnapEffect>();

        foreach (Transform child in CostumeContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var costume in costumes)
        {
            GameObject costumeItem = Instantiate(CostumeItemPrefab, CostumeContent);
            costumeItem.GetComponentInChildren<TMP_Text>().text = costume.name;
            costumeItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("selectedCostumeId", costume.id);
                PlayerPrefs.Save();
                // Snap to the selected item
                if (scrollSnap != null)
                {
                    RectTransform itemRect = costumeItem.GetComponent<RectTransform>();
                    scrollSnap.SnapToItem(itemRect);
                }
            });

        }



        if (scrollSnap != null)
        {
            scrollSnap.Initialize();
        }


    }
    #endregion
}


using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections;

public class HomeController : MonoBehaviour
{
    [Header("Information User")]
    public TMP_Text TitleText;

    [Header("Vertical Scroll")]
    public Transform EraContent;
    public GameObject EraItemPrefab;

    [Header("Horizontal Scroll")]
    public Transform CostumeContent;
    public GameObject CostumeItemPrefab;

    private string selectedEraId;
    private string selectedCostumeId;

    void Awake()
    {
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

    void LoadTitleCostume(string title)
    {
        TitleText.text = title;
    }

    /////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////
    #region APIs
    void GetAllErasActive()
    {
        StartCoroutine(new EraAPIs().GetAllErasRequest(
            (result) =>
            {
                GenerateEraMenu(EraManager.Instance.eras);
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
        ScrollSnapVertical scrollSnap = FindFirstObjectByType<ScrollSnapVertical>();
        foreach (Transform child in EraContent) DestroyImmediate(child.gameObject);

        foreach (var (era, index) in eras.Select((value, i) => (value, i)))
        {
            int capturedIndex = index;
            GameObject eraItem = Instantiate(EraItemPrefab, EraContent);
            eraItem.GetComponentInChildren<TMP_Text>().text = era.name;
            eraItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                scrollSnap.SetItemSelected(capturedIndex);
            });
        }

        if (scrollSnap != null)
        {
            StartCoroutine(SnapAfterBuild(scrollSnap, eras));
        }
    }

    void GenerateCostumeMenu(List<CostumeModel> costumes)
    {

        foreach (Transform child in CostumeContent)
        {
            Destroy(child.gameObject);
        }

        ScrollSnapEffectHorizontal scrollSnap = FindFirstObjectByType<ScrollSnapEffectHorizontal>();

        foreach (var (costume, index) in costumes.Select((value, i) => (value, i)))
        {
            int capturedIndex = index;
            GameObject costumeItem = Instantiate(CostumeItemPrefab, CostumeContent);

            costumeItem.GetComponentInChildren<TMP_Text>().text = costume.name;
            costumeItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                scrollSnap.SetItemSelected(capturedIndex);
            });
        }

        if (scrollSnap != null)
        {
            StartCoroutine(SnapAfterBuild1(scrollSnap, costumes));
        }
    }

    /// <summary>
    /// Đợi 1 frame + rebuild layout → sau đó snap
    /// </summary>
    private IEnumerator SnapAfterBuild(ScrollSnapVertical scrollSnap, List<EraModel> eras)
    {
        // Đợi vài frame để Unity build layout xong
        yield return null;
        yield return new WaitForEndOfFrame();
        int midIndex = eras.Count > 0 ? Mathf.RoundToInt((eras.Count - 1) / 2f) : -1;
        scrollSnap.Initialize();
        scrollSnap.OnItemSelected = (index, item) =>
        {
            PlayerPrefs.SetString("selectedEraId", eras[index].id);
            PlayerPrefs.Save();
            selectedEraId = eras[index].id;
            GetCostumesByEraId(selectedEraId);
        };
        // đảm bảo content vẫn tồn tại (sau khi Destroy/Instantiate)
        if (scrollSnap != null && scrollSnap.content != null)
        {
            scrollSnap.SetItemSelected(midIndex);
        }
    }

    private IEnumerator SnapAfterBuild1(ScrollSnapEffectHorizontal scrollSnap, List<CostumeModel> costumes)
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        int midIndex = costumes.Count > 0 ? Mathf.RoundToInt((costumes.Count - 1) / 2f) : -1;

        scrollSnap.ForceRebuild();
        scrollSnap.OnItemSelected = (index, item) =>
        {
            LoadTitleCostume(costumes[index].shortDescription);
            Debug.Log("Selected Costume: " + costumes[index].name);
            // PlayerPrefs.SetString("selectedCostumeId", costumes[index].id);
            // PlayerPrefs.Save();
            // selectedCostumeId = costumes[index].id;
        };
        if (scrollSnap != null && scrollSnap.content != null)
        {
            scrollSnap.SetItemSelected(midIndex);
        }
    }

    #endregion
}

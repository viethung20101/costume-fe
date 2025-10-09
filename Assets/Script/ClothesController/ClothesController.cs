using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
public class ClothesController : MonoBehaviour
{
    [Header("Vertical Scroll")]
    public Transform CategoryContent;
    public GameObject CategoryItemPrefab;

    private string selectedCategoryId;
    void Awake()
    {
        GetAllCategoriesActive();
    }

    /////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////
    #region APIs
    void GetAllCategoriesActive()
    {
        StartCoroutine(new CategoryAPIs().GetAllCategoriesRequest(
            (result) =>
            {
                var json = JsonConvert.DeserializeObject<CategoryResponse>(result);
                GenerateCategoryMenu(json.data);
            },
            (error) =>
            {
                Debug.LogError("Error fetching active categories: " + error);
            }
        ));
    }
    #endregion

    #region Generate Category Menu
    void GenerateCategoryMenu(List<CategoryModel> categories)
    {
        ScrollSnapVertical scrollSnap = FindFirstObjectByType<ScrollSnapVertical>();
        foreach (Transform child in CategoryContent) DestroyImmediate(child.gameObject);
        Debug.Log("Số lượng category: " + categories.Count);
        foreach (var (category, index) in categories.Select((value, i) => (value, i)))
        {
            int capturedIndex = index;
            GameObject categoryItem = Instantiate(CategoryItemPrefab, CategoryContent);
            categoryItem.GetComponentInChildren<TMP_Text>().text = category.name;
            categoryItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                scrollSnap.SetItemSelected(capturedIndex);
            });
        }

        if (scrollSnap != null)
        {
            StartCoroutine(SnapAfterBuild(scrollSnap, categories));
        }
    }
    /// <summary>
    /// Đợi 1 frame + rebuild layout → sau đó snap
    /// </summary>
    private IEnumerator SnapAfterBuild(ScrollSnapVertical scrollSnap, List<CategoryModel> categories)
    {
        // Đợi vài frame để Unity build layout xong
        yield return null;
        yield return new WaitForEndOfFrame();
        int midIndex = categories.Count > 0 ? Mathf.RoundToInt((categories.Count - 1) / 2f) : -1;
        scrollSnap.Initialize();
        scrollSnap.OnItemSelected = (index, item) =>
        {
            PlayerPrefs.SetString("selectedCategoryId", categories[index].id);
            PlayerPrefs.Save();
            selectedCategoryId = categories[index].id;
        };
        // đảm bảo content vẫn tồn tại (sau khi Destroy/Instantiate)
        if (scrollSnap != null && scrollSnap.content != null)
        {
            scrollSnap.SetItemSelected(midIndex);
        }
    }
    #endregion

    public void OnBackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Auth");
    }
}

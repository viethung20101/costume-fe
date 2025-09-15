using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LocalizationData {
    public Dictionary<string, string> entries;
}

public class LanguageManager : MonoBehaviour {
    private static Dictionary<string, string> localizedText;

    public static void LoadLanguage(string langCode) {
        TextAsset langFile = Resources.Load<TextAsset>("Lang/" + langCode);
        if (langFile != null) {
            localizedText = JsonUtility.FromJson<LocalizationData>(langFile.text).entries;
            Debug.Log("Loaded language: " + langCode);
        } else {
            Debug.LogWarning("Language file not found: " + langCode);
        }
    }

    public static string Get(string key) {
        if (localizedText != null && localizedText.ContainsKey(key)) {
            return localizedText[key];
        }
        return key; // fallback nếu không tìm thấy
    }
}

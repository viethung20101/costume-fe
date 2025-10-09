using UnityEngine;
using System.Collections.Generic;
public class EraManager : MonoBehaviour
{
    public static EraManager Instance;
    public List<EraModel> eras = new List<EraModel>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}

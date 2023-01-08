using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutorunSpeedDropdown : MonoBehaviour
{
    public int[] stepPerSceondList;
    TMP_Dropdown dropdown;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(int idx)
    {
        var speed = stepPerSceondList[idx];
        Debug.Log($"Set speed to {speed}");
        gameManager.autorunningStepPerSecond = speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ConditionToggle : MonoBehaviour
{
    public UnityEngine.UI.Text label;
    UnityEngine.UI.Toggle toggle;

    public event EventHandler<Tuple<string, bool>> toggleChanged;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<UnityEngine.UI.Toggle>();
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(bool value)
    {
        toggleChanged?.Invoke(this, new Tuple<string, bool>(label.text, toggle.isOn));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

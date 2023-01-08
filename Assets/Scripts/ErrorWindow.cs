using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class ErrorWindow : MonoBehaviour
{
    public GameObject mainUI;
    public TMP_InputField inputField;
    public DynamicBehaviour dynamicBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        mainUI.SetActive(false);

        dynamicBehaviour.d.errorEvent += OnError;

        // dynamicBehaviour.
    }

    void OnError(object sender, Exception e)
    {
        Debug.Log($"OnError sender={sender}, e={e}");
        inputField.text = "Solver failed:\n\n" + e.ToString();
        mainUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

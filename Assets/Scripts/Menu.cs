using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menuUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            menuUI.SetActive(!menuUI.activeSelf);
        }
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnResumeButtonClicked()
    {
        menuUI.SetActive(false);
    }
}

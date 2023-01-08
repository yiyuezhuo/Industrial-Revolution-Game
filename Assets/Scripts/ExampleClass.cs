
using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    void OnGUI()
    {
        // Make a button using a custom GUIContent parameter to pass in the tooltip.
        GUI.Button(new Rect(10, 10, 100, 20), new GUIContent("Click me", "This is the tooltip"));

        // Display the tooltip from the element that has mouseover or keyboard focus
        GUI.Label(new Rect(10, 40, 100, 40), GUI.tooltip);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public event EventHandler<int> TurnIncreased;
    // public event EventHandler<int> TurnIncreasedPost;

    public int turn = 0;

    public bool autorunning = false;
    public float autorunnigSecondPerStep = 1;
    public float autorunningStepPerSecond
    {
        get => 1 / autorunnigSecondPerStep;
        set
        {
            autorunnigSecondPerStep = 1 / value;
        }
    }
    public float elapsed = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(autorunning)
        {
            elapsed += Time.deltaTime;
            while(elapsed >= autorunnigSecondPerStep)
            {
                elapsed -= autorunnigSecondPerStep;
                Step();
            }
        }
    }

    public void Step()
    {
        turn += 1;
        TurnIncreased?.Invoke(this, turn);
        // TurnIncreasedPost?.Invoke(this, turn);
    }

    public void StartAuoturun()
    {
        autorunning = true;
    }

    public void StopAutorun()
    {
        autorunning = false;
        elapsed = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public event EventHandler<int> TurnIncreased;
    public event EventHandler<int> TurnIncreasedPost;

    public int turn = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step()
    {
        turn += 1;
        TurnIncreased?.Invoke(this, turn);
        TurnIncreasedPost?.Invoke(this, turn);
    }
}

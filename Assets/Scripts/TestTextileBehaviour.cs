using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTextileBehaviour : MarketBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float Demand(float p) => TestCurve.DemandTextile(p);
    public override float Supply(float p) => TestCurve.SupplyTextile(p, TestCurve.initialCottonPrice);

}

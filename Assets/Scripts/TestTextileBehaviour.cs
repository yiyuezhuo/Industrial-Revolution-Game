using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTextileBehaviour : MarketBehaviour
{
    // Start is called before the first frame update
    public Market cottonMarket;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override double Demand(double p) => TestCurve.DemandTextile(p);
    public override double Supply(double p) => TestCurve.SupplyTextile(p, cottonMarket.prevPrice);

}

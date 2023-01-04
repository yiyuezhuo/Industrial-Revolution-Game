using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public interface ITestMarket
{
    float Demand(float p);
    float Supply(float p);
}
*/

public abstract class MarketBehaviour : MonoBehaviour
{
    public abstract float Demand(float p);
    public abstract float Supply(float p);
}

public class TestCottonBehaviour : MarketBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float Demand(float p) => TestCurve.DemandCotton(p, TestCurve.initialTextilePrice);
    public override float Supply(float p) => TestCurve.SupplyCotton(p);
}

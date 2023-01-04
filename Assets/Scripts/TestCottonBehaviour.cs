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

/*
public interface ISupplier
{
    string Name { get; }
    float Supply(float price);
}

public interface IDemander
{
    string Name { get; }
    float Demand(float price);
}
*/

public abstract class MarketBehaviour : MonoBehaviour
{

    // public string Name = "Trade Good";

    public abstract double Demand(double p);
    public abstract double Supply(double p);
}

public class TestCottonBehaviour : MarketBehaviour
{
    public Market textileMarket;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override double Demand(double p) => TestCurve.DemandCotton(p, textileMarket.prevPrice);
    public override double Supply(double p) => TestCurve.SupplyCotton(p);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

using MathNet.Numerics.RootFinding;

public static class TestCurve
{
    public static float SupplyCotton(float priceCotton)
    {
        return Mathf.Sqrt(Mathf.Max(priceCotton - 0.2f, 0));
    }

    public static float SupplyTextileWorkshop(float priceTextile, float priceCotton)
    {
        var price = priceTextile - priceCotton;
        return Mathf.Sqrt(Mathf.Max(price - 0.2f, 0));
    }

    public static float SupplyTextileFactory(float priceTextile, float priceCotton)
    {
        var price = priceTextile - priceCotton;
        return Mathf.Pow(Mathf.Max(price, 0), 0.7f);
    }

    public static float DemandCottonWorkshop(float priceCotton, float priceTextile)
    {
        return SupplyTextileWorkshop(priceTextile, priceCotton);
    }

    public static float DemandCottonFactory(float priceCotton, float priceTextile)
    {
        return SupplyTextileFactory(priceTextile, priceCotton);
    }

    public static float DemandTextile(float priceTextile)
    {
        return 1 / priceTextile;
    }

    public static float SupplyTextile(float priceTextile, float priceCotton)
    {
        return SupplyTextileWorkshop(priceTextile, priceCotton) + SupplyTextileFactory(priceTextile, priceCotton);
    }

    public static float DemandCotton(float priceCotton, float priceTextile)
    {
        return DemandCottonWorkshop(priceCotton, priceTextile) + DemandCottonFactory(priceCotton, priceTextile);
    }

    public static float initialCottonPrice = 1f;
    // public static float initialCottonPrice = 0.1f;
    public static float initialTextilePrice = 2f;
}

public class Market : MonoBehaviour
{
    public LineRenderer supplyCurve;
    public LineRenderer demandCurve;
    public TMP_Text leftBottomLabel;
    public TMP_Text rightBottomLabel;
    public TMP_Text leftTopLabel;
    public TMP_Text priceLabel;
    public TMP_Text quantityLabel;
    public LineRenderer rootHorizontalLine;
    public LineRenderer rootVerticalLine;

    // public Vector2 xlim = new Vector2(0.5f, 2.0f);
    // public Vector2 ylim = new Vector2(0.0f, 2.0f);
    public Vector2 priceLim = new Vector2(0.2f, 3.0f);
    public Vector2 quantityLim = new Vector2(0, 3.0f);

    public int samples = 100;

    public GameManager gameManager;

    Func<float, float> Demand { get => market.Demand; }
    Func<float, float> Supply { get => market.Supply; }
    public MarketBehaviour market;

    public float prevPrice = 1;
    public float newPrice = 1;

    public event EventHandler<float> prevPriceChanged;

    // Start is called before the first frame update
    void Start()
    {
        // Test
        /*
        Demand = (cottonPrice) => TestCurve.DemandCotton(cottonPrice, TestCurve.initialTextilePrice);
        Supply = TestCurve.SupplyCotton;
        */
        // Demand = TestCurve.DemandTextile;
        // Supply = (textilePrice) => TestCurve.SupplyTextile(textilePrice, TestCurve.initialCottonPrice);

        // Sync();

        gameManager.TurnIncreased += Step;
        gameManager.TurnIncreasedPost += StepPost;
    }

    float[] PriceLinspace(Vector2 priceLim)
    {
        var price = new float[samples];
        var r = priceLim.y - priceLim.x;
        var n = (float)samples;
        for (var i = 0; i < samples; i++)
            price[i] = priceLim.x + r * (i / n);
        return price;
    }

    double Objective(double p)
    {
        Debug.Log($"p={p}");
        return (float)Demand((float)p) - Supply((float)p);
    }

    void Step(object sender, int turn)
    {
        newPrice = Sync();
    }

    void StepPost(object sender, int turn)
    {
        prevPriceChanged?.Invoke(this, newPrice);
        prevPrice = newPrice;
    }

    float Sync()
    {
        // DEBUG
        /*
        var r = Secant.FindRoot(x => x - 1f, -1, -2, -5, 5);
        Debug.Log($"r={r}");
        */


        // Func<double, double> F = (p) => Demand((float)p) - Supply((float)p); // TODO: prevent those ugly casting
        // var rootPrice = (float)Secant.FindRoot(F, 1, 1.5, priceLim.x, priceLim.y);
        
        
        var rootPrice = (float)Secant.FindRoot(Objective, 1, 1.5, priceLim.x, priceLim.y, 1e-4);
        var rootQuantity = Supply(rootPrice);
        Debug.Log($"rootPrice={rootPrice}, rootQuantity={rootQuantity}");

        priceLabel.text = rootPrice.ToString("0.##");
        quantityLabel.text = rootQuantity.ToString("0.##"); // = Demand(rootPrice)

        priceLabel.transform.localPosition = new Vector3(priceLabel.transform.localPosition.x, rootPrice / priceLim.y - 0.5f, priceLabel.transform.localPosition.z);
        quantityLabel.transform.localPosition = new Vector3(rootQuantity / quantityLim.y - 0.5f, quantityLabel.transform.localPosition.y, quantityLabel.transform.localPosition.z);
        
        rootVerticalLine.positionCount = 2;
        rootVerticalLine.SetPositions(new Vector3[] { 
            new Vector3(rootQuantity / quantityLim.y, 0, 0),
            new Vector3(rootQuantity / quantityLim.y, 1, 0)
        });

        rootHorizontalLine.positionCount = 2;
        rootHorizontalLine.SetPositions(new Vector3[] {
            new Vector3(0, rootPrice / priceLim.y, 0),
            new Vector3(1, rootPrice / priceLim.y, 0)
        });
        

        var demandPrice = PriceLinspace(priceLim);
        var quantityPrice = PriceLinspace(new Vector2(0, priceLim.y));

        // var pr = priceLim.y - priceLim.x;
        // var qr = quantityLim.y - quantityLim.x;
        var demandPositions = demandPrice.Select(p =>
            new Vector3(Demand(p) / quantityLim.y, p / priceLim.y, 0)
        ).ToArray();
        var supplyPositions = quantityPrice.Select(p =>
            new Vector3(Supply(p) / quantityLim.y, p / priceLim.y, 0)
        ).ToArray();

        demandCurve.positionCount = samples;
        demandCurve.SetPositions(demandPositions);

        supplyCurve.positionCount = samples;
        supplyCurve.SetPositions(supplyPositions);
        
        rightBottomLabel.text = quantityLim.y.ToString();
        leftTopLabel.text = priceLim.y.ToString();

        // DEBUG
        /*
        var dp = demandPrice.Select(Demand).ToArray();
        var sp = quantityPrice.Select(Supply).ToArray();
        var dd = demandPrice.Select(p => Demand(p) - Supply(p)).ToArray();
        Debug.Log($"dp={dp}, sp={sp}");
        */

        return rootPrice;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

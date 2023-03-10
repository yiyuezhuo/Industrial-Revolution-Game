using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

using MathNet.Numerics.RootFinding;

public static class TestCurve
{
    public static double SupplyCotton(double priceCotton)
    {
        return Math.Sqrt(Math.Max(priceCotton - 0.2, 0));
    }

    public static double SupplyTextileWorkshop(double priceTextile, double priceCotton)
    {
        var price = priceTextile - priceCotton;
        return Math.Sqrt(Math.Max(price - 0.2, 0));
    }

    public static double SupplyTextileFactory(double priceTextile, double priceCotton)
    {
        var price = priceTextile - priceCotton;
        return Math.Pow(Math.Max(price, 0), 0.7);
    }

    public static double DemandCottonWorkshop(double priceCotton, double priceTextile)
    {
        return SupplyTextileWorkshop(priceTextile, priceCotton);
    }

    public static double DemandCottonFactory(double priceCotton, double priceTextile)
    {
        return SupplyTextileFactory(priceTextile, priceCotton);
    }

    public static double DemandTextile(double priceTextile)
    {
        return 1 / priceTextile;
    }

    public static double SupplyTextile(double priceTextile, double priceCotton)
    {
        return SupplyTextileWorkshop(priceTextile, priceCotton) + SupplyTextileFactory(priceTextile, priceCotton);
    }

    public static double DemandCotton(double priceCotton, double priceTextile)
    {
        return DemandCottonWorkshop(priceCotton, priceTextile) + DemandCottonFactory(priceCotton, priceTextile);
    }

    // public static double initialCottonPrice = 1;
    // public static float initialCottonPrice = 0.1f;
    // public static double initialTextilePrice = 2;
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
    // public Vector2 priceLim = new Vector2(0.2f, 3.0f);
    // public Vector2 quantityLim = new Vector2(0, 3.0f);
    public double priceLimX = 0.2;
    public double priceLimY = 3;
    public double quantityLimX = 0;
    public double quantityLimY = 3;

    public int samples = 100;

    // public GameManager gameManager;

    // Func<double, double> Demand { get => market.Demand; }
    // Func<double, double> Supply { get => market.Supply; }
    // public MarketBehaviour market;

    // public double prevPrice = 1;
    // public double newPrice = 1;

    // public event EventHandler<double> prevPriceChanged;

    public Dynamic.MarketValue marketValue;
    public DynamicBehaviour dynamicBehaviour;

    MarketNode market;

    // Start is called befoWre the first frame update
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

        // gameManager.TurnIncreased += Step;
        // gameManager.TurnIncreasedPost += StepPost;

        dynamicBehaviour.d.stepEvent += (sender, args) => Sync();

        market = dynamicBehaviour.d.marketMap[marketValue];

        Sync();
    }

    double[] PriceLinspace(double left, double right)
    {
        var price = new double[samples];
        var r = right - left;
        var n = (double)samples;
        for (var i = 0; i < samples; i++)
            price[i] = left + r * (i / n);
        return price;
    }

    void Sync()
    {
        var rootPrice = market.FindRoot();
        var rootQuantity = market.Supply(rootPrice);
        // Debug.Log($"rootPrice={rootPrice}, rootQuantity={rootQuantity}");

        priceLabel.text = rootPrice.ToString("0.##");
        quantityLabel.text = rootQuantity.ToString("0.##"); // = Demand(rootPrice)

        priceLabel.transform.localPosition = new Vector3(priceLabel.transform.localPosition.x, (float)(rootPrice / priceLimY - 0.5), priceLabel.transform.localPosition.z);
        quantityLabel.transform.localPosition = new Vector3((float)(rootQuantity / quantityLimY - 0.5), quantityLabel.transform.localPosition.y, quantityLabel.transform.localPosition.z);

        rootVerticalLine.positionCount = 2;
        rootVerticalLine.SetPositions(new Vector3[] {
            new Vector3((float)(rootQuantity / quantityLimY), 0, 0),
            new Vector3((float)(rootQuantity / quantityLimY), 1, 0)
        });

        rootHorizontalLine.positionCount = 2;
        rootHorizontalLine.SetPositions(new Vector3[] {
            new Vector3(0, (float)(rootPrice / priceLimY), 0),
            new Vector3(1, (float)(rootPrice / priceLimY), 0)
        });

        var demandPrice = PriceLinspace(priceLimX, priceLimY);
        var quantityPrice = PriceLinspace(0, priceLimY);

        // var pr = priceLim.y - priceLim.x;
        // var qr = quantityLim.y - quantityLim.x;
        var demandPositions = demandPrice.Where(p => market.Demand(p) <= quantityLimY && market.Demand(p) >= quantityLimX).Select(p =>
            new Vector3((float)(market.Demand(p) / quantityLimY), (float)(p / priceLimY), 0)
        ).ToArray();
        var supplyPositions = quantityPrice.Where(p => market.Supply(p) <= quantityLimY && market.Supply(p) >= quantityLimX).Select(p =>
            new Vector3((float)(market.Supply(p) / quantityLimY), (float)(p / priceLimY), 0)
        ).ToArray();

        demandCurve.positionCount = demandPositions.Length;
        demandCurve.SetPositions(demandPositions);

        supplyCurve.positionCount = supplyPositions.Length;
        supplyCurve.SetPositions(supplyPositions);

        rightBottomLabel.text = quantityLimY.ToString();
        leftTopLabel.text = priceLimY.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

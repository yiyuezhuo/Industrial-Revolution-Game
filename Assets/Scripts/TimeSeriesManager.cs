using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSeriesManager : MonoBehaviour
{
    public TimeSeries supplyWorkshopFactoryTS;
    public TimeSeries standOfLivingTS;
    public TimeSeries priceTS;
    public TimeSeries productionOfMaterialTS;
    public TimeSeries productionOfTextile;

    TimeSeries[] timeseriesArr;

    public DynamicBehaviour dynamicBehaviour;
    Dynamic d;

    // Start is called before the first frame update
    void Start()
    {
        timeseriesArr = new TimeSeries[]
        {
            supplyWorkshopFactoryTS, standOfLivingTS, priceTS,
            productionOfMaterialTS, productionOfTextile
        };

        d = dynamicBehaviour.d;
        d.stepEvent += (sender, args) => Step();

        supplyWorkshopFactoryTS.data = new TimeSeries.Data[]
        {
            new TimeSeries.Data(){name="Workshop", color=Color.red},
            new TimeSeries.Data(){name="Factory", color=Color.blue}
        };

        standOfLivingTS.data = new TimeSeries.Data[]
        {
            new TimeSeries.Data(){name="Standard of Living", color=Color.red}
        };

        priceTS.data = new TimeSeries.Data[]
        {
            new TimeSeries.Data(){name="Cotton", color=Color.red},
            new TimeSeries.Data(){name="Workhour", color=Color.green},
            new TimeSeries.Data(){name="Textile", color=Color.blue}
        };

        productionOfMaterialTS.data = new TimeSeries.Data[]
        {
            new TimeSeries.Data(){name="Cotton", color=Color.red},
            new TimeSeries.Data(){name="Workhour", color=Color.blue},
        };

        productionOfTextile.data = new TimeSeries.Data[]
        {
            new TimeSeries.Data(){name="Textile", color=Color.red}
        };

        foreach (var timeseries in timeseriesArr)
            timeseries.Init();
    }

    void Step()
    {
        // Debug.Log("TimeSeriesManager step");

        supplyWorkshopFactoryTS.Add(new double[]{
            d.workshop.Supply(),
            d.factory.Supply()
        });

        standOfLivingTS.Add(new double[]{
            d.workhour.price / (d.textile.price + 1e-6),
        });

        priceTS.Add(new double[]{
            d.cotton.price,
            d.workhour.price,
            d.textile.price
        });

        productionOfMaterialTS.Add(new double[]{
            d.farm.Supply(),
            d.worker.Supply()
        });

        productionOfTextile.Add(new double[]{
            d.workshop.Supply() + d.factory.Supply(),
        });

        foreach (var timeseries in timeseriesArr)
            timeseries.Sync();
    }

    public void DropHalfLog()
    {
        foreach (var timeseries in timeseriesArr)
            timeseries.DropHalfLog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

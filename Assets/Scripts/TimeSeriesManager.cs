using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSeriesManager : MonoBehaviour
{
    public TimeSeries supplyWorkshopFactoryTS;
    public DynamicBehaviour dynamicBehaviour;
    Dynamic d;

    // Start is called before the first frame update
    void Start()
    {
        d = dynamicBehaviour.d;
        d.stepEvent += (sender, args) => Step();

        supplyWorkshopFactoryTS.data = new TimeSeries.Data[]
        {
            new TimeSeries.Data(){name="Workshop", color=Color.red},
            new TimeSeries.Data(){name="Factory", color=Color.blue}
        };
        supplyWorkshopFactoryTS.Init();
    }

    void Step()
    {
        Debug.Log("TimeSeriesManager step");

        supplyWorkshopFactoryTS.Add(new double[]{
            d.workshop.Supply(),
            d.factory.Supply()
        });
        supplyWorkshopFactoryTS.Sync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

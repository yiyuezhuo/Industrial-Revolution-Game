using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConsumer : TestSector
{
    public Market textileMarket;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        textileMarket.prevPriceChanged += (sender, value) => Sync();
    }

    void Sync()
    {
        RenderToUI(
            new Dictionary<string, float>() {
                { "Textile", TestCurve.DemandTextile(textileMarket.newPrice) }
            },
            null
        );
    }
}

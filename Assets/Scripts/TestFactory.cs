using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFactory : TestSector
{
    public Market cottonMarket;
    public Market textileMarket;

    protected override void Start()
    {
        base.Start();

        cottonMarket.prevPriceChanged += (sender, value) => Sync();
        textileMarket.prevPriceChanged += (sender, value) => Sync();
    }

    void Sync()
    {
        RenderToUI(
            new Dictionary<string, float>() {
                { "Cotton", TestCurve.DemandCottonFactory(cottonMarket.newPrice, textileMarket.newPrice) }
            },
            new Dictionary<string, float>() {
                { "Textile", TestCurve.SupplyTextileFactory(textileMarket.newPrice, cottonMarket.newPrice) }
            }
        );
    }
}

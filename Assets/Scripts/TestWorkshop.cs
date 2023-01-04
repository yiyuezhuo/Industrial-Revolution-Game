using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorkshop : TestSector
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
            new Dictionary<string, double>() {
                { "Cotton", TestCurve.DemandCottonWorkshop(cottonMarket.newPrice, textileMarket.newPrice) }
            },
            new Dictionary<string, double>() {
                { "Textile", TestCurve.SupplyTextileWorkshop(textileMarket.newPrice, cottonMarket.newPrice) }
            }
        );
    }

}

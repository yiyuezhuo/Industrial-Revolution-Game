using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestSector: MonoBehaviour
{
    protected Sector sector;

    protected virtual void Start()
    {
        sector = GetComponent<Sector>();
    }

    private string Render(string header, Dictionary<string, double> records)
    {
        var lines = new List<string>() { header };
        lines.AddRange(records.Select(KV => $"  {KV.Key}: {KV.Value.ToString("0.###")}"));
        return string.Join("\n", lines);
    }

    protected void RenderToUI(Dictionary<string, double> costRecords, Dictionary<string, double> produceRecords)
    {
        var lines = new List<string>();
        if (costRecords != null)
            lines.Add(Render("Cost", costRecords));
        if (produceRecords != null)
            lines.Add(Render("Produce", produceRecords));
        sector.inputField.text = string.Join("\n", lines);
    }
}

public class TestFarm : TestSector
{
    public Market cottonMarket;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        cottonMarket.prevPriceChanged += (sender, value) => Sync();
    }

    void Sync()
    {
        RenderToUI(
            null, 
            new Dictionary<string, double>() { 
                { "Cotton", TestCurve.SupplyCotton(cottonMarket.newPrice) } 
            }
        );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


public class Sector : MonoBehaviour
{
    public TMP_InputField inputField;

    public DynamicBehaviour dynamicBehaviour;
    public Dynamic.ProducerValue producerValue;
    ProducerNode producerNode;

    // Start is called before the first frame update
    void Start()
    {
        producerNode = dynamicBehaviour.d.producerMap[producerValue];
        dynamicBehaviour.d.stepEvent += (sender, args) => Sync();

        Sync();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        inputField.text = string.Join("\n", lines);
    }

    void Sync()
    {
        // if(producerNode.inputMarkets.Length > 0)
        var costRecords = producerNode.inputMarkets.Length == 0 ? null : producerNode.inputMarkets.ToDictionary(
            marketNode => dynamicBehaviour.d.GetName(marketNode), marketNode => producerNode.Demand(marketNode)
        );
        var produceRecords = producerNode.outputMarket == null ? null : new Dictionary<string, double>()
        {
            {dynamicBehaviour.d.GetName(producerNode.outputMarket), producerNode.Supply()}
        };
        RenderToUI(costRecords, produceRecords);
    }
}

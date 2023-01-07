using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

using MathNet.Numerics.RootFinding;


public class Producer
{
    public double supplyThreshold;
    public double supplyPower;
    public double supplyScale = 1;
    public double[] costCoef;

    public double SupplySource(double outputPrice) // source
    {
        return supplyScale * Math.Pow(Math.Max(outputPrice - supplyThreshold, 0), supplyPower);
    }

    public virtual double Supply(double outputPrice, IEnumerable<double> inputPrices)
    {
        double cost = 0;
        int idx = 0;
        foreach (var inputPrice in inputPrices)
        {
            cost += costCoef[idx] * inputPrice;
            idx += 1;
        }
        return SupplySource(outputPrice - cost);
    }

    public double DemandForSupply(int idx, double supplyQuantity)
    {
        return costCoef[idx] * supplyQuantity;
    }

    public double Demand(int idx, double outputPrice, IEnumerable<double> inputPrices)
    {
        var supply = Supply(outputPrice, inputPrices);
        return costCoef[idx] * supply;
    }
}

public class DopamineProducer : Producer
{
    public double textileDemandCoef = 1;

    public override double Supply(double outputPrice, IEnumerable<double> inputPrices)
    {
        double cost = 0;
        int idx = 0;
        foreach (var inputPrice in inputPrices)
        {
            cost += costCoef[idx] * inputPrice;
            idx += 1;
        }
        return textileDemandCoef / cost;
    }
}

public class ProducerNode
{
    public Producer producer;
    public MarketNode[] inputMarkets;
    public MarketNode outputMarket;

    public double Supply(MarketNode marketSubstituted, double priceSubstituted)
    {
        if (marketSubstituted == outputMarket)
        {
            return producer.Supply(priceSubstituted, inputMarkets.Select(market => market.price));
        }
        // Debug.Log($"producer={producer}, outputMarket={outputMarket}, inputMarkets={inputMarkets}");
        double outputPrice = outputMarket == null ? 0 : outputMarket.price; // TODO: temp hack for "terminator" nodes.
        return producer.Supply(outputPrice, inputMarkets.Select(market => market == marketSubstituted ? priceSubstituted : market.price));
    }

    public double Supply() => producer.Supply(outputMarket == null ? 0 : outputMarket.price, inputMarkets.Select(market => market.price));

    public double Demand(MarketNode marketQuery, MarketNode marketSubstituted, double priceSubstituted)
    {
        var supply = Supply(marketSubstituted, priceSubstituted);
        return producer.DemandForSupply(Array.IndexOf(inputMarkets, marketQuery), supply);
    }

    public double Demand(MarketNode marketQuery)
    {
        var supply = Supply();
        return producer.DemandForSupply(Array.IndexOf(inputMarkets, marketQuery), supply);
    }
}

public class MarketNode
{
    public double price;
    public double newPrice;
    public ProducerNode[] inputProducers;
    public ProducerNode[] outputProducers;

    public double Supply(double testPrice)
    {
        return inputProducers.Sum(producerNode => producerNode.Supply(this, testPrice));
    }

    public double Demand(double testPrice)
    {
        return outputProducers.Sum(ProducerNode => ProducerNode.Demand(this, this, testPrice));
    }

    public double Objective(double testPrice) => Supply(testPrice) - Demand(testPrice);

    public void Step()
    {
        newPrice = FindRoot();
    }

    public double FindRoot()
    {
        return Secant.FindRoot(Objective, 1, 2); 
    }

    public void PostStep(double damping)
    {
        price = damping * price + (1 - damping) * newPrice;
    }
}
public class Dynamic
{
    public MarketNode cotton = new MarketNode() { price = 1 };
    public MarketNode workhour = new MarketNode() { price = 1 };
    public MarketNode textile = new MarketNode() { price = 3 };

    public ProducerNode farm = new ProducerNode()
    {
        producer = new Producer()
        {
            supplyThreshold = 0.2,
            supplyPower = 0.5
        }
    };

    public ProducerNode worker = new ProducerNode()
    {
        producer = new Producer()
        {
            supplyThreshold = 0.5,
            supplyPower = 0.9,
            supplyScale = 3
        }
    };

    public ProducerNode workshop = new ProducerNode()
    {
        producer = new Producer()
        {
            supplyThreshold = 0.2,
            supplyPower = 0.5,
            costCoef = new double[] { 1, 1 } // Cotton, Workhour
        }
    };

    public ProducerNode factory = new ProducerNode()
    {
        producer = new Producer()
        {
            supplyThreshold = 0,
            supplyPower = 0.7,
            costCoef = new double[] { 1, 0.75 }
        }
    };

    public ProducerNode consumer = new ProducerNode()
    {
        producer = new DopamineProducer()
        {
            textileDemandCoef = 1,
            costCoef = new double[] {1}
        }
    };


    public double solverDamping = 0.5;
    public int solverSteps = 1;

    public event EventHandler stepEvent;

    public void Init()
    {
        // Producer Configuration
        farm.inputMarkets = new MarketNode[0];
        farm.outputMarket = cotton;

        worker.inputMarkets = new MarketNode[0];
        worker.outputMarket = workhour;

        workshop.inputMarkets = new MarketNode[] { cotton, workhour };
        workshop.outputMarket = textile;

        factory.inputMarkets = new MarketNode[] { cotton, workhour };
        factory.outputMarket = textile;

        consumer.inputMarkets = new MarketNode[] { textile };
        consumer.outputMarket = null;

        // Market Configuration
        cotton.inputProducers = new ProducerNode[] { farm };
        cotton.outputProducers = new ProducerNode[] { workshop, factory };

        workhour.inputProducers = new ProducerNode[] { worker };
        workhour.outputProducers = new ProducerNode[] { workshop, factory };

        textile.inputProducers = new ProducerNode[] { workshop, factory };
        textile.outputProducers = new ProducerNode[] { consumer };

        marketMap = new Dictionary<MarketValue, MarketNode>()
        {
            {MarketValue.Cotton, cotton },
            {MarketValue.Workhour, workhour },
            {MarketValue.Textile, textile }
        };

        producerMap = new Dictionary<ProducerValue, ProducerNode>()
        {
            {ProducerValue.Farm, farm },
            {ProducerValue.Worker, worker },
            {ProducerValue.Workshop, workshop },
            {ProducerValue.Factory, factory },
            {ProducerValue.Consumer, consumer }
        };

        marketNameMap = marketMap.ToDictionary(KV => KV.Value, KV => KV.Key.ToString());
        producerNameMap = producerMap.ToDictionary(KV => KV.Value, KV => KV.Key.ToString());
    }

    public void Step()
    {
        var markets = new MarketNode[] { cotton, workhour, textile };
        for (var i = 0; i < solverSteps; i++)
        {
            foreach (var market in markets)
                market.Step();
            foreach (var market in markets)
                market.PostStep(solverDamping);
        }
        stepEvent?.Invoke(this, EventArgs.Empty);
    }

    public enum MarketValue
    {
        Cotton,
        Workhour,
        Textile
    }

    public enum ProducerValue
    {
        Farm,
        Worker,
        Workshop,
        Factory,
        Consumer
    }

    public Dictionary<MarketValue, MarketNode> marketMap;
    public Dictionary<ProducerValue, ProducerNode> producerMap;
    Dictionary<MarketNode, string> marketNameMap;
    Dictionary<ProducerNode, string> producerNameMap;

    public string GetName(MarketNode marketNode) => marketNameMap[marketNode];
    public string GetName(ProducerNode producerNode) => producerNameMap[producerNode];
}


public class DynamicBehaviour : MonoBehaviour
{
    public Dynamic d = new Dynamic();
    public GameManager gameManager;

    // Use this for initialization
    // void Start()
    void Awake()
    {
        d.Init();
    }

    private void Start()
    {
        gameManager.TurnIncreased += (sender, args) => d.Step();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

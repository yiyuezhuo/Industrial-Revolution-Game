using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Effector
{
    public Dynamic d;
    public int t;
    public int target = 0;
    public int range = 100;

    public int GetDt()
    {
        if (t == target)
            return 0;
        return target < t ? -1 : 1;
    }
    public void Step()
    {
        var dt = GetDt();
        var dp = (double)dt / range;
        t = t + dt;
        Effect(dp);
    }

    public abstract void Effect(double dp);

    public void OnChanged(bool value)
    {
        target = value ? range : 0;
    }
}

public class CottonPlantation : Effector
{
    public double farmSupplyScaleBase = 0;
    public double farmSupplyScaleLim = 0.5;

    public override void Effect(double dp)
    {
        d.farm.producer.supplyScale += dp * farmSupplyScaleLim;
    }
}

public class OverseaTrade: Effector
{
    double lim = 0.5;

    public override void Effect(double dp)
    {
        var producer = (DopamineProducer)d.consumer.producer; // TODO: temp workaroud due to time bugdet constraint.
        producer.demandCoef += dp * lim;
    }
}

public class Colonies: OverseaTrade
{ }

public class Enclosure: Effector
{
    double lim = 1.0;

    public override void Effect(double dp)
    {
        d.worker.producer.supplyScale += dp * lim;
    }
}

public class MassPopulation: Effector
{
    double textileDemandCoefLim = 0.5;
    double workerLim = 0.5;
    double farmLim = 0.25;

    public override void Effect(double dp)
    {
        var producer = (DopamineProducer)d.consumer.producer; // TODO: temp workaroud due to time bugdet constraint.
        producer.demandCoef += dp * textileDemandCoefLim;
        d.worker.producer.supplyScale += dp * workerLim;
        d.farm.producer.supplyScale += dp * farmLim;
    }
}

public class MassInvestment: Effector
{
    double workshopLim = 0.2;
    double factoryLim = 0.5;

    public override void Effect(double dp)
    {
        d.workshop.producer.supplyScale += dp * workshopLim;
        d.factory.producer.supplyScale += dp * factoryLim;
    }
}

public class FlyingShutter: Effector
{
    double limScale = 0.3;
    double limWorkhour = -0.2;

    public override void Effect(double dp)
    {
        d.workshop.producer.supplyScale += dp * limScale;
        d.workshop.producer.costCoef[1] += dp * limWorkhour;
        d.factory.producer.supplyScale += dp * limScale;
        d.factory.producer.costCoef[1] += dp * limWorkhour;
    }
}

public class SpinningJenny: Effector
{
    double limScale = 0.3;
    double limWorkhour = -0.2;

    public override void Effect(double dp)
    {
        d.workshop.producer.supplyScale += dp * limScale;
        d.workshop.producer.costCoef[1] += dp * limWorkhour;
        d.factory.producer.supplyScale += dp * limScale;
        d.factory.producer.costCoef[1] += dp * limWorkhour;
    }
}

public class WaterFrame: Effector
{
    double limScale = 0.3;
    double limWorkhour = -0.2;

    public override void Effect(double dp)
    {
        d.factory.producer.supplyScale += dp * limScale;
        d.factory.producer.costCoef[1] += dp * limWorkhour;
    }
}

public class SteamPoweredMill: Effector
{
    double limScale = 0.3;
    double limWorkhour = -0.2;

    public override void Effect(double dp)
    {
        d.factory.producer.supplyScale += dp * limScale;
        d.factory.producer.costCoef[1] += dp * limWorkhour;
    }
}

public class ConditionManager : MonoBehaviour
{
    public DynamicBehaviour dynamicBehaviour;
    Dynamic d { get => dynamicBehaviour.d; }

    public GameObject uiObject;
    public Transform toggleListTransform;
    public Dictionary<string, Effector> effectorMap = new Dictionary<string, Effector>()
    {
        {"Cotton Plantation",  new CottonPlantation()},
        {"Oversea Trade", new OverseaTrade() },
        {"Colonies", new Colonies() },
        {"Enclosure", new Enclosure() },
        {"Mass Population", new MassPopulation() },
        {"Mass Investment", new MassInvestment() },
        {"Flying Shuttle", new FlyingShutter()},
        {"Spinning Jenny", new SpinningJenny() },
        {"Water Frame", new WaterFrame() },
        {"Steam Powered Mill", new SteamPoweredMill() }
    };

    // Start is called before the first frame update
    void Start()
    {
        uiObject.SetActive(false);

        foreach(Transform t in toggleListTransform)
        {
            // t.GetComponent()
            var toggle = t.GetComponent<ConditionToggle>();
            toggle.toggleChanged += OnToggleChanged;
        }

        foreach(var effector in effectorMap.Values)
        {
            effector.d = d;
        }

        d.stepEvent += (sender, args) => Step();
    }

    void OnToggleChanged(object sender, Tuple<string, bool> args)
    {
        Debug.Log($"sender={sender}, args={args}");

        effectorMap[args.Item1].OnChanged(args.Item2);
    }

    public void Step()
    {
        foreach(var effector in effectorMap.Values)
        {
            effector.Step();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClose()
    {
        uiObject.SetActive(false);
    }

    public void OnOpen()
    {
        uiObject.SetActive(true);
    }

    public void OnToggleWindow()
    {
        uiObject.SetActive(!uiObject.activeSelf);
    }
}

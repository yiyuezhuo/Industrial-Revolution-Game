using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TimeSeries : MonoBehaviour
{
    public TMP_Text yLimText;
    public Transform lineContainer;
    public GameObject linePrefab;

    double yLim;

    public int samples = 100;

    public Data[] data;

    public class Data
    {
        public string name;
        public Color color;
        public List<double> data = new List<double>();
        public LineRenderer line;
    }

    public void Init()
    {
        foreach(Transform transform in lineContainer.transform)
        {
            Destroy(transform.gameObject);
        }

        foreach(var d in data)
        {
            // var lineObj = Instantiate(linePrefab, new Vector3(-0.5f, -0.5f, 0), Quaternion.identity, lineContainer);
            var lineObj = Instantiate(linePrefab, lineContainer);
            // lineObj.transform.localPosition = new Vector3(-0.5f, -0.5f, 0);
            d.line = lineObj.GetComponent<LineRenderer>();
            d.line.positionCount = samples;

            var gradient = new Gradient();
            // Just populate the same color keys
            var colorKey = new GradientColorKey[2];
            colorKey[0].color = d.color;
            colorKey[0].time = 0f;
            colorKey[1].color = d.color;
            colorKey[1].time = 1f;

            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 0.5f;
            alphaKey[0].time = 0f;
            alphaKey[1].alpha = 1f;
            alphaKey[1].time = 1f;

            gradient.SetKeys(colorKey, alphaKey);
            d.line.colorGradient = gradient;
        }
    }

    public void Sync()
    {
        if (data.Length == 0)
            return;

        var n = data[0].data.Count;
        var step = ((float)n) / samples;
        foreach(var d in data)
        {
            var positions = new Vector3[samples];
            for(var i = 0; i < samples; i++)
            {
                var idx = (int)System.Math.Floor(i * step);
                var x = (float)i / samples;
                var y = d.data[idx] / yLim;
                positions[i] = new Vector3(x, (float)y, 0);
            }
            d.line.SetPositions(positions);
        }
    }

    public void Add(double[] records)
    {
        for(var i=0;i < data.Length;i++)
        {
            var r = records[i];
            data[i].data.Add(records[i]);
            if (r > yLim)
                yLim = r;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

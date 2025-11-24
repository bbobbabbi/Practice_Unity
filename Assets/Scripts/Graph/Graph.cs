using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static FunctionLibrary;
using static UnityEngine.GraphicsBuffer;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField, Range(10,100)] private int resolution = 10;
    [SerializeField, Range(2, 20)] private float speed = 2f;
    [SerializeField] private Transform dotParent;
    [SerializeField, Range(0, 1)] private float sinPriquncy = 1f;
    [SerializeField] private FunctionLibrary.FunctionName sinGraph = FunctionName.None;
    [SerializeField] private FunctionLibrary.NewFunctionName newSinGraph = NewFunctionName.Sphere;
    Transform dot;
    Transform[] points;
    delegate void playGraph(float a);
    playGraph play;
    enum GraphType { one,two,three,ripple,spiral,rspiral};
    enum DotType{ multi,single};
    DotType dotType = DotType.single;
    GraphType graphType = GraphType.one;
    private void Awake()
    {
        //var scale = Vector3.one *(2f/resolution);
        //for (int i = 0; i < resolution; i++)
        //{
        //    float pos = (i + 0.5f) / 5f - (resolution/10);
        //    Transform point = Instantiate(pointPrefab);
        //    point.localPosition = new Vector3(pos,pos*pos, 0);
        //    point.localScale = scale;
        //}
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int j = 0;  j < points.Length; j++)
        {
            points[j] = Instantiate(pointPrefab);
            points[j].localScale = scale;
            points[j].SetParent(dotParent);
        }

        dot = Instantiate(pointPrefab);
        play = PlayGraph;

        //for (int i = 0; i < resolution; i++)
        //{
        //    point.SetParent(this.transform,false);
        //    position.x = (i + 0.5f) * step - 1f;
        //    position.y = position.x * position.x;
        //    point.localPosition = position;
        //    point.localScale = scale;
        //}


    }

    float f =0;   
    int trigger = 1;


    public void SwitchingGraph() {
        if (graphType != GraphType.rspiral)
        {
            graphType += 1;
        }
        else { 
            graphType = 0;
        }
    }
    public void ChangePlay() {
        if (dotType == DotType.multi)
        {
            dot.gameObject.SetActive(true);
            dotParent.gameObject.SetActive(false);  
            dotType = DotType.single;
            play = PlayGraph;
        }
        else
        {
            dot.gameObject.SetActive(false);
            dotParent.gameObject.SetActive(true);
            dotType = DotType.multi;
            play = SinGraph;
        }
    }

    void SinGraph(float t)
    {
        f += speed * Time.deltaTime/5f;
        FunctionLibrary.actionFunc fuc;
        FunctionLibrary.newActionFunc nfuc;
        float step = 2f / resolution;
        if (sinGraph != FunctionName.None)
        {
            fuc = FunctionLibrary.GetFunction(sinGraph);
            for (int j = 0, x = 0, z = 0; j < points.Length; j++, x++)
            {
                if (x == resolution)
                {
                    x = 0;
                    z += 1;
                }
                Transform jpoint = points[j];
                Vector3 position = jpoint.localPosition;
                position.x = (x + 0.5f) * step - 1f;
                position.z = ((z + 0.5f) * step - 1f);
                position.y = fuc(position.x, position.z, t, sinPriquncy);
                jpoint.localPosition = position;
            }
        }
        else if (newSinGraph != NewFunctionName.None) {
            nfuc = FunctionLibrary.GetFunction(newSinGraph);
            float v = 0.5f * step - 1f;
            for (int j = 0, x = 0, z = 0; j < points.Length; j++, x++)
            {
                if (x == resolution)
                {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }
                float u = (x + 0.5f) * step - 1f;
                points[j].localPosition = nfuc(u, v, t,sinPriquncy);
            }
        }
    }



    void PlayGraph(float t) {
        if (t < 0)
        {
            trigger = -trigger;
            f = 0;
        } else if (t > resolution) {
            trigger = -trigger;
            f = resolution;
        } 
        f += speed * trigger * Time.deltaTime;
        float step = 2f / resolution;
        var position = Vector3.zero;
        var scale = Vector3.one * step;
        dot.SetParent(this.transform, false);
        position.x = (t + 0.5f) * step - 1f;
        if (graphType == GraphType.one)
            position.y = position.x;
        else if (graphType == GraphType.two)
            position.y = position.x * position.x;
        else if (graphType == GraphType.three)
            position.y = position.x * position.x * position.x;
        else if (graphType == GraphType.ripple)
            position.y = FunctionLibrary.Ripple(position.x,t);
        else if (graphType == GraphType.spiral)
        {
            position = FunctionLibrary.spril(t, step);
        }
        else if (graphType == GraphType.rspiral)
        {
            position = FunctionLibrary.Rspril(t, step);
        }
        dot.localPosition = position;
        dot.localScale = scale;
    }

    void Update()
    {
        
        play.Invoke(f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField, Range(10,100)] private int resolution = 10;
    [SerializeField, Range(2, 20)] private float speed = 2f;
    [SerializeField] private Transform dotParent;

    Transform dot;
    Transform[] points;
    delegate void playGraph(float a);
    playGraph play;
    enum GraphType { one,two,three,spiral,rspiral};
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
        points = new Transform[resolution];
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int j = 0; j < points.Length; j++)
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
        for (int j = 0; j < points.Length; j++)
        {
            Transform jpoint = points[j]; 
            float step = 2f / resolution;
            Vector3 position = jpoint.localPosition;
            position.x = (j + 0.5f) * step - 1f;
            position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time));
            jpoint.localPosition = position;
        }
    }

    void PlayGraph(float t) {
        if (t < 0 || t > resolution)
        {
            trigger = -trigger;
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
        else if (graphType == GraphType.spiral)
        {  int deg = 30;
            float temp = (t + 0.5f) * step - 1f;
            if (temp<0) {
                temp = -temp;
            }
            float theta = Mathf.Lerp(0f, deg, temp);   // 0..maxTheta
            float r = 0.01F + 0.05f * theta;
            position.x = r * Mathf.Cos(theta);
            position.z = r * Mathf.Sin(theta);
            position.y = (t + 0.5f) * step - 1f;
        }
        else if (graphType == GraphType.rspiral)
        {
            int deg = 30;
            float temp = (t + 0.5f) * step - 1f;
            if (temp < 0)
            {
                temp = -temp;
            }
            float theta = Mathf.Lerp(0f, deg, temp);   // 0..maxTheta
            float r = 0.01F + 0.04f * theta;
            position.x = (t + 0.5f) * step - 1f;
            position.z = r * Mathf.Sin(theta);
            position.y = r * Mathf.Cos(theta);
        }
        dot.localPosition = position;
        dot.localScale = scale;
    }

    void Update()
    {
        
        play.Invoke(f);
    }

}

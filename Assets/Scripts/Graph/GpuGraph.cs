using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static FunctionLibrary;
using static UnityEngine.GraphicsBuffer;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Threading;
using System;
using static Unity.VisualScripting.Member;
using System.Timers;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GpuGraph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField, Range(10, 10000)] private int resolution ;
    [SerializeField, Range(2, 20)] private float speed = 2f;
    [SerializeField] private Transform dotParent;
    [SerializeField, Range(0, 1)] private float sinPriquncy = 1f;
    [SerializeField] private FunctionLibrary.FunctionName sinGraph = FunctionName.None;
    [SerializeField] private FunctionLibrary.NewFunctionName newSinGraph = NewFunctionName.Sphere;
    [SerializeField, Min(0)] private float functionDuration = 1f, transitionDuration = 1f;
    [SerializeField] private TransitionMode transitionMode = TransitionMode.Cycle;
    [SerializeField]
    ComputeShader computeShader;
    [SerializeField]
    Material material;
    [SerializeField]
    Mesh mesh;

    static readonly int
    positionsId = Shader.PropertyToID("_Positions"),
    resolutionId = Shader.PropertyToID("_Resolution"),
    stepId = Shader.PropertyToID("_Step"),
    timeId = Shader.PropertyToID("_Time");


    float duration;
    Transform dot;

    delegate void playGraph(float a);
    playGraph play;
    enum GraphType { one, two, three, ripple, spiral, rspiral };
    enum DotType { multi, single };
    DotType dotType = DotType.single;
    GraphType graphType = GraphType.one;
    private CancellationTokenSource source;

    bool transitioning;
    FunctionLibrary.NewFunctionName transitionFunction;
    ComputeBuffer positionsBuffer;
    Bounds bounds;
    //Vector3[] debugData;



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
        //debugData = new Vector3[resolution * resolution];

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

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution , 3*4);
        bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
    }

    private void OnDisable()
    {
        positionsBuffer?.Release();
        positionsBuffer = null;
    }

    float f = 0;
    int trigger = 1;

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);
        //positionsBuffer.GetData(debugData);
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        Graphics.DrawMeshInstancedProcedural(
            mesh, 0, material, bounds, positionsBuffer.count
        );
    }


    public void SwitchingGraph()
    {
        if (source != null && !source.IsCancellationRequested)
        {
            ForcedSetMorphPosition();
        }
        else
        {
            if (graphType != GraphType.rspiral)
            {
                graphType += 1;
            }
            else
            {
                graphType = 0;
            }
        }
    }
    public void ChangePlay()
    {
        if (dotType == DotType.multi)
        {
            dot.gameObject.SetActive(true);
            dotParent.gameObject.SetActive(false);
            dotType = DotType.single;
            //토큰 취소
            source?.Cancel();
            source?.Dispose();
            source = null;
            play = PlayGraph;
        }
        else
        {
            dot.gameObject.SetActive(false);
            dotParent.gameObject.SetActive(true);
            dotType = DotType.multi;
            source = StartRotate();
            USFRotateFunction().Forget();
            play = SinGraph;
        }
    }


    CancellationTokenSource StartRotate()
    {
        source?.Cancel();
        source?.Dispose();

        return new CancellationTokenSource();
    }

    private async UniTask USRotateFunction()
    {
        while (!source.Token.IsCancellationRequested)
        {

            await UniTask.Delay(TimeSpan.FromSeconds(functionDuration), cancellationToken: source.Token);
            if (transitionMode == TransitionMode.Cycle)
            {
                newSinGraph = FunctionLibrary.GetNextFunctionName(newSinGraph);
            }
            else
            {
                newSinGraph = FunctionLibrary.GetRandomFunctionName(newSinGraph);
            }
        }
    }
    private async UniTask USFRotateFunction()
    {
        while (!source.Token.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, source.Token);
            duration += Time.deltaTime;
            if (transitioning)
            {
                if (duration >= transitionDuration)
                {
                    duration -= transitionDuration;
                    transitioning = false;
                }
            }
            else
            {
                if (duration >= functionDuration)
                {
                    ForcedSetMorphPosition();
                }
            }
        }
    }

    private void ForcedSetMorphPosition()
    {
        duration = 0;
        transitioning = true;
        transitionFunction = newSinGraph;
        if (transitionMode == TransitionMode.Cycle)
        {
            newSinGraph = FunctionLibrary.GetNextFunctionName(newSinGraph);
        }
        else
        {
            newSinGraph = FunctionLibrary.GetRandomFunctionName(newSinGraph);
        }
    }

    void SinGraph(float t)
    {
        UpdateFunctionOnGPU();
        //f += speed * Time.deltaTime / 5f;
        //FunctionLibrary.actionFunc fuc;
        //FunctionLibrary.newActionFunc fnfuc, tnfunc, nnfunc;
        //float step = 2f / resolution;
        //if (sinGraph != FunctionName.None)
        //{
        //    fuc = FunctionLibrary.GetFunction(sinGraph);
        //    for (int j = 0, x = 0, z = 0; j < points.Length; j++, x++)
        //    {
        //        if (x == resolution)
        //        {
        //            x = 0;
        //            z += 1;
        //        }
        //        Transform jpoint = points[j];
        //        Vector3 position = jpoint.localPosition;
        //        position.x = (x + 0.5f) * step - 1f;
        //        position.z = ((z + 0.5f) * step - 1f);
        //        position.y = fuc(position.x, position.z, t, sinPriquncy);
        //        jpoint.localPosition = position;
        //    }
        //}
        //else if (newSinGraph != NewFunctionName.None && transitioning)
        //{
        //    fnfuc = FunctionLibrary.GetFunction(transitionFunction);
        //    tnfunc = FunctionLibrary.GetFunction(newSinGraph);
        //    float progress = duration / transitionDuration;
        //    float v = 0.5f * step - 1f;
        //    for (int j = 0, x = 0, z = 0; j < points.Length; j++, x++)
        //    {
        //        if (x == resolution)
        //        {
        //            x = 0;
        //            z += 1;
        //            v = (z + 0.5f) * step - 1f;
        //        }
        //        float u = (x + 0.5f) * step - 1f;
        //        points[j].localPosition = FunctionLibrary.Morph(u, v, t, sinPriquncy, fnfuc, tnfunc, progress);
        //    }
        //}
        //else if (newSinGraph != NewFunctionName.None && !transitioning)
        //{
        //    nnfunc = FunctionLibrary.GetFunction(newSinGraph);
        //    float progress = f / transitionDuration;
        //    float v = 0.5f * step - 1f;
        //    for (int j = 0, x = 0, z = 0; j < points.Length; j++, x++)
        //    {
        //        if (x == resolution)
        //        {
        //            x = 0;
        //            z += 1;
        //            v = (z + 0.5f) * step - 1f;
        //        }
        //        float u = (x + 0.5f) * step - 1f;
        //        points[j].localPosition = nnfunc(u, v, t, sinPriquncy);
        //    }
        //}
    }



    void PlayGraph(float t)
    {
        if (t < 0)
        {
            trigger = -trigger;
            f = 0;
        }
        else if (t > resolution)
        {
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
            position.y = FunctionLibrary.Ripple(position.x, t);
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

    private void OnDestroy()
    {
        source?.Dispose();
    }
}

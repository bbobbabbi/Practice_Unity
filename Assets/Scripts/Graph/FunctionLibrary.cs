using UnityEngine;

using static UnityEngine.Mathf;
public static class  FunctionLibrary
{

    public delegate float actionFunc(float u, float v,float t,float m);
    public delegate Vector3 newActionFunc(float u, float v,float t, float m);
    public enum FunctionName {  Wave, MultiWave, Ripple, NRipple, None  }
    public enum NewFunctionName { NewWave, NewMultiWave, NewRipple, Sphere , Torus, None  }
    static actionFunc[] functions = { Wave, MultiWave, Ripple, NRipple };
    static newActionFunc[] newFunctions = { NewWave, NewMultiWave, NewRipple, Sphere, Torus };
    
    public static actionFunc GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }
    public static newActionFunc GetFunction(NewFunctionName name)
    {
        return newFunctions[(int)name];
    }

    public static float Wave(float x, float z, float t, float m) {
        return Sin(PI*(m*x+z+t));
    }
    public static Vector3 NewWave(float u, float v, float t, float m) {
        Vector3 p = new Vector3();
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;

        return p;
    }

    public static float MultiWave(float x, float z, float t ,float m) {
        float y = Sin(PI * (x + m *t));
        y += Sin(2f * PI * (z + t)) * 0.5f;
        y += Sin(PI * (x + z + 0.25f * t));
        y = y * (1f / 2.5f);
        return y;
    }
    public static Vector3 NewMultiWave(float u, float v, float t, float m)
    {
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + 0.5f * t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= 1f / 2.5f;
        p.z = v;
        return p;
    }

    public static float Ripple(float x, float z, float t, float m)
    {
        float d = Abs(x);
        float y = Sin(PI * (4f*m * d - t));
        return y / (1f + 10f * d);
    }
    public static Vector3 NewRipple(float u, float v, float t, float m)
    {
        float d = Sqrt(u * u + v * v);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }
    public static float NRipple(float x, float z, float t, float m)
    {
        float d = Sqrt(x * x + z * z);
        float y = Sin(PI * (4f * m * d - t));
        return y / (1f + 10f * d);
    }
    public static float Ripple(float x, float t)
    {
        float d = Abs(x);
        float y = Sin(PI * (4f * d - t));
        return y / (1f + 10f * d);
    }


    public static Vector3 Sphere(float u, float v, float t, float m)
    {
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(m * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    public static Vector3 Torus(float u, float v, float t,float m)
    {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(m*PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }
    public static Vector3 spril(float t, float step) {
        int deg = 30;
        float temp = (t + 0.5f) * step - 1f;
        if (temp < 0)
        {
            temp = -temp;
        }
        float theta = Lerp(0f, deg, temp);   // 0..maxTheta
        float r = 0.01F + 0.05f * theta;
        float x = r * Cos(theta);
        float z = r * Sin(theta);
        float y = (t + 0.5f) * step - 1f;
        return new Vector3(x, y, z);
    }
    public static Vector3 Rspril(float t, float step)
    {
        Vector3 rValue = spril(t, step);
        rValue = new Vector3(rValue.y, rValue.x, rValue.z);
        return rValue;
    }
}

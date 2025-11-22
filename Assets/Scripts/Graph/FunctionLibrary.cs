using UnityEngine;

using static UnityEngine.Mathf;
public static class  FunctionLibrary
{

    public delegate float actionFunc(float x, float z,float t,float m);
    public enum FunctionName { Wave, MultiWave, Ripple, NewRipple }
    static actionFunc[] functions = { Wave, MultiWave, Ripple, NewRipple };

    public static actionFunc GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }
    public static float Wave(float x, float z, float t, float m) {
        return Sin(PI*(m*x+z+t));
    }
    public static float MultiWave(float x, float z, float t ,float m) {
        float y = Sin(PI * (x + m *t));
        y += Sin(2f * PI * (z + t)) * 0.5f;
        y += Sin(PI * (x + z + 0.25f * t));
        y = y * (1f / 2.5f);
        return y;
    }

    public static float Ripple(float x, float z, float t, float m)
    {
        float d = Abs(x);
        float y = Sin(PI * (4f*m * d - t));
        return y / (1f + 10f * d);
    }
    public static float NewRipple(float x, float z, float t, float m)
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

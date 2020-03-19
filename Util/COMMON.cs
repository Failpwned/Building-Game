using UnityEngine;

public static class COMMON
{
    public static class TAG
    {
        public const string SOLID_3D = "Solid3D";
        public const string UI_CANVAS = "UI Canvas";
        public const string UI_BUTTON = "UI Button";
    }

    public static class MATHFUNCTIONS
    {
        public static float SquaredSmooth(float startValue, float endValue, float time)
        {
            if (time < 0.5f)
            {
                return (Mathf.Pow(time, 2) * (endValue - startValue) * 2) + startValue;
            }
            else if(time >= 0.5f && time < 1)
            {
                return (-Mathf.Pow(time - 1, 2) * (endValue - startValue) * 2) + endValue;
            }
            else
            {
                return endValue;
            }
        }
    }
}

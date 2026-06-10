using System;

namespace BLL.Utils
{
    public static class StatHelper
    {
        public static int ToScaled(int rawValue, int scale)
        {
            return rawValue * scale;
        }

        public static int ToScaledFromFloat(float value, int scale)
        {
            return (int)System.Math.Round(value * scale);
        }

        public static float FromScaled(int scaledValue, int scale)
        {
            return scaledValue / (float)scale;
        }
    }
}

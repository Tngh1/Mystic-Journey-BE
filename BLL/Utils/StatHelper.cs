using System;

namespace BLL.Utils
{
    // Initializes a new default instance of the StatHelper class.
    public static class StatHelper
    {
        // Multiply the raw stat value by the configured scale to convert it into the compact integer representation used by the game.
        public static int ToScaled(int rawValue, int scale)
        {
            return rawValue * scale;
        }

        // Multiply the floating-point stat by the configured scale, round the result, and return the compact integer representation.
        public static int ToScaledFromFloat(float value, int scale)
        {
            return (int)System.Math.Round(value * scale);
        }

        // Divide the compact integer stat by the configured scale to recover the gameplay floating-point value.
        public static float FromScaled(int scaledValue, int scale)
        {
            return scaledValue / (float)scale;
        }
    }
}

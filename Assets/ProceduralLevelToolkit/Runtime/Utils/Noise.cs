
using UnityEngine;

namespace ProceduralLevelToolkit.Runtime.Utils
{
    public static class Noise
    {
        /// <summary>
        /// Return perlin noise.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public static float Perlin2D(float x, float y, float scale, float offsetX = 0f, float offsetY = 0f)
        {
            if (scale <= 0f) scale = 0.0001f;
            return Mathf.PerlinNoise(x / scale + offsetX, y / scale + offsetY);
        }
    }
}

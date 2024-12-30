using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NoiseMapLibrary
{
    public struct NoiseMaps
    {
        public static float CamouflageNoise(float x, float y, float centralizationPower = 0)
        {
            Vector2[] verticiesPosition = new Vector2[4];
            float[] verticiesPower = new float[4];
            float[] verticiesValue = new float[4];

            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    verticiesPosition[i * 2 + j] = new Vector2(i == 1 ? Mathf.Ceil(x) : Mathf.Floor(x),
                                                               j == 1 ? Mathf.Ceil(y) : Mathf.Floor(y));

                    verticiesPower[i * 2 + j] = NoiseMap(i == 1 ? Mathf.CeilToInt(x) : Mathf.FloorToInt(x),
                                                         j == 1 ? Mathf.CeilToInt(y) : Mathf.FloorToInt(y));

                    verticiesValue[i * 2 + j] = NoiseMap(i == 1 ? Mathf.CeilToInt(x + 1) : Mathf.FloorToInt(x + 1),
                                                         j == 1 ? Mathf.CeilToInt(y + 1) : Mathf.FloorToInt(y + 1));
                }
            }

            verticiesPower = CompressToValue(verticiesPower, compressionPoint:0.5f, centralizationPower);

            int indexPowerestVertice = 0;
            for (int i = 0; i < 4; i++)
            {
                float distanceToVertice = Vector2.Distance(new Vector2(x, y), verticiesPosition[i]);
                float distanceToPowerestVertice = Vector2.Distance(new Vector2(x, y), verticiesPosition[indexPowerestVertice]);

                if (distanceToVertice * (1 - verticiesPower[i]) < distanceToPowerestVertice * (1 - verticiesPower[indexPowerestVertice]))
                    indexPowerestVertice = i;
            }

            return verticiesValue[indexPowerestVertice];
        }

        private static float[] CompressToValue(float[] values, float compressionPoint, float power)
        {
            float[] result = new float[values.Length];
            for(int i = result.Length - 1; i >= 0; i--)
            {
                result[i] = ((values[i] - compressionPoint) / (1 + power) + compressionPoint);
            }
            return result;
        }

        public static float NoiseMap(int x, int y)
        {
            uint hashX = (((uint)x + uint.MaxValue / 2) * 73856093);
            uint hashY = (((uint)y + uint.MaxValue / 2) * 87349663);

            uint finalHash = hashX ^ hashY;

            return (finalHash % 1000000) / 1000000f;
        }
    }
}

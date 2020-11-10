namespace UnityEngine.Tilemaps.ProceduralGeneration
{
    public class NoiseTileGenerator: MonoBehaviour
    {

        public NoiseSet[] noiseSets;
        private static NoiseSet[] staticNoiseSets;

        private void Start()
        {
            staticNoiseSets = noiseSets;
        }


        public static TileBase GetTile(int seed, float amplitude, float lacunarity, Vector3 position, int set)
        {
            TileBase tile = null;

            float noiseValue = Mathf.Clamp(Mathf.PerlinNoise(((float)position.x + seed) * amplitude / lacunarity, ((float)position.y + seed) * amplitude / lacunarity), 0f, 1f);

            if(staticNoiseSets != null)
            {
                for (int i = 0; i < staticNoiseSets[set].noiseLayers.Length; i++)
                {
                    if (noiseValue >= staticNoiseSets[set].noiseLayers[i].minRange && noiseValue <= staticNoiseSets[set].noiseLayers[i].maxRange)
                    {
                        tile = staticNoiseSets[set].noiseLayers[i].tile;
                        break;
                    }

                }
            }
           
            return tile;
        }
    }

    [System.Serializable]
    public struct NoiseLayer
    {
        public TileBase tile;

        [Range(0, 1)]
        public float minRange;
        [Range(0, 1)]
        public float maxRange;
    }
    [System.Serializable]
    public struct NoiseSet
    {
        public NoiseLayer[] noiseLayers;
    }
}




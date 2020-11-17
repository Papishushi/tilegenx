using Assets;

namespace UnityEngine.Tilemaps.ProceduralGeneration
{
    public class NoiseTileGenerator: MonoBehaviour
    {
        ///This structure prevents users from creating more than one instances of this component on the scene.
#if UNITY_EDITOR
        public static Component oldInstance = null;

        private void OnValidate()
        {
            if (oldInstance == null)
            {
                oldInstance = this;
            }
        }
        private void Reset()
        {
            if (oldInstance != null && oldInstance != this)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Component already exists on scene", "Do you want to replace it?", "Ok, replace it", "No, thanks!"))
                {
                    gameObject.AddComponent<DeleteComponent>().componentReference = oldInstance;
                    oldInstance = null;
                }
                else
                {
                    gameObject.AddComponent<DeleteComponent>().componentReference = this;
                }
            }
        }
#endif

        private static NoiseSet[] staticNoiseSets;
        public NoiseSet[] noiseSets;

        private void Awake()
        {
            if(staticNoiseSets == null)
            {
                staticNoiseSets = noiseSets;
            }
        }

        public static TileBase GetTile(int seed, float amplitude, float lacunarity, Vector3 position, int set)
        {
            TileBase tile = null;

            float noiseValue = Mathf.PerlinNoise(((float)position.x + seed) * amplitude / lacunarity, ((float)position.y + seed) * amplitude / lacunarity);

            if(staticNoiseSets != null)
            {
                for (int i = 0; i < staticNoiseSets[set].noiseLayers.Length; i++)
                {
                    if (noiseValue >= staticNoiseSets[set].noiseLayers[i].minRange && noiseValue <= staticNoiseSets[set].noiseLayers[i].maxRange)
                    {
                        tile = staticNoiseSets[set].noiseLayers[i].tile;
                        break;
                    }
                    else if(noiseValue < 0 || noiseValue > 1)
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
        public bool isWall;

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




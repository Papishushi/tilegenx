using Kutility.Serialization;

namespace UnityEngine.Tilemaps.tilegenX
{
    public class NoiseTileGenerator: Singleton<NoiseTileGenerator>
    {
        private static NoiseSet[] StaticNoiseSets;
        public NoiseSet[] noiseSets;

#if UNITY_EDITOR
        public override void OnValidate()
        {
            base.OnValidate();

            if (noiseSets != null)
            {
                for (int i = 0; i < noiseSets.Length; i++)
                {
                    noiseSets[i].SetDefaultNames();
                }
            }
        }
        public override void Reset()
        {
            base.Reset();
        }
#endif

        private void Awake()
        {
            if(StaticNoiseSets == null)
            {
                StaticNoiseSets = noiseSets;
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public static TileBase GetTile(int seed, float amplitude, float lacunarity, Vector3 position, int set)
        {
            TileBase tile = null;

            float noiseValue = Mathf.PerlinNoise(((float)position.x + seed) * amplitude / lacunarity, ((float)position.y + seed) * amplitude / lacunarity);

            if(StaticNoiseSets != null)
            {
                for (int i = 0; i < StaticNoiseSets[set].noiseLayers.Length; i++)
                {
                    if (noiseValue >= StaticNoiseSets[set].noiseLayers[i].minRange && noiseValue <= StaticNoiseSets[set].noiseLayers[i].maxRange)
                    {
                        tile = StaticNoiseSets[set].noiseLayers[i].tile;
                        break;
                    }
                    else if(noiseValue < 0 || noiseValue > 1)
                    {
                        tile = StaticNoiseSets[set].noiseLayers[i].tile;
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
        public string name;
        [Space(2)]
        public TileBase tile;
        [Space(1)]
        public bool isWall;

        [ConditionalHide("isWall" , true, true)]
        [Range(0, 1)]
        public float minRange;

        
        [ConditionalHide("isWall", true, true)]
        [Range(0, 1)]
        public float maxRange;
    }
    [System.Serializable]
    public struct NoiseSet
    {
        public string name; 

        [Space(2)]
        public NoiseLayer[] noiseLayers;

        public void SetDefaultNames()
        {
            for(int i = 0; i < noiseLayers.Length; i++)
            {
                if (noiseLayers[i].name == "" || noiseLayers[i].name == null)
                {
                    noiseLayers[i].name = "Tile Layer" + " " + i;
                }
            }
        }
    }
}
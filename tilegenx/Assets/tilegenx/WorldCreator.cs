using Kutility.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Tilemaps.tilegenX;

public class WorldCreator : Singleton<WorldCreator>
{
    private Generator generator = null;

    public Grid grid = null;

    public Tilemap tilemap = null;

    public Transform player;

    public int seed;
    public float amplitude;
    public float lacunarity;

    public Generator.TileLayerMode layerMode;

    [Space(10)]
    [Range(0, 50)]
    public int size;
    [Range(1, 100)]
    public int x;
    [Range(1, 100)]
    public int y;

    [Range(-50, 50)]
    public int offsetX;
    [Range(-50, 50)]
    public int offsetY;

    public Generator.CircularGridMode circularGridMode;
    //

    private Vector3Int lastPlayerCellPosition;

#if UNITY_EDITOR
    public override void OnValidate()
    {
        base.OnValidate();
    }
    public override void Reset()
    {
        base.Reset();
    }
#endif

    private void Awake()
    {
        lastPlayerCellPosition = new Vector3Int(Random.Range(int.MinValue, int.MaxValue), Random.Range(int.MinValue, int.MaxValue), Random.Range(int.MinValue, int.MaxValue));
        generator = new Generator();
    }

    public override void Update()
    {
        base.Update();

        if (PlayerCellPosition() != lastPlayerCellPosition)
        {
            switch (layerMode)
            {
                case Generator.TileLayerMode.Standard:

                    generator.GenerateGrid(x, y, PlayerCellPosition(), tilemap, seed, amplitude, lacunarity, 0);
                    break;

                case Generator.TileLayerMode.Cross:

                    generator.GenerateGrid(x, y, PlayerCellPosition(), size, offsetX, offsetY, tilemap, seed, amplitude, lacunarity, 0);
                    break;

                case Generator.TileLayerMode.Circular:

                    generator.GenerateGrid(PlayerCellPosition(), size, circularGridMode, tilemap, seed, amplitude, lacunarity, 0);
                    break;
                default:

                    break;
            }

            lastPlayerCellPosition = PlayerCellPosition();
        }
    }

    public Vector3Int PlayerCellPosition()
    {
        return grid.LocalToCell(player.transform.localPosition);
    }

}

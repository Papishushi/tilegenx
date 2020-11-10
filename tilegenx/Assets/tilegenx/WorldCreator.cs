using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Tilemaps.ProceduralGeneration;

public class WorldCreator : MonoBehaviour
{
    private Generator generator = null;

    public Grid grid = null;

    public Tilemap tilemap = null;

    public Transform player;

    public int seed;
    public float amplitude;
    public float lacunarity;
    private bool isAdditive = false;

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

    private void Awake()
    {
        lastPlayerCellPosition = new Vector3Int(Random.Range(int.MinValue, int.MaxValue), Random.Range(int.MinValue, int.MaxValue), Random.Range(int.MinValue, int.MaxValue));
        generator = new Generator();
    }

    private void Update()
    {
        if (PlayerCellPosition() != lastPlayerCellPosition)
        {
            switch (layerMode)
            {
                case Generator.TileLayerMode.Standard:

                    generator.GenerateGrid(x, y, PlayerCellPosition(), tilemap, seed, amplitude, lacunarity, 0, isAdditive);
                    break;

                case Generator.TileLayerMode.Cross:

                    generator.GenerateCrossGrid(x, y, PlayerCellPosition(), size, offsetX, offsetY, tilemap, seed, amplitude, lacunarity, 0, isAdditive);
                    break;

                case Generator.TileLayerMode.Circular:

                    generator.GenerateCicularGrid(PlayerCellPosition(), size, circularGridMode, tilemap, seed, amplitude, lacunarity, 0, isAdditive);
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

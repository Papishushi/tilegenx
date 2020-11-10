using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Tilemaps.ProceduralGeneration;

public class StructureCreator : MonoBehaviour
{
    private Generator generator = null;

    public Transform player;
    public Grid grid;
    public Tilemap tilemap;
    public Tilemap wallTilemap;

    public int seed;

    public float amplitude;
    public float lacunarity;


    public int tileSet;

    public bool isAdditive;

    public TileBase rightTopCornerTile = null;
    public TileBase rightBotCornerTile = null;
    public TileBase leftTopCornerTile = null;
    public TileBase leftBotCornerTile = null;

    public TileBase rightTopInnerCornerTile = null;
    public TileBase rightBotInnerCornerTile = null;
    public TileBase leftTopInnerCornerTile = null;
    public TileBase leftBotInnerCornerTile = null;

    public TileBase topBorderTile = null;
    public TileBase leftBorderTile = null;
    public TileBase rightBorderTile = null;
    public TileBase botBorderTile = null;

    private Vector3Int lastPlayerCellPosition;

    private int randomX = 0;
    private int randomY = 0;
    private Vector3Int randomCenterOnRange = Vector3Int.zero;
    private int randomSize = 0;
    private int randomOffsetX = 0;
    private int randomOffsetY = 0;

    private void Awake()
    {
        lastPlayerCellPosition = Vector3Int.zero;
        generator = new Generator();
    }

    private void Update()
    {

        if (PlayerCellPosition().magnitude > lastPlayerCellPosition.magnitude + 20)
        {
            randomX = Random.Range(5, 20);
            randomY = Random.Range(5, 20);
            randomCenterOnRange = PlayerCellPosition() + new Vector3Int(Random.Range(-20, 20), Random.Range(-20, 20), 0); 
            randomSize = Random.Range(1, 10);
            randomOffsetX = Random.Range(-5, 5);
            randomOffsetY = Random.Range(-5, 5);

            generator.GenerateCrossGrid(randomX,randomY,randomCenterOnRange,randomSize,randomOffsetX,randomOffsetY, tilemap, seed, amplitude, lacunarity, tileSet, isAdditive);

            lastPlayerCellPosition = PlayerCellPosition();
        }
        if (PlayerCellPosition() != lastPlayerCellPosition)
        {


            generator.FindLimits(tilemap);

            generator.GenerateLimits(wallTilemap, rightTopCornerTile, rightBotCornerTile, leftTopCornerTile, leftBotCornerTile,
                rightTopInnerCornerTile, rightBotInnerCornerTile, leftTopInnerCornerTile, leftBotInnerCornerTile,
                topBorderTile, leftBorderTile, rightBorderTile, botBorderTile);

        }
    }
    public Vector3Int PlayerCellPosition()
    {
        return grid.LocalToCell(player.transform.localPosition);
    }


}

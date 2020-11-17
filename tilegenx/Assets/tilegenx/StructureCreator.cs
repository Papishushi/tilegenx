using Boo.Lang;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Tilemaps.ProceduralGeneration;

public class StructureCreator : MonoBehaviour
{
    public Transform player;
    public Grid grid;
    public Tilemap tilemap;
    public Tilemap wallTilemap;

    public int seed;

    public float amplitude;
    public float lacunarity;

    public int tileSet;

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
    private Vector3Int lastGenerationCellPosition;

    private int randomX = 0;
    private int randomY = 0;
    private Vector3Int randomCenterOnRange = Vector3Int.zero;
    private int randomSize = 0;
    private int randomOffsetX = 0;
    private int randomOffsetY = 0;

    private List<Generator> generators = new List<Generator>();

    private void Awake()
    {
        lastPlayerCellPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if (PlayerCellPosition() != lastPlayerCellPosition)
        {
            if (PlayerCellPosition().magnitude > lastGenerationCellPosition.magnitude + 100)
            {
                randomX = Random.Range(5, 20);
                randomY = Random.Range(5, 20);
                randomCenterOnRange = PlayerCellPosition() + new Vector3Int(Random.Range(-20, 20), Random.Range(-20, 20), 0);
                randomSize = Random.Range(1, 10);
                randomOffsetX = Random.Range(-5, 5);
                randomOffsetY = Random.Range(-5, 5);

                Generator generator = new Generator();

                generators.Add(generator);

                lastGenerationCellPosition = PlayerCellPosition();
            }

            foreach (Generator generator in generators)
            {
                generator.GenerateGrid(randomX, randomY, randomCenterOnRange, randomSize, randomOffsetX, randomOffsetY, tilemap, seed, amplitude, lacunarity, tileSet);


                generator.FindLimits(tilemap);

                generator.GenerateLimits(wallTilemap, rightTopCornerTile, rightBotCornerTile, leftTopCornerTile, leftBotCornerTile,
                    rightTopInnerCornerTile, rightBotInnerCornerTile, leftTopInnerCornerTile, leftBotInnerCornerTile,
                    topBorderTile, leftBorderTile, rightBorderTile, botBorderTile);
            }

            lastPlayerCellPosition = PlayerCellPosition();
        }
       

       

    }
    public Vector3Int PlayerCellPosition()
    {
        return grid.LocalToCell(player.transform.localPosition);
    }


}

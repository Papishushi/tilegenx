using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralGeneration
{
    public static class Generator
    {
        public static List<Vector3Int> ActiveTiles = new List<Vector3Int>();
        public static List<Vector3Int> StructureTiles = new List<Vector3Int>();
        public static List<Vector3Int> BoundaryTiles = new List<Vector3Int>();

        public static List<Vector3Int> RightTopCornerTiles = new List<Vector3Int>();
        public static List<Vector3Int> LeftTopCornerTiles = new List<Vector3Int>();
        public static List<Vector3Int> RightBotCornerTiles = new List<Vector3Int>();
        public static List<Vector3Int> LeftBotCornerTiles = new List<Vector3Int>();

        public static List<Vector3Int> RightTopInnerCornerTiles = new List<Vector3Int>();
        public static List<Vector3Int> LeftTopInnerCornerTiles = new List<Vector3Int>();
        public static List<Vector3Int> RightBotInnerCornerTiles = new List<Vector3Int>();
        public static List<Vector3Int> LeftBotInnerCornerTiles = new List<Vector3Int>();

        public static List<Vector3Int> TopBorderTiles = new List<Vector3Int>();
        public static List<Vector3Int> LeftBorderTiles = new List<Vector3Int>();
        public static List<Vector3Int> RightBorderTiles = new List<Vector3Int>();
        public static List<Vector3Int> BotBorderTiles = new List<Vector3Int>();

        public static Dictionary<Vector3Int, TileBase> GeneratedWorldDictionary = new Dictionary<Vector3Int, TileBase>();
        public static Dictionary<Vector3Int, TileBase> GeneratedStructureDictionary = new Dictionary<Vector3Int, TileBase>();

        public static Vector3Int leftBottomBoundaryCorner = new Vector3Int();
        public static Vector3Int leftUpperBoundaryCorner = new Vector3Int();

        public static Vector3Int rightBottomBoundaryCorner = new Vector3Int();
        public static Vector3Int rightUpperBoundaryCorner = new Vector3Int();

        public enum CircularGridMode
        {
            Standard,
            Squared
        };
        //
        public enum TileLayerMode
        {
            Standard,
            Cross,
            Circular
        };

        #region |·Grids and Boundaries·|

        ////////////////////
        // Standard Grids //
        ////////////////////

        /// <summary>
        /// Generate a dynamic and transformable rectangular grid.
        /// </summary>
        /// <param name="x">The size of the X axys of the boundary.</param>
        /// <param name="y">The size of the Y axys of the boundary.</param>
        /// <param name="center">The center of the generation.</param>
        /// <param name="tilemap">The Tilemap that will be used.</param>
        /// <param name="tile">The tile used for the generation representation.</param>
        public static void GenerateGrid(int x, int y, Vector3Int center, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive)
        {
            if (!isAdditive)
            {
                foreach (Vector3Int tilePosition in ActiveTiles)
                {
                    tilemap.SetTile(tilePosition, null);
                }

                ActiveTiles.Clear();
            } 

            for (int i = -x / 2; i <= x / 2; i++)
            {
                for (int j = -y / 2; j <= y / 2; j++)
                {
                    Vector3Int indexPosition = new Vector3Int(center.x + i, center.y + j, center.z);

                    if (GeneratedWorldDictionary.ContainsKey(indexPosition))
                    {
                        TileBase temp;
                        GeneratedWorldDictionary.TryGetValue(indexPosition, out temp);


                        tilemap.SetTile(indexPosition, temp);
                        if(!ActiveTiles.Contains(indexPosition))
                        {
                            ActiveTiles.Add(indexPosition);
                        }
                        
                    }
                    else
                    {
                        tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                        if (!ActiveTiles.Contains(indexPosition))
                        {
                            ActiveTiles.Add(indexPosition);
                        }
                        if (!GeneratedWorldDictionary.ContainsKey(indexPosition))
                        {
                            GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                        }
                    }

                }
            }
        }
        /// <summary>
        /// Generate the bounds of the rectangular grid.
        /// </summary>
        /// <param name="x">The size of the X axys of the boundary.</param>
        /// <param name="y">The size of the Y axys of the boundary.</param>
        /// <param name="center">The center of the generation.</param>
        /// <param name="tilemap">The Tilemap that will be used.</param>
        /// <param name="tileBorder">The tile used for the generation representation.</param>
        public static void TileBoundary(int x, int y, Vector3Int center, Tilemap tilemap, TileBase tileBorder)
        {
            leftBottomBoundaryCorner = new Vector3Int(center.x - x / 2, center.y - y / 2, center.z);
            rightBottomBoundaryCorner = new Vector3Int(center.x + x / 2, center.y - y / 2, center.z);

            leftUpperBoundaryCorner = new Vector3Int(center.x - x / 2, center.y + y / 2, center.z);
            rightUpperBoundaryCorner = new Vector3Int(center.x + x / 2, center.y + y / 2, center.z);

            foreach (Vector3Int tilePosition in BoundaryTiles)
            {
                tilemap.SetTile(tilePosition, null);
            }
            BoundaryTiles.Clear();

            for (int i = leftBottomBoundaryCorner.x; i <= rightBottomBoundaryCorner.x; i++)
            {
                BoundaryTiles.Add(new Vector3Int(center.x + i - center.x, center.y - y / 2, 0));

                BoundaryTiles.Add(new Vector3Int(center.x + i - center.x, center.y + y / 2, 0));
            }

            for (int i = leftBottomBoundaryCorner.y; i <= leftUpperBoundaryCorner.y; i++)
            {
                BoundaryTiles.Add(new Vector3Int(center.x - x / 2, center.y + i - center.y, 0));

                BoundaryTiles.Add(new Vector3Int(center.x + x / 2, center.y + i - center.y, 0));
            }

            foreach (Vector3Int tilePosition in BoundaryTiles)
            {
                tilemap.SetTile(tilePosition, tileBorder);
            }
        }

        /////////////////
        // Cross Grids //
        /////////////////

        /// <summary>
        /// Generate a dynamic and transformable cross-section grid.
        /// </summary>
        /// <param name="x">The size of the X axys of the boundary.</param>
        /// <param name="y">The size of the Y axys of the boundary.</param>
        /// <param name="center">The center of the generation.</param>
        /// <param name="size">The size of the cross. Larger values equal to thicker crosses.</param>
        /// <param name="offsetX">This is the offset of the cross arms parallel to the Y axys.</param>
        /// <param name="offsetY">This is the offset of the cross arms parallel to the X axys.</param>
        /// <param name="tilemap">The Tilemap that will be used.</param>
        /// <param name="tile">The tile used for the generation representation.</param>
        public static void GenerateCrossGrid(int x, int y, Vector3Int center, int size, int offsetX, int offsetY, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive)
        {
            if (!isAdditive)
            {
                foreach (Vector3Int tilePosition in ActiveTiles)
                {
                    tilemap.SetTile(tilePosition, null);
                }

                ActiveTiles.Clear();
            }

            for (int i = -x / 2; i <= x / 2; i++)
            {
                for (int j = -y / 2; j <= y / 2; j++)
                {
                    //Al sumar center a los indices los convierto en locales en toda la matríz
                    //Si el índice está entre la izquierda y la derecha del rango en x
                    if (i + center.x - offsetX >= center.x - size && i + center.x - offsetX <= center.x + size ||
                        //Si el índice está entre abajo y arriba del rango en y
                        j + center.y - offsetY >= center.y - size && j + center.y - offsetY <= center.y + size)
                    {
                        Vector3Int indexPosition = new Vector3Int(center.x + i, center.y + j, center.z);

                        if (GeneratedWorldDictionary.ContainsKey(indexPosition))
                        {
                            TileBase temp;
                            GeneratedWorldDictionary.TryGetValue(indexPosition, out temp);

                            tilemap.SetTile(indexPosition, temp);

                            if (!ActiveTiles.Contains(indexPosition))
                            {
                                ActiveTiles.Add(indexPosition);
                            }
                        }
                        else
                        {
                            tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                            if (!ActiveTiles.Contains(indexPosition))
                            {
                                ActiveTiles.Add(indexPosition);
                            }
                            if (!GeneratedWorldDictionary.ContainsKey(indexPosition))
                            {
                                GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                            }
                        }
                    }
                }
            }
        }
        //SEPARAR LAS ACCIONES DE TIEMPO REAL DE LAS QUE ESTÁN BASADAS EN LA DISTANCIA DESDE LA ÚLTIMA GENERACIÓN. TODO ESTO BASADO EN EL RANGO DE LAS ACTIVETILES
        public static void GenerateCrossGrid(int x, int y, Vector3Int center, int size, int offsetX, int offsetY, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive, bool isNewGeneration)
        {
            if (!isAdditive)
            {
                foreach (Vector3Int tilePosition in StructureTiles)
                {
                    tilemap.SetTile(tilePosition, null);
                }

                StructureTiles.Clear();
            }
            else
            {
                foreach (Vector3Int tilePosition in StructureTiles)
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

            if(isNewGeneration)
            {
                for (int i = -x / 2; i <= x / 2; i++)
                {
                    for (int j = -y / 2; j <= y / 2; j++)
                    {
                        //Al sumar center a los indices los convierto en locales en toda la matríz
                        //Si el índice está entre la izquierda y la derecha del rango en x
                        if (i + center.x - offsetX >= center.x - size && i + center.x - offsetX <= center.x + size ||
                            //Si el índice está entre abajo y arriba del rango en y
                            j + center.y - offsetY >= center.y - size && j + center.y - offsetY <= center.y + size)
                        {
                            Vector3Int indexPosition = new Vector3Int(center.x + i, center.y + j, center.z);

                            if (!GeneratedStructureDictionary.ContainsKey(indexPosition))
                            {
                                if (!StructureTiles.Contains(indexPosition))
                                {
                                    StructureTiles.Add(indexPosition);
                                }
                                GeneratedStructureDictionary.Add(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Vector3Int tilePosition in StructureTiles)
                {
                    if (ActiveTiles.Contains(tilePosition))
                    {
                        TileBase temp;
                        GeneratedStructureDictionary.TryGetValue(tilePosition, out temp);

                        tilemap.SetTile(tilePosition, temp);
                    }
                    else
                    {
                        tilemap.SetTile(tilePosition, null);
                    }

                }
            }
          

         


        }
        /// <summary>
        /// Generate the bounds of the cross-section grid.
        /// </summary>
        /// <param name="x">The size of the X axys of the boundary.</param>
        /// <param name="y">The size of the Y axys of the boundary.</param>
        /// <param name="center">The center of the generation.</param>
        /// <param name="size">The size of the cross. Larger values equal to thicker crosses.</param>
        /// <param name="offsetX">This is the offset of the cross arms parallel to the Y axys.</param>
        /// <param name="offsetY">This is the offset of the cross arms parallel to the X axys.</param>
        /// <param name="tilemap">The Tilemap that will be used.</param>
        /// <param name="tileBorder">The tile used for the generation representation.</param>
        public static void CrossTileBoundary(int x, int y, Vector3Int center, int size, int offsetX, int offsetY, Tilemap tilemap, TileBase tileBorder)
        {
            leftBottomBoundaryCorner = new Vector3Int(center.x - x / 2, center.y - y / 2, center.z);
            rightBottomBoundaryCorner = new Vector3Int(center.x + x / 2, center.y - y / 2, center.z);

            leftUpperBoundaryCorner = new Vector3Int(center.x - x / 2, center.y + y / 2, center.z);
            rightUpperBoundaryCorner = new Vector3Int(center.x + x / 2, center.y + y / 2, center.z);

            foreach (Vector3Int tilePosition in BoundaryTiles)
            {
                tilemap.SetTile(tilePosition, null);
            }
            BoundaryTiles.Clear();

            for (int i = leftBottomBoundaryCorner.x; i <= rightBottomBoundaryCorner.x; i++)
            {
                if (i + -offsetX >= center.x - size && i + -offsetX <= center.x + size)
                {
                    BoundaryTiles.Add(new Vector3Int(center.x + i - center.x, center.y - y / 2, 0));
                    BoundaryTiles.Add(new Vector3Int(center.x + i - center.x, center.y + y / 2, 0));
                }
            }

            for (int i = leftBottomBoundaryCorner.y; i <= leftUpperBoundaryCorner.y; i++)
            {
                if (i + -offsetY >= center.y - size && i + -offsetY <= center.y + size)
                {
                    BoundaryTiles.Add(new Vector3Int(center.x - x / 2, center.y + i - center.y, 0));
                    BoundaryTiles.Add(new Vector3Int(center.x + x / 2, center.y + i - center.y, 0));
                }
            }

            foreach (Vector3Int tilePosition in BoundaryTiles)
            {
                tilemap.SetTile(tilePosition, tileBorder);
            }
        }

        ////////////////////
        // Circular Grids //
        ////////////////////

        /// <summary>
        /// Generate a dynamic and transformable circular grid.
        /// </summary>
        /// <param name="center">The center of the generation.</param>
        /// <param name="size">The size of the cross. Larger values equal to thicker crosses.</param>
        /// <param name="circularGridMode">This is the mode in witch the generation will take place. Standard for normal generation, Squared for squared magnitude generation.</param>
        /// <param name="tilemap">The Tilemap that will be used.</param>
        /// <param name="tile">The tile used for the generation representation.</param>
        public static void GenerateCicularGrid(Vector3Int center, int size, CircularGridMode circularGridMode, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive)
        {
            int x = size * 2;
            int y = size * 2;

            if (!isAdditive)
            {
                foreach (Vector3Int tilePosition in ActiveTiles)
                {
                    tilemap.SetTile(tilePosition, null);
                }

                ActiveTiles.Clear();
            }

            for (int i = -x / 2; i <= x / 2; i++)
            {
                for (int j = -y / 2; j <= y / 2; j++)
                {
                    Vector3Int tempLocalMatrixPosition = new Vector3Int(i, j, 0);
                    switch (circularGridMode)
                    {
                        case CircularGridMode.Standard:
                            if (tempLocalMatrixPosition.magnitude < size)
                            {
                                Vector3Int indexPosition = new Vector3Int(center.x + i, center.y + j, center.z);

                                if (GeneratedWorldDictionary.ContainsKey(indexPosition))
                                {
                                    TileBase tempTile;
                                    GeneratedWorldDictionary.TryGetValue(indexPosition, out tempTile);


                                    tilemap.SetTile(indexPosition, tempTile);
                                    if (!ActiveTiles.Contains(indexPosition))
                                    {
                                        ActiveTiles.Add(indexPosition);
                                    }

                                }
                                else
                                {
                                    tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                                    if (!ActiveTiles.Contains(indexPosition))
                                    {
                                        ActiveTiles.Add(indexPosition);
                                    }
                                    if (!GeneratedWorldDictionary.ContainsKey(indexPosition))
                                    {
                                        GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                                    }
                                }
                            }
                            break;
                        case CircularGridMode.Squared:
                            if (tempLocalMatrixPosition.sqrMagnitude < size)
                            {
                                Vector3Int indexPosition = new Vector3Int(center.x + i, center.y + j, center.z);

                                if (GeneratedWorldDictionary.ContainsKey(indexPosition))
                                {
                                    TileBase tempTile;
                                    GeneratedWorldDictionary.TryGetValue(indexPosition, out tempTile);

                                    tilemap.SetTile(indexPosition, tempTile);
                                    if (!ActiveTiles.Contains(indexPosition))
                                    {
                                        ActiveTiles.Add(indexPosition);
                                    }

                                }
                                else
                                {

                                    tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                                    if (!ActiveTiles.Contains(indexPosition))
                                    {
                                        ActiveTiles.Add(indexPosition);
                                    }
                                    if (!GeneratedWorldDictionary.ContainsKey(indexPosition))
                                    {
                                        GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Generate the bounds of the circular grid.
        /// </summary>
        /// <param name="center">The center of the generation.</param>
        /// <param name="size">The size of the cross. Larger values equal to thicker crosses.</param>
        /// <param name="tilemap">The Tilemap that will be used.</param>
        /// <param name="tileBorder">The tile used for the generation representation.</param>
        public static void CircularTileBoundary(Vector3Int center, int size, Tilemap tilemap, TileBase tileBorder)
        {
            int x = size * 2;
            int y = size * 2;

            leftBottomBoundaryCorner = new Vector3Int(center.x - x / 2, center.y - y / 2, center.z);
            rightBottomBoundaryCorner = new Vector3Int(center.x + x / 2, center.y - y / 2, center.z);

            leftUpperBoundaryCorner = new Vector3Int(center.x - x / 2, center.y + y / 2, center.z);
            rightUpperBoundaryCorner = new Vector3Int(center.x + x / 2, center.y + y / 2, center.z);

            foreach (Vector3Int tilePosition in BoundaryTiles)
            {
                tilemap.SetTile(tilePosition, null);
            }
            BoundaryTiles.Clear();

            for (int i = leftBottomBoundaryCorner.x; i <= rightBottomBoundaryCorner.x; i++)
            {
                for (int j = leftBottomBoundaryCorner.y; j <= leftUpperBoundaryCorner.y; j++)
                {
                    Vector3Int tempPosition = new Vector3Int(i, j, 0);

                    if (tempPosition.magnitude >= size && tempPosition.magnitude - 1 < size)
                    {
                        BoundaryTiles.Add(tempPosition + center);
                    }
                }
            }

            foreach (Vector3Int tilePosition in BoundaryTiles)
            {
                tilemap.SetTile(tilePosition, tileBorder);
            }
        }
        #endregion

        #region |·Zones·|
        private static void FindCorners(Tilemap tilemap)
        {

            LeftBotCornerTiles.Clear();

            LeftTopCornerTiles.Clear();

            RightBotCornerTiles.Clear();

            RightTopCornerTiles.Clear();

            foreach (Vector3Int cell in StructureTiles)
            {
                if(tilemap.HasTile(cell))
                {
                    if (!tilemap.HasTile(cell - new Vector3Int(1, 0, 0)) && !tilemap.HasTile(cell - new Vector3Int(0, 1, 0)))
                    {
                        LeftBotCornerTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell - new Vector3Int(1, 0, 0)) && !tilemap.HasTile(cell + new Vector3Int(0, 1, 0)))
                    {
                        LeftTopCornerTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell + new Vector3Int(1, 0, 0)) && !tilemap.HasTile(cell - new Vector3Int(0, 1, 0)))
                    {
                        RightBotCornerTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell + new Vector3Int(1, 0, 0)) && !tilemap.HasTile(cell + new Vector3Int(0, 1, 0)))
                    {
                        RightTopCornerTiles.Add(cell);
                    }
                } 
            }

        }
        private static void GenerateCorners(Tilemap tilemap, TileBase rightTopCornerTile, TileBase rightBotCornerTile, TileBase leftTopCornerTile, TileBase leftBotCornerTile)
        {
            foreach (Vector3Int tilePosition in LeftBotCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftBotCornerTile);
                }
                else 
                {
                    tilemap.SetTile(tilePosition, null);
                }
               
            }
            foreach (Vector3Int tilePosition in LeftTopCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftTopCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in RightBotCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, rightBotCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in RightTopCornerTiles)
            {
                if(ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, rightTopCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

        }

        private static void FindInnerCorners(Tilemap tilemap)
        {

            LeftBotInnerCornerTiles.Clear();

            LeftTopInnerCornerTiles.Clear();

            RightBotInnerCornerTiles.Clear();

            RightTopInnerCornerTiles.Clear();

            foreach (Vector3Int cell in StructureTiles)
            {
                if(tilemap.HasTile(cell))
                {
                    if (!tilemap.HasTile(cell - new Vector3Int(1, 1, 0)) && tilemap.HasTile(cell - new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell - new Vector3Int(1, 0, 0)))
                    {
                        LeftBotInnerCornerTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell - new Vector3Int(1, -1, 0)) && tilemap.HasTile(cell + new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell - new Vector3Int(1, 0, 0)))
                    {
                        LeftTopInnerCornerTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell - new Vector3Int(-1, 1, 0)) && tilemap.HasTile(cell - new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell + new Vector3Int(1, 0, 0)))
                    {
                        RightBotInnerCornerTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell - new Vector3Int(-1, -1, 0)) && tilemap.HasTile(cell + new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell + new Vector3Int(1, 0, 0)))
                    {
                        RightTopInnerCornerTiles.Add(cell);
                    }
                }
               
            }

        }
        private static void GenerateInnerCorners(Tilemap tilemap, TileBase rightTopInnerCornerTile, TileBase rightBotInnerCornerTile, TileBase leftTopInnerCornerTile, TileBase leftBotInnerCornerTile)
        {
            foreach (Vector3Int tilePosition in LeftBotInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftBotInnerCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in LeftTopInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftTopInnerCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in RightBotInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, rightBotInnerCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in RightTopInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, rightTopInnerCornerTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

        }

        private static void FindBorders(Tilemap tilemap)
        {
            LeftBorderTiles.Clear();
            RightBorderTiles.Clear();
            BotBorderTiles.Clear();
            TopBorderTiles.Clear();

            foreach (Vector3Int cell in StructureTiles)
            {
                if (tilemap.HasTile(cell))
                {
                    if (!tilemap.HasTile(cell - new Vector3Int(1, 0, 0)) && tilemap.HasTile(cell - new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell + new Vector3Int(0, 1, 0)))
                    {
                        LeftBorderTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell + new Vector3Int(1, 0, 0)) && tilemap.HasTile(cell - new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell + new Vector3Int(0, 1, 0)))
                    {
                        RightBorderTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell - new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell - new Vector3Int(1, 0, 0)) && tilemap.HasTile(cell + new Vector3Int(1, 0, 0)))
                    {
                        BotBorderTiles.Add(cell);
                    }
                    else if (!tilemap.HasTile(cell + new Vector3Int(0, 1, 0)) && tilemap.HasTile(cell - new Vector3Int(1, 0, 0)) && tilemap.HasTile(cell + new Vector3Int(1, 0, 0)))
                    {
                        TopBorderTiles.Add(cell);
                    }
                }
            }

        }
        private static void GenerateBorders(Tilemap tilemap, TileBase topBorderTile, TileBase leftBorderTile, TileBase rightBorderTile, TileBase botBorderTile)
        {
            

            foreach (Vector3Int tilePosition in LeftBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftBorderTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in RightBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, rightBorderTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in BotBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, botBorderTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }
            foreach (Vector3Int tilePosition in TopBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, topBorderTile);
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

        }

        public static void FindLimits(Tilemap tilemap)
        {
            FindCorners(tilemap);
            FindInnerCorners(tilemap);
            FindBorders(tilemap);
        }

        public static void GenerateLimits(Tilemap tilemap, TileBase rightTopCornerTile, TileBase rightBotCornerTile, TileBase leftTopCornerTile, TileBase leftBotCornerTile, 
            TileBase rightTopInnerCornerTile, TileBase rightBotInnerCornerTile, TileBase leftTopInnerCornerTile, TileBase leftBotInnerCornerTile,
             TileBase topBorderTile, TileBase leftBorderTile, TileBase rightBorderTile, TileBase botBorderTile)
        {
            GenerateCorners(tilemap,rightTopCornerTile, rightBotCornerTile,leftTopCornerTile, leftBotCornerTile);
            GenerateInnerCorners(tilemap, rightTopInnerCornerTile, rightBotInnerCornerTile, leftTopInnerCornerTile, leftBotInnerCornerTile);
            GenerateBorders(tilemap, topBorderTile, leftBorderTile, rightBorderTile, botBorderTile);
        }
        #endregion
    }


}

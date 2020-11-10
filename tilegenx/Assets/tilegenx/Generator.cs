using System.Collections.Generic;

namespace UnityEngine.Tilemaps.ProceduralGeneration
{
    public class Generator 
    {
        public List<Vector3Int> ActiveTiles = new List<Vector3Int>();

        public List<Vector3Int> RightTopCornerTiles = new List<Vector3Int>();
        public List<Vector3Int> LeftTopCornerTiles = new List<Vector3Int>();
        public List<Vector3Int> RightBotCornerTiles = new List<Vector3Int>();
        public List<Vector3Int> LeftBotCornerTiles = new List<Vector3Int>();

        public List<Vector3Int> RightTopInnerCornerTiles = new List<Vector3Int>();
        public List<Vector3Int> LeftTopInnerCornerTiles = new List<Vector3Int>();
        public List<Vector3Int> RightBotInnerCornerTiles = new List<Vector3Int>();
        public List<Vector3Int> LeftBotInnerCornerTiles = new List<Vector3Int>();

        public List<Vector3Int> TopBorderTiles = new List<Vector3Int>();
        public List<Vector3Int> LeftBorderTiles = new List<Vector3Int>();
        public List<Vector3Int> RightBorderTiles = new List<Vector3Int>();
        public List<Vector3Int> BotBorderTiles = new List<Vector3Int>();

        private Dictionary<Vector3Int, TileBase> GeneratedWorldDictionary = new Dictionary<Vector3Int, TileBase>();


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
        public void GenerateGrid(int x, int y, Vector3Int center, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive)
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
        public  void GenerateCrossGrid(int x, int y, Vector3Int center, int size, int offsetX, int offsetY, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive)
        {

            foreach (Vector3Int tilePosition in ActiveTiles)
            {
                tilemap.SetTile(tilePosition, null);
            }
            if (!isAdditive)
            {
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
        public  void GenerateCicularGrid(Vector3Int center, int size, CircularGridMode circularGridMode, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set, bool isAdditive)
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
        #endregion

        #region |·Zones·|
        private void FindCorners(Tilemap tilemap)
        {

            LeftBotCornerTiles.Clear();
            LeftTopCornerTiles.Clear();
            RightBotCornerTiles.Clear();
            RightTopCornerTiles.Clear();

            foreach (Vector3Int cell in ActiveTiles)
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
                    else
                    {
                        TileBase temp;
                        GeneratedWorldDictionary.TryGetValue(cell, out temp);

                        tilemap.SetTile(cell, temp);


                        if (!ActiveTiles.Contains(cell))
                        {
                            ActiveTiles.Add(cell);
                        }
                    }
                } 
            }

        }
        private void GenerateCorners(Tilemap tilemap, TileBase rightTopCornerTile, TileBase rightBotCornerTile, TileBase leftTopCornerTile, TileBase leftBotCornerTile)
        {
            foreach (Vector3Int tilePosition in LeftBotCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftBotCornerTile);

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftBotCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftTopCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightBotCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightTopCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

        }

        private void FindInnerCorners(Tilemap tilemap)
        {

            LeftBotInnerCornerTiles.Clear();
            LeftTopInnerCornerTiles.Clear();
            RightBotInnerCornerTiles.Clear();
            RightTopInnerCornerTiles.Clear();

            foreach (Vector3Int cell in ActiveTiles)
            {
                if (tilemap.HasTile(cell))
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
                    else
                    {
                        TileBase temp;
                        GeneratedWorldDictionary.TryGetValue(cell, out temp);

                        tilemap.SetTile(cell, temp);


                        if (!ActiveTiles.Contains(cell))
                        {
                            ActiveTiles.Add(cell);
                        }
                    }
                }
               
            }

        }
        
        private void GenerateInnerCorners(Tilemap tilemap, TileBase rightTopInnerCornerTile, TileBase rightBotInnerCornerTile, TileBase leftTopInnerCornerTile, TileBase leftBotInnerCornerTile)
        {
            foreach (Vector3Int tilePosition in LeftBotInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftBotInnerCornerTile);

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if(temp != leftBotInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftTopInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }

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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightBotInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightTopInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

        }

        private void FindBorders(Tilemap tilemap)
        {
            LeftBorderTiles.Clear();
            RightBorderTiles.Clear();
            BotBorderTiles.Clear();
            TopBorderTiles.Clear();

            foreach (Vector3Int cell in ActiveTiles)
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
                    else
                    {
                        TileBase temp;
                        GeneratedWorldDictionary.TryGetValue(cell, out temp);

                        tilemap.SetTile(cell, temp);


                        if (!ActiveTiles.Contains(cell))
                        {
                            ActiveTiles.Add(cell);
                        }
                    }
                }
            }

        }
        private void GenerateBorders(Tilemap tilemap, TileBase topBorderTile, TileBase leftBorderTile, TileBase rightBorderTile, TileBase botBorderTile)
        {
            

            foreach (Vector3Int tilePosition in LeftBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    tilemap.SetTile(tilePosition, leftBorderTile);

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != botBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }
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

                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != topBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, tilemap.GetTile(tilePosition));
                    }

                }
                else
                {
                    tilemap.SetTile(tilePosition, null);
                }
            }

        }

        public void FindLimits(Tilemap tilemap)
        {
            FindCorners(tilemap);
            FindInnerCorners(tilemap);
            FindBorders(tilemap);
        }

        public void GenerateLimits(Tilemap tilemap, TileBase rightTopCornerTile, TileBase rightBotCornerTile, TileBase leftTopCornerTile, TileBase leftBotCornerTile, 
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

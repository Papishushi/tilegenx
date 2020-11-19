/**
  * @file Generator.cs
  * @author Daniel Molinero
  * @version 0.1.0
  * @section Copyright © <2020+> <Daniel Molinero>
  * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
  * to deal in the Software without restriction, including without limitation the rights to use, copy,
  * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
  * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
  *
  * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
  *
  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  **/

namespace UnityEngine.Tilemaps.tilegenX
{
    using System.Collections.Generic;

    public class Generator 
    {
        #region |·Class declaration·|

        public readonly List<Vector3Int> ActiveTiles = new List<Vector3Int>();

        private readonly List<Vector3Int> RightTopCornerTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> LeftTopCornerTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> RightBotCornerTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> LeftBotCornerTiles = new List<Vector3Int>();

        private readonly List<Vector3Int> RightTopInnerCornerTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> LeftTopInnerCornerTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> RightBotInnerCornerTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> LeftBotInnerCornerTiles = new List<Vector3Int>();

        private readonly List<Vector3Int> TopBorderTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> LeftBorderTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> RightBorderTiles = new List<Vector3Int>();
        private readonly List<Vector3Int> BotBorderTiles = new List<Vector3Int>();

        private readonly Dictionary<Vector3Int, TileBase> GeneratedWorldDictionary = new Dictionary<Vector3Int, TileBase>(); 

        public enum CircularGridMode
        {
            Standard,
            Squared
        };

        public enum TileLayerMode
        {
            Standard,
            Cross,
            Circular
        };
        #endregion

        #region |·Grids·|

        ////////////////////
        // Standard Grids //
        ////////////////////

        /// <summary>
        /// Generate a dynamic and transformable rectangular grid.
        /// </summary>
        /// <param name="x">The size of the X axys boundary.</param>
        /// <param name="y">The size of the Y axys boundary.</param>
        /// <param name="center">The center of the generation.</param>
        /// <param name="tilemap">The Tilemap that will be used for the generation.</param>
        /// <param name="seed">The seed of the generator. Larger values return less repetitive patterns, smaller ones return more fratal patterns.</param>
        /// <param name="amplitude"></param>
        /// <param name="lacunarity"></param>
        /// <param name="set">The set of tiles that will be used by the generator.</param>
        public void GenerateGrid(int x, int y, Vector3Int center, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set)
        {
            /// Cast the list to an array for optimization purposes.
             Vector3Int[] activeTilesToArray = ActiveTiles.ToArray();
            /// Everytime this method is called we clear from the tilemap all the tiles contained in ActiveTiles.
            for (int i = 0; i < activeTilesToArray.Length; i++)
            {
                tilemap.SetTile(activeTilesToArray[i], null);
            }
            /// Clear all the data from ActiveTiles.
            ActiveTiles.Clear();

            /// Instantiate a new reutilizable Vector3Int that will be assigned with local positions from the current index
            Vector3Int indexPosition;

            for (int i = -x / 2; i <= x / 2; i++)
            {
                for (int j = -y / 2; j <= y / 2; j++)
                {
                    /// Assign indexposition the local position of the current index
                    indexPosition = new Vector3Int(center.x + i, center.y + j, center.z);

                    /// Add the current indexPosition data if it is not within the GeneratedWorldDictionary
                    if (!GeneratedWorldDictionary.ContainsKey(indexPosition))
                    {
                        ///Use NoiseTileGenerator.GetTile() to get the current tilebase
                        GeneratedWorldDictionary.Add(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));
                    }

                    /// Get the value from the dictionary and assign it to its tile
                    GeneratedWorldDictionary.TryGetValue(indexPosition, out TileBase temp);
                    tilemap.SetTile(indexPosition, temp);

                    /// Add the current local index to ActiveTiles list
                    ActiveTiles.Add(indexPosition);
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
        public  void GenerateGrid(int x, int y, Vector3Int center, int size, int offsetX, int offsetY, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set)
        {
            /// Everytime this method is called we clear from the tilemap all the tiles contained in ActiveTiles.
            foreach (Vector3Int tilePosition in ActiveTiles)
            {
                tilemap.SetTile(tilePosition, null);
            }
            /// Clear all the data from ActiveTiles.
            ActiveTiles.Clear();

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
                        }
                        else
                        {
                            tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                            GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                        }

                        if (!ActiveTiles.Contains(indexPosition))
                        {
                            ActiveTiles.Add(indexPosition);
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
        public  void GenerateGrid(Vector3Int center, int size, CircularGridMode circularGridMode, Tilemap tilemap, int seed, float amplitude, float lacunarity, int set)
        {
            int x = size * 2;
            int y = size * 2;

            /// Everytime this method is called we clear from the tilemap all the tiles contained in ActiveTiles.
            foreach (Vector3Int tilePosition in ActiveTiles)
            {
                tilemap.SetTile(tilePosition, null);
            }
            /// Clear all the data from ActiveTiles.
            ActiveTiles.Clear();


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
                                }
                                else
                                {
                                    tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                                    GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                                }

                                if (!ActiveTiles.Contains(indexPosition))
                                {
                                    ActiveTiles.Add(indexPosition);
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
                                }
                                else
                                {
                                    tilemap.SetTile(indexPosition, NoiseTileGenerator.GetTile(seed, amplitude, lacunarity, tilemap.CellToWorld(indexPosition), set));

                                    GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));      
                                }

                                if (!ActiveTiles.Contains(indexPosition))
                                {
                                    ActiveTiles.Add(indexPosition);
                                }
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region |·Wall Zones·|
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
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftBotCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, leftBotCornerTile);
                    }
                }
              
            }
            foreach (Vector3Int tilePosition in LeftTopCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftTopCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, leftTopCornerTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in RightBotCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightBotCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, rightBotCornerTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in RightTopCornerTiles)
            {
                if(ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightTopCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, rightTopCornerTile);
                    }
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
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if(temp != leftBotInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, leftBotInnerCornerTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in LeftTopInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftTopInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, leftTopInnerCornerTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in RightBotInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightBotInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, rightBotInnerCornerTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in RightTopInnerCornerTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightTopInnerCornerTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, rightTopInnerCornerTile);
                    }
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
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != leftBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, leftBorderTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in RightBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != rightBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, rightBorderTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in BotBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != botBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, botBorderTile);
                    }
                }
            }
            foreach (Vector3Int tilePosition in TopBorderTiles)
            {
                if (ActiveTiles.Contains(tilePosition))
                {
                    TileBase temp;

                    GeneratedWorldDictionary.TryGetValue(tilePosition, out temp);

                    if (temp != topBorderTile)
                    {
                        GeneratedWorldDictionary.Remove(tilePosition);
                        GeneratedWorldDictionary.Add(tilePosition, topBorderTile);
                    }
                }
            }

        }

        #region |·Public Wall Zones·|
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
            GenerateCorners(tilemap, rightTopCornerTile, rightBotCornerTile, leftTopCornerTile, leftBotCornerTile);
            GenerateInnerCorners(tilemap, rightTopInnerCornerTile, rightBotInnerCornerTile, leftTopInnerCornerTile, leftBotInnerCornerTile);
            GenerateBorders(tilemap, topBorderTile, leftBorderTile, rightBorderTile, botBorderTile);
        }
        #endregion
        #endregion
    }
}

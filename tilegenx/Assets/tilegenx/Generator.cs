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

        public readonly Tilemap tilemap;

        public readonly Tilemap wallTilemap;

        public readonly DynamicTile dynamicTile;

        public readonly List<Vector3Int> ActiveTiles = new List<Vector3Int>();

        private readonly Dictionary<Vector3Int, TileBase> GeneratedWorldDictionary = new Dictionary<Vector3Int, TileBase>();

        private Vector3Int lastCenter = Vector3Int.zero;

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

        #region |·Initializators·|

        public Generator(Tilemap tilemap)
        {
            this.tilemap = tilemap;
        }
        public Generator (Tilemap tilemap, Tilemap wallTilemap, DynamicTile dynamicTile)
        {
            this.tilemap = tilemap;
            this.wallTilemap = wallTilemap;
            this.dynamicTile = dynamicTile;
        }

        #endregion

        #region |·Grids·|

        ////////////////////
        // Standard Grids //
        ////////////////////

        private Vector2Int UpdateDirection()
        {
            return Vector2Int.zero;
        }

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
        public void GenerateGrid(int x, int y, Vector3Int center, int seed, float amplitude, float lacunarity, int set)
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

            lastCenter = center;
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
        public  void GenerateGrid(int x, int y, Vector3Int center, int size, int offsetX, int offsetY, int seed, float amplitude, float lacunarity, int set)
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
                            tilemap.SetTile(indexPosition, dynamicTile);

                            GeneratedWorldDictionary.Add(indexPosition, tilemap.GetTile(indexPosition));
                        }

                        if (!ActiveTiles.Contains(indexPosition))
                        {
                            ActiveTiles.Add(indexPosition);
                        }

                    }
                }
            }

            lastCenter = center;
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
        public  void GenerateGrid(Vector3Int center, int size, CircularGridMode circularGridMode, int seed, float amplitude, float lacunarity, int set)
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

            lastCenter = center;

        }
        #endregion

        #region |·Wall Zones·|
        public void GenerateLimits()
        {
            foreach(Vector3Int position in ActiveTiles)
            {
                DynamicTile temp = (DynamicTile)tilemap.GetTile(position);

                if(temp != null && temp.isLimit)
                {
                    wallTilemap.SetTile(position, ScriptableObject.CreateInstance<Tile>());
                }
            }
        }
        #endregion
    }
}

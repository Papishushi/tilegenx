using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps.tilegenX
{
    [Serializable]
    public struct DynamicTileSprites
    {
        [HideInInspector]
        public string name;
        [Space(5)]
        public Sprite center;
        [Space(5)]
        public Sprite left;
        public Sprite bot;
        public Sprite top;
        public Sprite right;
        [Space(5)]
        public Sprite leftTop;
        public Sprite leftbot;
        public Sprite rightTop;
        public Sprite rightBot;
        [Space(5)]
        public Sprite topEnd;
        public Sprite botEnd;
        [Space(5)]
        public Sprite leftEnd;
        public Sprite rightEnd;
        [Space(5)]
        public Sprite unique;
        [Space(5)]
        public Sprite cross;
        public Sprite tBot;
        public Sprite tTop;
        public Sprite tLeft;
        public Sprite tRight;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (DynamicTileSprites lhs, DynamicTileSprites rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator != (DynamicTileSprites lhs, DynamicTileSprites rhs)
        {
            return !lhs.Equals(rhs);
        }
        public static bool IsNotNull(DynamicTileSprites? dynamicTileSprites)
        {
            return dynamicTileSprites != null;
        }
    }
    public class DynamicTile : TileBase
    {
        public DynamicTileSprites[] sprites = new DynamicTileSprites[1];
        [HideInInspector]
        public bool isLimit;

        private float randomTileSeed;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!IsValidLenght())
            {
                FixLenght();
            }
            else
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].name = "Set " + (i + 1);
                }
            }
        }

        // The following is a helper that adds a menu item to create a MyTile Asset
        [MenuItem("Assets/tilegenX/DynamicTile")]
        public static void CreateMyTile()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Dynamic Tile", "New Dynamic Tile", "Asset", "Save Dynamic Tile");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(CreateInstance<DynamicTile>(), path);
        }
#endif
        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            base.StartUp(position, tilemap, go);

            if (!IsValidLenght())
            {
                FixLenght();
            }

            randomTileSeed = Mathf.RoundToInt(Random.value);

            return true;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            tileData.colliderType = Tile.ColliderType.Grid;

            if (IsValidLenght())
            {
                int i = randomTileSeed > 0.5f ? 0 : sprites.Length - 1;

                if (!HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(0, -1, 0)))
                {
                    tileData.sprite = sprites[i].botEnd != null ? sprites[i].botEnd : null;//abajo
                    isLimit = true;
                    return;
                }
                else if (!HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)))
                {
                    tileData.sprite = sprites[i].topEnd != null ? sprites[i].topEnd : null;//arriba
                    isLimit = true;
                    return;
                }
                else if (!HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(-1, 0, 0)))
                {
                    tileData.sprite = sprites[i].leftEnd != null ? sprites[i].leftEnd : null;//izquierda
                    isLimit = true;
                    return;
                }
                else if (!HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0)))
                {
                    tileData.sprite = sprites[i].rightEnd != null ? sprites[i].rightEnd : null; //derecha
                    isLimit = true;
                    return;
                }
                else
                {

                    if (!HasWall(tilemap, position - new Vector3Int(1, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 1, 0))
                   && !HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 0, 0))
                   && !HasWall(tilemap, position - new Vector3Int(1, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, -1, 0)))
                    {
                        tileData.sprite = sprites[i].unique != null ? sprites[i].unique : null;
                        isLimit = true;
                        return;
                    }
                    else if (HasWall(tilemap, position - new Vector3Int(1, 1, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(-1, 1, 0))
                  && HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(-1, 0, 0))
                  && HasWall(tilemap, position - new Vector3Int(1, -1, 0)) && HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && HasWall(tilemap, position - new Vector3Int(-1, -1, 0)))
                    {
                        tileData.sprite = sprites[i].center != null ? sprites[i].center : null;
                        isLimit = false;
                        return;
                    }
                    else if (!HasWall(tilemap, position - new Vector3Int(1, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(1, -1, 0))
                   && HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(0, -1, 0)) & HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0)))
                    {
                        tileData.sprite = sprites[i].cross != null ? sprites[i].cross : null;
                        isLimit = true;
                        return;
                    }

                    else if (HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0))
                   && !HasWall(tilemap, position - new Vector3Int(1, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 1, 0)))
                    {
                        tileData.sprite = sprites[i].tBot != null ? sprites[i].tBot : null;
                        isLimit = true;
                        return;
                    }
                    else if (HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0))
                   && !HasWall(tilemap, position - new Vector3Int(1, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, -1, 0)))
                    {
                        tileData.sprite = sprites[i].tTop != null ? sprites[i].tTop : null;
                        isLimit = true;
                        return;
                    }
                    else if (HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(-1, 0, 0))
                   && !HasWall(tilemap, position - new Vector3Int(-1, -1, 0)) && !HasWall(tilemap, position - new Vector3Int(-1, 1, 0)))
                    {
                        tileData.sprite = sprites[i].tRight != null ? sprites[i].tRight : null;
                        isLimit = true;
                        return;
                    }
                    else if (HasWall(tilemap, position - new Vector3Int(0, -1, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0))
                   && !HasWall(tilemap, position - new Vector3Int(1, 1, 0)) && !HasWall(tilemap, position - new Vector3Int(1, -1, 0)))
                    {
                        tileData.sprite = sprites[i].tLeft != null ? sprites[i].tLeft : null;
                        isLimit = true;
                        return;
                    }
                    else
                    {
                        if (!HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(0, 1, 0))
                            && HasWall(tilemap,position - new Vector3Int(-1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(0, -1, 0)))
                        {
                            tileData.sprite = sprites[i].leftbot != null ? sprites[i].leftbot : null;//left-bot
                            isLimit = true;
                            return;
                        }
                        else if (!HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position + new Vector3Int(0, 1, 0))
                            && HasWall(tilemap, position - new Vector3Int(-1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)))
                        {
                            tileData.sprite = sprites[i].leftTop != null ? sprites[i].leftTop : null;//left-top
                            isLimit = true;
                            return;
                        }
                        else if (!HasWall(tilemap, position + new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position - new Vector3Int(0, 1, 0))
                            && HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(0, -1, 0)))
                        {
                            tileData.sprite = sprites[i].rightBot != null ? sprites[i].rightBot : null;//right-bot
                            isLimit = true;
                            return;
                        }
                        else if (!HasWall(tilemap, position + new Vector3Int(1, 0, 0)) && !HasWall(tilemap, position + new Vector3Int(0, 1, 0))
                            && HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)))
                        {
                            tileData.sprite = sprites[i].rightTop != null ? sprites[i].rightTop : null;//right-top
                            isLimit = true;
                            return;
                        }
                        else
                        {
                            if (!HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position + new Vector3Int(0, 1, 0)))
                            {
                                tileData.sprite = sprites[i].left != null ? sprites[i].left : null;//left
                                isLimit = true;
                                return;
                            }
                            else if (!HasWall(tilemap, position + new Vector3Int(1, 0, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position + new Vector3Int(0, 1, 0)))
                            {
                                tileData.sprite = sprites[i].right != null ? sprites[i].right : null;//right
                                isLimit = true;
                                return;
                            }
                            else if (!HasWall(tilemap, position - new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position + new Vector3Int(1, 0, 0)))
                            {
                                tileData.sprite = sprites[i].bot != null ? sprites[i].bot : null;//bot
                                isLimit = true;
                                return;
                            }
                            else if (!HasWall(tilemap, position + new Vector3Int(0, 1, 0)) && HasWall(tilemap, position - new Vector3Int(1, 0, 0)) && HasWall(tilemap, position + new Vector3Int(1, 0, 0)))
                            {
                                tileData.sprite = sprites[i].top != null ? sprites[i].top : null;//top
                                isLimit = true;
                                return;
                            }
                            else
                            {
                                if (!HasWall(tilemap, position - new Vector3Int(1, 1, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)))
                                {
                                    tileData.sprite = sprites[i].rightTop != null ? sprites[i].rightTop : null;//left-bot
                                    isLimit = true;
                                    return;
                                }
                                else if (!HasWall(tilemap, position - new Vector3Int(1, -1, 0)) && HasWall(tilemap, position + new Vector3Int(0, 1, 0)))
                                {
                                    tileData.sprite = sprites[i].rightBot != null ? sprites[i].rightBot : null;//left-top
                                    isLimit = true;
                                    return;
                                }
                                else if (!HasWall(tilemap, position - new Vector3Int(-1, 1, 0)) && HasWall(tilemap, position - new Vector3Int(0, 1, 0)))
                                {
                                    tileData.sprite = sprites[i].leftTop != null ? sprites[i].leftTop : null;//right-bot
                                    isLimit = true;
                                    return;
                                }
                                else if (!HasWall(tilemap, position - new Vector3Int(-1, -1, 0)) && HasWall(tilemap, position + new Vector3Int(0, 1, 0)))
                                {
                                    tileData.sprite = sprites[i].leftbot != null ? sprites[i].leftbot : null;//right-top
                                    isLimit = true;
                                    return;
                                }
                                else
                                {
                                    tileData.sprite = sprites[i].unique != null ? sprites[i].unique : null;//uniqueWall
                                    isLimit = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
#if UNITY_STANDALONE
            else
            {
                FixLenght();
            }
#endif
        }
        // This refreshes itself and other RoadTiles that are orthogonally and diagonally adjacent
        public override void RefreshTile(Vector3Int location, ITilemap tilemap)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int position = new Vector3Int(location.x + x, location.y + y, location.z);

                    if (HasWall(tilemap, position))
                    {
                        tilemap.RefreshTile(position);
                    }
                }
            }
        }

        public bool HasWall(ITilemap tilemap, Vector3Int position)
        {
            DynamicTile temp;

            temp = tilemap.GetTile<DynamicTile>(position);

            return temp == this;
        }

        public bool IsValidLenght()
        {
            if (sprites.Length < 1)
            {
                return false;
            }
            else if (sprites.Length > 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void FixLenght()
        {
            if (sprites.Length < 1)
            {
                sprites = new DynamicTileSprites[1];

#if UNITY_EDITOR
                Debug.LogError(name + " Sprites size must be 1 or 2.");
#endif
            }
            else if (sprites.Length > 2)
            {
                DynamicTileSprites[] temp = sprites;
                sprites = new DynamicTileSprites[2];

                sprites[0] = temp[0];
                sprites[1] = temp[1];

#if UNITY_EDITOR
                Debug.LogError(name + " Sprites size must be 1 or 2.");
            }
            else
            {
                Debug.LogWarning(name + ", are you sure you want to Fix Lenght?", this);

            }
#endif
        }
    }
}

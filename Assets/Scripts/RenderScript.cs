using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderScript : MonoBehaviour
{
    public GameObject[,] tiles = null;
    public Camera mainCamera;

    [Header("Config")]
    public Color[] starColors;

    [Header("Prefabs")]
    public GameObject emptyTile;
    public GameObject wallTile;
    public GameObject starTile;
    public GameObject portalTile;

    public void Render (LevelState level) 
    {
        if(tiles != null)
        {
            foreach (var obj in tiles)
            {
                Destroy(obj);
            }
        }

        // Set Camera Size
        mainCamera.orthographicSize = Mathf.Max(level.verticalSize / 2, level.horizontalSize / 3.5f) + 1;

        tiles = new GameObject[level.horizontalSize, level.verticalSize];

        for (int x = 0; x < level.horizontalSize; x++)
        {
            for (int y = 0; y < level.verticalSize; y++)
            {
                var tile = level[x, y];
                Vector3 tilePos = level.CoordinateToWorldPosition(new Coordinate(x, y));
                if(tile == null)
                {
                    tiles[x, y] = Instantiate(emptyTile, tilePos, new Quaternion());
                }
                else if (tile.GetType() == typeof(TileTypes.Wall))
                {
                    tiles[x, y] = Instantiate(wallTile, tilePos, new Quaternion());
                }
                else if (tile.GetType() == typeof(TileTypes.Star))
                {
                    tiles[x, y] = Instantiate(emptyTile, tilePos, new Quaternion());
                    var star = Instantiate(starTile, tiles[x,y].transform);
                    star.GetComponent<SpriteRenderer>().color = starColors[((TileTypes.Star)tile).ownerPlayerId];
                }
                else if (tile.GetType() == typeof(TileTypes.Portal))
                {
                    tiles[x, y] = Instantiate(portalTile, tilePos, new Quaternion());
                    tiles[x, y].GetComponent<PortalController>()?.AssignColor(((TileTypes.Portal)tile).portalIndex);
                }
                tiles[x, y].transform.parent = this.transform;
                tiles[x, y].name = $"[{x},{y}]";
            }
        }
    }

    public void SetToEmpty (LevelState level, Coordinate coordinate)
    {
        Destroy(tiles[coordinate.x, coordinate.y]);
        tiles[coordinate.x, coordinate.y] = Instantiate(emptyTile, level.CoordinateToWorldPosition(coordinate), new Quaternion());
    }
}

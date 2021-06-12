using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderScript : MonoBehaviour
{
    public GameObject[,] tiles = null;
    public Camera mainCamera;

    [Header("Config")]
    public Color redStarColor;
    public Color blueStarColor;

    [Header("Prefabs")]
    public GameObject emptyTile;
    public GameObject wallTile;
    public GameObject starTile;

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
        float xOffset = (level.horizontalSize - 1) / 2f;
        float yOffset = (level.verticalSize - 1) / 2f;

        for (int x = 0; x < level.horizontalSize; x++)
        {
            for (int y = 0; y < level.verticalSize; y++)
            {
                var tile = level[x, y];
                Vector3 tilePos = new Vector3(x - xOffset, y - yOffset, 0);
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
                    star.GetComponent<SpriteRenderer>().color = ((TileTypes.Star)tile).color == "R" ? redStarColor : blueStarColor;
                }
                tiles[x, y].transform.parent = this.transform;
                tiles[x, y].name = $"[{x},{y}]";
            }
        }
    }
}

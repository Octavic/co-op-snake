using System.Collections;
using UnityEngine;

public class SnakeGrid : MonoBehaviour
{
    public float GridSize;
    public Vector2 GridCellOffset;

    // Use this for initialization
    void Start()
    {
        this.GridCellOffset = new Vector2(this.GridSize / 2, -this.GridSize / 2);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

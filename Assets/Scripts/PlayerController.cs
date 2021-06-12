using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float GridSize;
    public List<KeyCode> ControlKeys;
    public DirectionEnum CurrentlyFacing;
    public PlayerSegment SegmentPrefab;

    private static List<DirectionEnum> Directions = new List<DirectionEnum>() {
        DirectionEnum.Right,
        DirectionEnum.Down,
        DirectionEnum.Left,
        DirectionEnum.Up
    };
    private static Dictionary<DirectionEnum, Vector2> Movements = new Dictionary<DirectionEnum, Vector2>
    {
        {
            DirectionEnum.Right, new Vector2(1, 0)
        },
        {
            DirectionEnum.Down, new Vector2(0, -1)
        }
        ,
        {
            DirectionEnum.Left, new Vector2(-1, 0)
        },
        {
            DirectionEnum.Up, new Vector2(0, 1)
        }
    };

    public PlayerSegment Head
    {
        get
        {
            return this.Body.First();
        }
    }
    public PlayerSegment Neck
    {
        get
        {
            return this.Body[1];
        }
    }
    public PlayerSegment Tail
    {
        get
        {
            return this.Body.Last();
        }
    }

    /// <summary>
    /// Position of each body
    /// </summary>
    public List<PlayerSegment> Body;
    public int Length
    {
        get
        {
            return this.Body.Count;
        }
    }

    /// <summary>
    /// How many pieces needed to be added to the snake
    /// </summary>
    public int PendingPieces;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(this.ControlKeys[i]))
            {
                var newFacing = Directions[i];
                var potentialHeadCoor = this.Head.Coordinate + Movements[newFacing];
                if (potentialHeadCoor != this.Neck.Coordinate)
                {
                    this.CurrentlyFacing = newFacing;
                }
            }
        }
    }

    /// <summary>
    /// Called when a game tick passes
    /// </summary>
    public void GameUpdate()
    {
        var newHeadCoor = this.Head.Coordinate + Movements[this.CurrentlyFacing];
        var newHead = Instantiate(this.SegmentPrefab, this.transform);
        newHead.Coordinate = newHeadCoor;
        newHead.transform.position = newHeadCoor * this.GridSize;
        this.Body.Insert(0, newHead);
        if (this.PendingPieces > 0)
        {
            this.PendingPieces--;
        }
        else
        {
            Destroy(this.Tail.gameObject);
            this.Body.RemoveAt(this.Body.Count - 1);
        }
    }
}

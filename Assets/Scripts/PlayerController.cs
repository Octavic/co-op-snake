using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public List<KeyCode> ControlKeys;
    public DirectionEnum CurrentlyFacing;
    public PlayerSegment SegmentPrefab;
    public PlayerSegment HeadPrefab;

    private static List<DirectionEnum> Directions = new List<DirectionEnum>() {
        DirectionEnum.Right,
        DirectionEnum.Down,
        DirectionEnum.Left,
        DirectionEnum.Up
    };
    private static Dictionary<DirectionEnum, Coordinate> Movements = new Dictionary<DirectionEnum, Coordinate>
    {
        {
            DirectionEnum.Right, new Coordinate(1, 0)
        },
        {
            DirectionEnum.Down, new Coordinate(0, -1)
        },
        {
            DirectionEnum.Left, new Coordinate(-1, 0)
        },
        {
            DirectionEnum.Up, new Coordinate(0, 1)
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
            if (this.Body.Count < 2)
            {
                return null;
            }
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

                if (this.Neck == null)
                {
                    this.CurrentlyFacing = newFacing;
                    continue;
                }

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
        // Add new head
        var newHeadCoor = this.Head.Coordinate + Movements[this.CurrentlyFacing];
        var newHead = Instantiate(this.HeadPrefab, this.transform);
        newHead.Coordinate = newHeadCoor;
        this.Body.Insert(0, newHead);

        // Modify neck (The old head) to be normal segment
        if (this.Neck != null)
        {
            var neckCoor = this.Neck.Coordinate;
            var newNeck = Instantiate(this.SegmentPrefab, this.transform);
            newNeck.Coordinate = neckCoor;

            // Replace old neck
            Destroy(this.Neck.gameObject);
            this.Body[1] = newNeck;
        }

        // Check if there are pending pieces from eating fruit or starting game. Otherwise remove tail
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

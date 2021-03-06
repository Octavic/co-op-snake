using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerIdentifier;
    public List<KeyCode> ControlKeys;
    public DirectionEnum CurrentlyFacing;
    public PlayerSegment SegmentPrefab;
    public PlayerSegment HeadPrefab;
    public Material completeSnakeMaterial;
    public float BufferDuration;
    private Queue<PlayerInputBufferItem> InputBuffer = new Queue<PlayerInputBufferItem>();
    
    /// <summary>
    /// All possible directions ordered
    /// </summary>
    private static List<DirectionEnum> Directions = new List<DirectionEnum>() {
        DirectionEnum.Right,
        DirectionEnum.Down,
        DirectionEnum.Left,
        DirectionEnum.Up
    };

    /// <summary>
    /// All possible movements based on direction
    /// </summary>
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
                this.InputBuffer.Enqueue(new PlayerInputBufferItem(i, Time.timeSinceLevelLoad));
            }
        }
    }

    /// <summary>
    /// Called when a game tick passes
    /// </summary>
    public void GameUpdate()
    {
        // Determine facing
        var currentTime = Time.timeSinceLevelLoad;
        while(this.InputBuffer.Count > 0)
        {
            var input = this.InputBuffer.Dequeue();
            
            // Check if input is stale
            if ((currentTime - input.Time) > this.BufferDuration)
            {
                continue;
            }

            // Check if move is legal
            var newFacing = Directions[input.KeycodeId];
            if (this.Neck == null)
            {
                this.CurrentlyFacing = newFacing;
                break;
            }

            var potentialHeadCoor = this.Head.Coordinate + Movements[newFacing];
            if (potentialHeadCoor != this.Neck.Coordinate)
            {
                this.CurrentlyFacing = newFacing;
                break;
            }
        }

        // Add new piece at head
        var newTile = Instantiate(this.SegmentPrefab, this.transform);
        newTile.Coordinate = this.Head.Coordinate;
        this.Body.Insert(1, newTile);

        // Move head
        this.Head.Coordinate += Movements[this.CurrentlyFacing];

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

    public void OnLevelComplete()
    {
        this.StartCoroutine(this.DestroySnake());
    }
    public IEnumerator DestroySnake()
    {
        for(int i = 0; i < this.Body.Count; i++)
        {
            this.Body[i].GetComponent<SpriteRenderer>().material = completeSnakeMaterial;
            yield return new WaitForSeconds(0.5f);
        }
    }
}

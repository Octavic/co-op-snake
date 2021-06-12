using System.Collections;
using UnityEngine;

public class PlayerSegment : SnakeGridTile
{
    public Coordinate Coordinate
    {
        get
        {
            return this._coordinate;
        }
        set
        {
            this._coordinate = value;
            this.transform.position = new Vector2(value.x, value.y) * this.GetComponentInParent<SnakeGrid>().GridSize;
        }
    }
    private Coordinate _coordinate;

    public override void OnPlayerHeadEnter(PlayerController enteredPlayer)
    {
        GameObject.FindObjectOfType<GameController>().OnGameOver();
    }
}

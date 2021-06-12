using System.Collections;
using UnityEngine;

public class PlayerSegment : SnakeGridTile
{
    public Vector2 Coordinate
    {
        get
        {
            return this._coordinate;
        }
        set
        {
            this._coordinate = value;
            this.transform.position = value * this.GetComponentInParent<SnakeGrid>().GridSize;
        }
    }
    private Vector2 _coordinate;

    public override void OnPlayerHeadEnter(PlayerController enteredPlayer)
    {
        GameObject.FindObjectOfType<GameController>().OnGameOver();
    }
}

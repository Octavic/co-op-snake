using System.Collections;
using UnityEngine;

public class PlayerSegment : SnakeGridTile
{
    public override void OnPlayerHeadEnter(PlayerController enteredPlayer)
    {
        GameObject.FindObjectOfType<GameController>().OnGameOver();
    }
}

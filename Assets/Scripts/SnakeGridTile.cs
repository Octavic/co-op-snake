using System.Collections;
using UnityEngine;

public abstract class SnakeGridTile: MonoBehaviour
{
    /// <summary>
    /// Called when the player's head tile collides with this tile
    /// </summary>
    public abstract void OnPlayerHeadEnter(PlayerController enteredPlayer);
}

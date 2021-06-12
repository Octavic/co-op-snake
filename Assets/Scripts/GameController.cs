using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// Number of seconds between each game update
    /// </summary>
    public float TickSpeed;

    public void OnGameOver()
    {
        Debug.Log("GAME OVER");
    }
}

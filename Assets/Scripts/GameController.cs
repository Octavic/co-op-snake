using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LevelState _level;

    public string levelName;

    /// <summary>
    /// Number of seconds between each game update
    /// </summary>
    public float TickSpeed;

    public void Start()
    {
        var levelAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
        _level = new LevelState(levelAsset.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries));
    }

    public void OnGameOver()
    {
        Debug.Log("GAME OVER");
    }
}

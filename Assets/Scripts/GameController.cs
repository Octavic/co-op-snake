using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LevelState _level;

    public string levelName;
    public RenderScript levelRenderer;

    /// <summary>
    /// Number of seconds between each game update
    /// </summary>
    public float TickSpeed;

    public void Start()
    {
        var levelAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
        _level = new LevelState(levelAsset.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries));
        levelRenderer.Render(_level);
    }

    public void OnGameOver()
    {
        Debug.Log("GAME OVER");
    }
}

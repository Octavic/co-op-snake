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

    private void Update()
    {

    }

    private IEnumerator ExecuteGameCycle()
    {
        this.GameUpdate();

        // Check for player collision
        yield return new WaitForSeconds(this.TickSpeed);
    }

    private void GameUpdate()
    {
        foreach (var player in GameObject.FindObjectsOfType<PlayerController>())
        {
            player.GameUpdate();
        }
    }

    public void OnGameOver()
    {
        Debug.Log("GAME OVER");
    }
}

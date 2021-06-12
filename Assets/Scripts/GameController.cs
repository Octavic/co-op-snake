using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController staticInstance;

    private LevelState _level;
    public string levelName;
    public RenderScript levelRenderer;

    /// <summary>
    /// Number of seconds between each game update
    /// </summary>
    public float TickSpeed;

    private Coroutine ExecuteGameCoroutine;

    public void Start()
    {
        if (this.levelRenderer != null)
        {
            var levelAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
            _level = new LevelState(levelAsset.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries));
            levelRenderer.Render(_level);
        }

        // Start game
        this.ExecuteGameCoroutine = StartCoroutine(this.ExecuteGame());

        if (staticInstance) Destroy(staticInstance.gameObject);
        staticInstance = this;
    }

    private void Update()
    {

    }

    private IEnumerator ExecuteGame()
    {
        while (true)
        {
            this.GameUpdate();

            // Check for player collision
            yield return new WaitForSeconds(this.TickSpeed);
        }
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

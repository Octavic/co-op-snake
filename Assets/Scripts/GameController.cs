using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController staticInstance;

    private LevelState _level;
    public string levelName;
    public RenderScript levelRenderer;

    public PlayerController PlayerAPrefab;
    public PlayerController PlayerBPrefab;

    private List<PlayerController> players;

    /// <summary>
    /// Number of seconds between each game update
    /// </summary>
    public float TickSpeed;

    private Coroutine ExecuteGameCoroutine;

    public void Start()
    {

    }

    private void Update()
    {

    }

    private void DestroyOldGame()
    {
        if (this.players != null)
        {
            foreach (var player in this.players)
            {
                Destroy(player.gameObject);
            }
            this.players = null;
        }

        if (this.ExecuteGameCoroutine != null)
        {
            StopCoroutine(this.ExecuteGameCoroutine);
            this.ExecuteGameCoroutine = null;
        }
    }

    public void StartGame()
    {
        this.DestroyOldGame();

        // load level
        if (this.levelRenderer != null)
        {
            var levelAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
            _level = new LevelState(levelAsset.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries));
            levelRenderer.Render(_level);
        }

        // Keep track of static isntance
        if (staticInstance)
        {
            Destroy(staticInstance.gameObject);
        }
        staticInstance = this;

        // Start game
        this.ExecuteGameCoroutine = StartCoroutine(this.ExecuteGame());
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

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController staticInstance;
    public int PlayerCount;

    private LevelState _level;
    public string levelName;
    public RenderScript levelRenderer;
    public CountdownOverlay Countdown;
    public GameOverOverlay GameOver;

    public List<PlayerController> PlayerPrefabs;
    public SnakeGrid Grid;
    public float Score;
    public bool IsGameOver { get; private set; }

    private List<PlayerController> players;

    private Coroutine ExecuteGameCoroutine;

    public void Start()
    {
        // Keep track of static isntance
        if (staticInstance)
        {
            Destroy(staticInstance.gameObject);
        }
        staticInstance = this;

        this.StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && this.IsGameOver)
        {
            this.StartGame();
        }
    }

    private void DestroyOldGame()
    {
        // Destroy players
        if (this.players != null)
        {
            foreach (var player in this.players)
            {
                Destroy(player.gameObject);
            }
            this.players = null;
        }

        // Stop coroutine
        if (this.ExecuteGameCoroutine != null)
        {
            StopCoroutine(this.ExecuteGameCoroutine);
            this.ExecuteGameCoroutine = null;
        }
    }

    private void SpawnPlayers()
    {
        if (this._level == null)
        {
            Debug.LogError("Cannont spawn players without initiating level state!");
            return;
        }

        this.players = new List<PlayerController>();
        for (int i = 0; i < this.PlayerCount; i++)
        {
            var newPlayer = Instantiate(this.PlayerPrefabs[i], this.Grid.transform);
            newPlayer.PendingPieces = this._level.GetPlayerSpawnLength(i) - 1;
            this.players.Add(newPlayer);
            var newPlayerPos = this._level.GetPlayerSpawnPos(i);
            newPlayer.Head.Coordinate = newPlayerPos;
        }
    }

    public void StartGame()
    {
        this.IsGameOver = false;

        // Hide gameover overlay
        this.GameOver.Hide();

        // Reset score
        this.Score = 0;

        // Remove old game and game objects
        this.DestroyOldGame();

        // load level
        if (this.levelRenderer != null)
        {
            var levelAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
            _level = new LevelState(
                levelAsset.text.Split(new string[] { "\r\n" },
                System.StringSplitOptions.RemoveEmptyEntries),
                levelRenderer
            );
            levelRenderer.Render(_level);
        }

        // Move grid to line up with the rendered grid
        this.Grid.transform.position = this._level.CoordinateToWorldPosition(new Coordinate(0, 0));

        // Spawn players
        this.SpawnPlayers();

        // Start game
        this.ExecuteGameCoroutine = StartCoroutine(this.ExecuteGame());
    }

    private IEnumerator ExecuteGame()
    {
        // Do the countdown
        var countdownBetween = 0.5f;

        this.Countdown.Show();
        this.Countdown.ChangeText("3");
        yield return new WaitForSeconds(countdownBetween);
        this.Countdown.ChangeText("2");
        yield return new WaitForSeconds(countdownBetween);
        this.Countdown.ChangeText("1");
        yield return new WaitForSeconds(countdownBetween);
        this.Countdown.Hide();

        while (!this.IsGameOver)
        {
            // Update
            this.GameUpdate();

            // Check for player collision
            foreach (var player in this.players)
            {
                var playerHeadCoordiante = player.Head.Coordinate;

                // Check for player collision
                foreach (var checkPlayer in this.players)
                {
                    // Skip head against head collision check
                    for (int i = 1; i < checkPlayer.Body.Count; i++)
                    {
                        var tile = checkPlayer.Body[i];
                        if (tile.Coordinate == playerHeadCoordiante)
                        {
                            tile.OnPlayerHeadEnter(player);
                        }
                    }
                }

                // Check for level 
                this._level.Activate(playerHeadCoordiante.x, playerHeadCoordiante.y, player);
            }

            yield return new WaitForSeconds(this._level.tickSpeed);
        }
    }

    private void GameUpdate()
    {
        foreach (var player in this.players)
        {
            player.GameUpdate();
        }
    }

    public void OnGameOver()
    {
        this.IsGameOver = true;
        this.GameOver.Show(this.Score);
    }

    public void AddScore(float score)
    {
        this.Score += score;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float ScorePerLevel;
    public float ExpectedTime;

    public int PlayerCount;

    public int CurrentLevel;
    public int LevelTotal;
    public Text LevelText;
    public Text HintText;
    public Text TimeText;
    public List<string> Hints;

    public FadeBlack FadeEffect;
    public WhiteFlash FlashEffect;

    public RenderScript levelRenderer;
    public CountdownOverlay Countdown;
    public GameOverOverlay GameOver;
    public LevelCompleteOverlay LevelComplete;

    public List<PlayerController> PlayerPrefabs;
    public SnakeGrid Grid;

    public float Score { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsTiming { get; private set; }
    public bool PlayerWon { get; private set; }

    public static GameController staticInstance;

    private List<PlayerController> players;
    private LevelState _level;
    private Coroutine ExecuteGameCoroutine;

    private float LevelStartTime;

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
            if (this.PlayerWon)
            {
                this.CurrentLevel++;
                if (this.CurrentLevel > this.LevelTotal)
                {
                    StartCoroutine(this.OnBeatGame());
                    return;
                }
            }
            this.StartGame();
        }

        // Show time
        if (this.IsTiming)
        {
            var timePassed = Time.timeSinceLevelLoad - this.LevelStartTime;
            this.TimeText.text = timePassed.ToString("0.00");
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
            newPlayer.CurrentlyFacing = (DirectionEnum)this._level.GetPlayerSpawnDirection(i);
            newPlayer.playerIdentifier = i;
            this.players.Add(newPlayer);
            var newPlayerPos = this._level.GetPlayerSpawnPos(i);
            newPlayer.Head.Coordinate = newPlayerPos;
            newPlayer.BufferDuration = this._level.tickSpeed * 1.8f;
        }
    }

    public void StartGame()
    {
        // Reset
        this.IsGameOver = false;

        // Hide all other overlays
        this.GameOver.Hide();
        this.LevelComplete.Hide();

        // Remove old game and game objects
        this.DestroyOldGame();

        // load level
        if (this.levelRenderer != null)
        {
            var levelAsset = Resources.Load<TextAsset>($"Levels/{this.CurrentLevel}");
            _level = new LevelState(
                levelAsset.text.Split(new string[] { "\r\n" },
                System.StringSplitOptions.RemoveEmptyEntries),
                levelRenderer
            );
            levelRenderer.Render(_level);

            // Set level text
            this.LevelText.text = $"LEVEL {this.CurrentLevel}";

            // Set hints
            if (this.CurrentLevel <= this.Hints.Count)
            {
                this.HintText.text = this.Hints[this.CurrentLevel - 1];
            }
            else
            {
                this.HintText.text = "";
            }
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

        // Start Timer
        this.IsTiming = true;
        this.LevelStartTime = Time.timeSinceLevelLoad;

        while (!this.IsGameOver)
        {
            // Update
            this.GameUpdate();

            // Check for win condition
            if (this._level.StarsCollected)
            {
                var player1HeadCoor = this.players[0].Head.Coordinate;
                var player2HeadCoor = this.players[1].Head.Coordinate;

                if (player1HeadCoor.DistanceTo(player2HeadCoor) <= 1)
                {
                    this.OnLevelComplete();
                    continue;
                }
            }

            // Check for player collision
            foreach (var player in this.players)
            {
                var playerHeadCoordiante = player.Head.Coordinate;

                // Check for player collision
                foreach (var checkPlayer in this.players)
                {
                    // Skip head against head collision check
                    var startingIndex = player == checkPlayer ? 1 : 0;
                    for (int i = startingIndex; i < checkPlayer.Body.Count; i++)
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

    public void OnLevelComplete()
    {
        // Stop timer
        this.IsTiming = false;

        // Add score
        this.AddScore(this.ScorePerLevel);
        var timeToBeat = Time.timeSinceLevelLoad - this.LevelStartTime;
        this.AddScore(this.ExpectedTime - timeToBeat);

        this.IsGameOver = true;
        this.LevelComplete.SetScore(this.Score);
        this.LevelComplete.Show();
        this.PlayerWon = true;
        this.FlashEffect.Flash();

      
        foreach (var player in this.players)
        {
            player.OnLevelComplete();
        }
    }

    public void OnGameOver()
    {
        // Stop timer
        this.IsTiming = false;

        this.IsGameOver = true;
        this.GameOver.SetScore(this.Score);
        this.GameOver.Show();
        this.PlayerWon = false;
    }

    public void AddScore(float score)
    {
        this.Score += score;
    }

    private IEnumerator OnBeatGame()
    {
        this.FadeEffect.ToBlack();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(2);
    }
}

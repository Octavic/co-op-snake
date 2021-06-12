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

    public List<PlayerController> PlayerPrefabs;
    public SnakeGrid Grid;
    private List<PlayerController> players;

    private Coroutine ExecuteGameCoroutine;

    public void Start()
    {
        this.StartGame();
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
        // Remove old game and game objects
        this.DestroyOldGame();

        // load level
        if (this.levelRenderer != null)
        {
            var levelAsset = Resources.Load<TextAsset>($"Levels/{levelName}");
            _level = new LevelState(levelAsset.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries), levelRenderer);
            levelRenderer.Render(_level);
        }

        // Move grid to line up with the rendered grid
        this.Grid.transform.position = this.levelRenderer.tiles[0, 0].transform.position;

        // Keep track of static isntance
        if (staticInstance)
        {
            Destroy(staticInstance.gameObject);
        }
        staticInstance = this;

        // Spawn players
        this.SpawnPlayers();

        // Start game
        this.ExecuteGameCoroutine = StartCoroutine(this.ExecuteGame());
    }

    private IEnumerator ExecuteGame()
    {
        while (true)
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
        Debug.Log("GAME OVER");
    }
}

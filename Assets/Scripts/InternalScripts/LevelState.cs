using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelState
{
    public enum TileType
    {
        Empty,
        Wall,
        Star
    }

    public int horizontalSize;
    public int verticalSize;
    public float tickSpeed;
    public float tickDecrementFactor;
    public int tickDecrementInterval = -1;
    public int tickElapsed = 0;

    public int starsRemaining;

    public readonly int playerCount;
    Coordinate[] playerStartPos;
    int[] playerStartLength;
    int[] playerStartDirection;

    private TileTypes.Tile[,] map;

    // Offset for unity transform positions
    private float xOffset;
    private float yOffset;

    private RenderScript renderer;

    public LevelState(string[] file, RenderScript renderer)
    {
        this.renderer = renderer;
        if (file[0] == "snake level format v1")
        {
            horizontalSize = ParseIntAttribute(file[1], "hSize");
            verticalSize = ParseIntAttribute(file[2], "vSize");
            tickSpeed = ParseFloatAttribute(file[3], "tickSpeed");
            try
            {
                tickDecrementFactor = ParseFloatAttribute(file[4], "tickDecrementFactor");
                tickDecrementInterval = ParseIntAttribute(file[5], "tickDecrementInterval");
                playerCount = ParseIntAttribute(file[6], "playerCount");
            }
            catch
            {
                playerCount = ParseIntAttribute(file[4], "playerCount");
            }

            playerStartPos = new Coordinate[playerCount];
            playerStartLength = new int[playerCount];
            playerStartDirection = new int[playerCount];

            map = new TileTypes.Tile[horizontalSize, verticalSize];

            xOffset = (horizontalSize - 1) / 2f;
            yOffset = (verticalSize - 1) / 2f;

            bool parsingContent = false;
            for (int i = 0; i < file.Length; i++)
            {
                if (!parsingContent && file[i] == "[CONTENT]")
                {
                    parsingContent = true;
                }
                else if (parsingContent && !string.IsNullOrWhiteSpace(file[i]) && !file[i].StartsWith("#"))
                {
                    string[] p = file[i].Split(' ');
                    switch (p[0])
                    {
                        case "p":
                            ValidateParameterCount(p, 5, i);
                            int playerIdentifier = ParseIntValue(p[1], i);
                            if (playerIdentifier >= playerCount)
                            {
                                throw new ArgumentException($"Player {playerIdentifier} is outside the index. Only {playerCount} players were specified");
                            }
                            if (playerStartLength[playerIdentifier] != 0)
                            {
                                throw new Exception($"Player {playerIdentifier} postion was defined twice");
                            }
                            playerStartPos[playerIdentifier] = new Coordinate(ParseIntValue(p[2], i), ParseIntValue(p[3], i));
                            playerStartLength[playerIdentifier] = ParseIntValue(p[4], i);
                            break;

                        case "w":
                            ValidateParameterCount(p, 5, i);
                            int startX = ParseIntValue(p[1], i);
                            int startY = ParseIntValue(p[2], i);
                            int width = ParseIntValue(p[3], i);
                            int height = ParseIntValue(p[4], i);
                            for (int x = startX; x < startX + width; x++)
                            {
                                for (int y = startY; y < startY + height; y++)
                                {
                                    if (x >= horizontalSize || y >= verticalSize) throw new Exception($"Line {i}: size exceeded the dimensions of the map");
                                    map[x, y] = new TileTypes.Wall()
                                    {
                                        coordinate = new Coordinate(x, y)
                                    };
                                }
                            }
                            break;

                        case "s":
                            ValidateParameterCount(p, 4, i);
                            int starX = ParseIntValue(p[1], i);
                            int starY = ParseIntValue(p[2], i);
                            if (starX >= horizontalSize || starY >= verticalSize) throw new Exception($"Line {i}: size exceeded the dimensions of the map");
                            map[starX, starY] = new TileTypes.Star()
                            {
                                coordinate = new Coordinate(starX, starY),
                                ownerPlayerId = ParseIntValue(p[3], i)
                            };
                            starsRemaining++;
                            break;


                        default:
                            throw new Exception($"Line {i}: Unsupported Content Type {p[0]}");
                    }
                }
            }
        }
        else if (file[0] == "snake level format v2")
        {
            horizontalSize = ParseIntAttribute(file[1], "hSize");
            verticalSize = ParseIntAttribute(file[2], "vSize");
            tickSpeed = ParseFloatAttribute(file[3], "tickSpeed");
            try
            {
                tickDecrementFactor = ParseFloatAttribute(file[4], "tickDecrementFactor");
                tickDecrementInterval = ParseIntAttribute(file[5], "tickDecrementInterval");
                playerCount = ParseIntAttribute(file[6], "playerCount");
            }
            catch
            {
                playerCount = ParseIntAttribute(file[4], "playerCount");
            }

            playerStartPos = new Coordinate[playerCount];
            playerStartLength = new int[playerCount];
            playerStartDirection = new int[playerCount];

            map = new TileTypes.Tile[horizontalSize, verticalSize];

            xOffset = (horizontalSize - 1) / 2f;
            yOffset = (verticalSize - 1) / 2f;

            // Parse Player starting info
            bool parsingContent = false;
            for (int i = 0; i < file.Length; i++)
            {
                if (!parsingContent && file[i] == "[PLAYERS]")
                {
                    parsingContent = true;
                }
                else if (parsingContent && !string.IsNullOrWhiteSpace(file[i]) && !file[i].StartsWith("#"))
                {
                    if (file[i] == "[PLAYERS END]")
                    {
                        break;
                    }
                    string[] p = file[i].Split(' ');
                    ValidateParameterCount(p, 5, i);
                    int playerIdentifier = ParseIntValue(p[0], i);
                    if (playerIdentifier >= playerCount)
                    {
                        throw new ArgumentException($"Player {playerIdentifier} is outside the index. Only {playerCount} players were specified");
                    }
                    if (playerStartLength[playerIdentifier] != 0)
                    {
                        throw new Exception($"Player {playerIdentifier} postion was defined twice");
                    }
                    playerStartPos[playerIdentifier] = new Coordinate(ParseIntValue(p[1], i), ParseIntValue(p[2], i));
                    playerStartLength[playerIdentifier] = ParseIntValue(p[3], i);
                    playerStartDirection[playerIdentifier] = ParseIntValue(p[4], i);
                }
            }

            parsingContent = false;
            int lineIndex = 0;
            Dictionary<char, TileTypes.Portal> pendingPortals = new Dictionary<char, TileTypes.Portal>();
            int nextPortalIndex = 0;

            for (int i = 0; i < file.Length; i++)
            {
                if (!parsingContent && file[i] == "[CONTENT]")
                {
                    parsingContent = true;
                }
                else if (parsingContent && !string.IsNullOrWhiteSpace(file[i]) && !file[i].StartsWith("#"))
                {
                    if(file[i] == "[CONTENT END]")
                    {
                        break;
                    }
                    if (file[i].Length != horizontalSize)
                    {
                        throw new Exception($"Line {i}: content has length {file[i].Length}, which is not the correct horizontal size of {horizontalSize}");
                    }
                    if (lineIndex >= verticalSize)
                    {
                        throw new Exception($"Line {i}: Too many horizontal lines of content. Expected ${verticalSize} lines");
                    }
                    for (int j = 0; j < file[i].Length; j++)
                    {
                        int playerIndex = file[i][j] - '0';
                        if (playerIndex >= 0 && playerIndex < playerCount)
                        {
                            starsRemaining++;
                            map[j, verticalSize - 1 - lineIndex] = new TileTypes.Star()
                            {
                                coordinate = new Coordinate(j, verticalSize - 1 - lineIndex),
                                ownerPlayerId = playerIndex
                            };
                        }
                        else
                        {
                            switch (file[i][j])
                            {
                                case ' ':
                                    break;
                                case 'w':
                                    map[j, verticalSize - 1 - lineIndex] = new TileTypes.Wall()
                                    {
                                        coordinate = new Coordinate(j, verticalSize - 1 - lineIndex)
                                    };
                                    break;
                                case '!':
                                case '@':
                                case '#':
                                case '$':
                                case '%':
                                case '^':
                                case '&':
                                case '*':
                                case '~':
                                case '`':
                                    char portalKey = file[i][j];
                                    var newPortal = new TileTypes.Portal()
                                    {
                                        coordinate = new Coordinate(j, verticalSize - 1 - lineIndex),
                                        
                                    };
                                    map[j, verticalSize - 1 - lineIndex] = newPortal;
                                    if (pendingPortals.TryGetValue(portalKey, out var correspondingPortal))
                                    {
                                        if (correspondingPortal.ConnectedPortal != null)
                                        {
                                            throw new Exception($"There are more than two portals with key {portalKey}");
                                        }
                                        newPortal.ConnectedPortal = correspondingPortal;
                                        newPortal.portalIndex = correspondingPortal.portalIndex;
                                        correspondingPortal.ConnectedPortal = newPortal;
                                    }
                                    else
                                    {
                                        newPortal.portalIndex = nextPortalIndex;
                                        nextPortalIndex++;
                                        pendingPortals.Add(portalKey, newPortal);
                                    }
                                    break;

                                default:
                                    throw new Exception($"Line {i}:{j}: Unsupported character {file[i][j]}");
                            }
                        }
                    }
                    lineIndex++;
                }
            }
            if (lineIndex < verticalSize)
            {
                throw new Exception($"Too few horizontal lines of content. Expected {verticalSize} lines, got {lineIndex + 1} lines");
            }
        }
        else
        {
            throw new NotSupportedException("Only v1 and v2 format are supported");
        }

        // Validate all players have been moved
        for (int i = 0; i < playerStartLength.Length; i++)
        {
            if (playerStartLength[i] == 0)
            {
                throw new ArgumentException($"The map does not specify the starting conditions of Player {i}!");
            }
        }
    }

    public Coordinate GetPlayerSpawnPos(int playerId)
    {
        return this.playerStartPos[playerId];
    }
    public int GetPlayerSpawnLength(int playerId)
    {
        return this.playerStartLength[playerId];
    }
    public int GetPlayerSpawnDirection(int playerId)
    {
        return this.playerStartDirection[playerId];
    }

    public TileTypes.Tile this[int x, int y]
    {
        get
        {
            return map[x, y];
        }
    }

    public bool StarsCollected
    {
        get
        {
            return starsRemaining <= 0;
        }
    }

    private void ValidateParameterCount(string[] parameters, int expectedCount, int lineNumber)
    {
        if (parameters.Length != expectedCount) throw new ArgumentException($"Line {lineNumber}: Expected {expectedCount} parameters, got {parameters.Length} parameters");
    }

    private int ParseIntValue(string source, int lineNumber, bool mustBePositive = true)
    {
        if (int.TryParse(source, out int value))
        {
            if (mustBePositive && value < 0) throw new ArgumentException($"Line {lineNumber}: {value} needs be to be positive integer");
            return value;
        }
        else throw new ArgumentException($"Line {lineNumber}: Cannot parse {source} as integer");
    }

    private int ParseIntAttribute(string source, string expectedName)
    {
        string[] vars = source.Split(':');
        if (vars.Length != 2) throw new ArgumentException($"Error Parsing {source} for {expectedName}: Source must be in the format of [name]:[value]");
        if (vars[0] != expectedName) throw new ArgumentException($"Expected attribute name {expectedName}, got {vars[0]}");
        return int.Parse(vars[1]);
    }

    private float ParseFloatAttribute(string source, string expectedName)
    {
        string[] vars = source.Split(':');
        if (vars.Length != 2) throw new ArgumentException($"Error Parsing {source} for {expectedName}: Source must be in the format of [name]:[value]");
        if (vars[0] != expectedName) throw new ArgumentException($"Expected attribute name {expectedName}, got {vars[0]}");
        return float.Parse(vars[1]);
    }

    /// <summary>
    /// Activates a tile at a given location, which activates special effects related to the place
    /// </summary>
    public void Activate(int x, int y, PlayerController player)
    {
        // Die if out of bounds
        if (x < 0 || y < 0 || x >= horizontalSize || y >= verticalSize)
        {
            return;
        }
        if (map[x, y] == null)
        {
            return;
        }
        else
        {
            map[x, y].Activate(this, renderer, player);
        }
    }

    public Vector3 CoordinateToWorldPosition (Coordinate coordinate)
    {
        return new Vector3(coordinate.x - xOffset, coordinate.y - yOffset);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    Coordinate? aPosition;
    Coordinate? bPosition;
    int aLength;
    int bLength;

    private TileTypes.Tile[,] map;

    public LevelState(string[] file)
    {
        if (file[0] != "snake level format v1") throw new NotSupportedException("Only v1 format is supported");
        horizontalSize = ParseIntAttribute(file[1], "hSize");
        verticalSize = ParseIntAttribute(file[2], "vSize");
        tickSpeed = ParseFloatAttribute(file[3], "tickSpeed");

        map = new TileTypes.Tile[horizontalSize, verticalSize];

        bool parsingContent = false;
        for(int i = 0; i < file.Length; i++)
        {
            if (!parsingContent && file[i] == "[CONTENT]")
            {
                parsingContent = true;
            }
            else if (parsingContent && !string.IsNullOrWhiteSpace(file[i]) && !file[i].StartsWith("#"))
            {
                string[] p = file[i].Split(' ');
                switch(p[0])
                {
                    case "a":
                        if (aPosition.HasValue) throw new Exception("A postion was defined twice");
                        aPosition = new Coordinate(ParseIntValue(p[1], i), ParseIntValue(p[2], i));
                        aLength = ParseIntValue(p[3], i);
                        break;

                    case "b":
                        if (bPosition.HasValue) throw new Exception("B postion was defined twice");
                        bPosition = new Coordinate(ParseIntValue(p[1], i), ParseIntValue(p[2], i));
                        bLength = ParseIntValue(p[3], i);
                        break;

                    case "w":
                        int startX = ParseIntValue(p[1], i);
                        int startY = ParseIntValue(p[2], i);
                        int width = ParseIntValue(p[3], i);
                        int height = ParseIntValue(p[4], i);
                        for(int x = startX; x < startX + width; x++)
                        {
                            for (int y = startY; y < startY + height; y++)
                            {
                                if (x >= horizontalSize || y >= verticalSize) throw new Exception($"Line {i}: size exceeded the dimensions of the map");
                                map[x, y] = new TileTypes.Wall();
                            }
                        }
                        break;

                    case "s":
                        int starX = ParseIntValue(p[1], i);
                        int starY = ParseIntValue(p[2], i);
                        if (starX >= horizontalSize || starY > verticalSize) throw new Exception($"Line {i}: size exceeded the dimensions of the map");
                        map[starX, starY] = new TileTypes.Star()
                        {
                            color = p[3]
                        };
                        break;


                    default:
                        throw new Exception($"Line {i}: Unsupported Content Type {p[0]}");
                }
            }
        }

        if(!aPosition.HasValue || !bPosition.HasValue)
        {
            throw new ArgumentException("The map does not specify the starting conditions of A and B!");
        }
    }

    private int ParseIntValue (string source, int lineNumber, bool mustBePositive = true)
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

    private float ParseFloatAttribute (string source, string expectedName)
    {
        string[] vars = source.Split(':');
        if (vars.Length != 2) throw new ArgumentException($"Error Parsing {source} for {expectedName}: Source must be in the format of [name]:[value]");
        if (vars[0] != expectedName) throw new ArgumentException($"Expected attribute name {expectedName}, got {vars[0]}");
        return float.Parse(vars[1]);
    }
}

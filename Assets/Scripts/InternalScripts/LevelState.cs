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

    Coordinate aPosition;
    Coordinate bPosition;

    public LevelState(string[] file)
    {
        if (file[0] != "snake level format v1") throw new NotSupportedException("Only v1 format is supported");
        
    }

    private float ParseFloatAttribute (string source, string expectedName)
    {
        string[] vars = source.Split(':');
        if (vars.Length != 2) throw new ArgumentException("Source must be in the format of [name]:[value]");
        if (vars[0] != expectedName) throw new ArgumentException("Expected attribute name ");
        return float.Parse(vars[1]);
    }
}

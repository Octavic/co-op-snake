using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TileTypes
{
    public abstract class BaseTile
    {
        public abstract Coordinate Activate();
    }
}
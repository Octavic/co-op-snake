using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TileTypes
{
    public abstract class Tile
    {
        public abstract void Activate(PlayerController player);
    }

    public class Wall: Tile
    {
        public override void Activate (PlayerController player)
        {
            return;
        }
    }

    public class Star : Tile
    {
        public string color;
        public override void Activate(PlayerController player)
        {
            return;
        }
    }
}
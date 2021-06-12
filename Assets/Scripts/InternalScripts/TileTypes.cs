using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TileTypes
{
    public abstract class Tile
    {
        public abstract void Activate(LevelState level, PlayerController player);
    }

    public class Wall: Tile
    {
        public override void Activate (LevelState level, PlayerController player)
        {
            GameController.staticInstance.OnGameOver();
            return;
        }
    }

    public class Star : Tile
    {
        public int color;
        private bool collected = false;
        public override void Activate(LevelState level, PlayerController player)
        {
            if (!collected)
            {
                collected = true;
                level.starsRemaining--;
            }
            return;
        }
    }
}
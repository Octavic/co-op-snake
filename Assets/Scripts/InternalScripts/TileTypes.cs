using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TileTypes
{
    public abstract class Tile
    {
        public Coordinate coordinate;

        public abstract void Activate(LevelState level, RenderScript renderer, PlayerController player);
    }

    public class Wall: Tile
    {
        public override void Activate (LevelState level, RenderScript renderer, PlayerController player)
        {
            GameController.staticInstance.OnGameOver();
            return;
        }
    }

    public class Star : Tile
    {
        public int ownerPlayerId;
        private bool collected = false;
        public override void Activate(LevelState level, RenderScript renderer, PlayerController player)
        {
            if (!collected && player.playerIdentifier == ownerPlayerId)
            {
                collected = true;
                level.starsRemaining--;
                player.PendingPieces++;
                renderer.SetToEmpty(level, coordinate);
            }
            return;
        }
    }
}
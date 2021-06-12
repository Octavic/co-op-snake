using UnityEngine;

public abstract class SnakeGridTile : MonoBehaviour
{
    public Coordinate Coordinate
    {
        get
        {
            return this._coordinate;
        }
        set
        {
            this._coordinate = value;
            var parentGrid = this.GetComponentInParent<SnakeGrid>();
            if (parentGrid == null)
            {
                Debug.LogError("Unable to find parent grid for " + this.gameObject.name);
            }
            else
            {
                this.transform.position = new Vector2(value.x, value.y) * parentGrid.GridSize;
            }
        }
    }
    private Coordinate _coordinate;

    /// <summary>
    /// Called when the player's head tile collides with this tile
    /// </summary>
    public abstract void OnPlayerHeadEnter(PlayerController enteredPlayer);
}

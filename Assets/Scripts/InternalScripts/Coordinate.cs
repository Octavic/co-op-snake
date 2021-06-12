/// <summary>
/// Defines a x,y coordinate on a 2D grid
/// </summary>
public struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Coordinate operator +(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x + b.x, a.y + b.y);
    }

    public static bool operator ==(Coordinate a, Coordinate b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Coordinate a, Coordinate b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override bool Equals(object obj)
    {
        var coordinate = obj as Coordinate?;
        if (coordinate == null)
        {
            return base.Equals(obj);
        }

        return this == coordinate;
    }

    public override int GetHashCode()
    {
        return this.y * 1000 + this.x;
    }
}

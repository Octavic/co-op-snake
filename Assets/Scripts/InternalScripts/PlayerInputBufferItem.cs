using UnityEngine;

public struct PlayerInputBufferItem
{
    public int KeycodeId;
    public float Time;

    public PlayerInputBufferItem(int keycodeId, float time)
    {
        this.KeycodeId = keycodeId;
        this.Time = time;
    }
}

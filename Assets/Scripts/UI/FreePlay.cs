using System.Collections;
using UnityEngine;

public class FreePlay : MonoBehaviour
{
    public float TickSpeed;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(this.EndSceneFreePlay());
    }

    IEnumerator EndSceneFreePlay()
    {
        var players = GameObject.FindObjectsOfType<PlayerController>();
        while (true)
        {
            foreach (var player in players)
            {
                player.GameUpdate();
            }

            yield return new WaitForSeconds(this.TickSpeed);
        }
    }
}

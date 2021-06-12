using System.Linq;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var player in GameObject.FindObjectsOfType<PlayerController>())
            {
                player.GameUpdate();
            }
        }
    }
}

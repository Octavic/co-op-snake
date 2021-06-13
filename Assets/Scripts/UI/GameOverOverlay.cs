using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverOverlay : MonoBehaviour
{
    public Text ScoreText;
    public void Show(float score)
    {
        this.gameObject.SetActive(true);
        this.ScoreText.text = "SCORE: " + score.ToString();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

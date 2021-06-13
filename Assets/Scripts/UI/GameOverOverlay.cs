using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverOverlay : BaseOverlay
{
    public Text ScoreText;
    public void SetScore(float score)
    {
        this.ScoreText.text = "SCORE: " + ((int)score).ToString();
    }
}

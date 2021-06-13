using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownOverlay : BaseOverlay
{
    public Text OverlayText;
    
    public void ChangeText(string text)
    {
        this.OverlayText.text = text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

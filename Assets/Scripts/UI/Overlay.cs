using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    public Text OverlayText;
    
    // Use this for initialization
    void Start()
    {
    }

    /// <summary>
    /// Shows the overlay
    /// </summary>
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    /// <summary>
    /// Hides the overlay
    /// </summary>
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void ChangeText(string text)
    {
        this.OverlayText.text = text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using UnityEngine;

public abstract class BaseOverlay : MonoBehaviour
{
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

}

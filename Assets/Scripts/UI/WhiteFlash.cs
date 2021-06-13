using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WhiteFlash : MonoBehaviour
{
    public float CurrentAlpha
    {
        get
        {
            return this.GetComponent<Image>().color.a;
        }
        set
        {
            this.GetComponent<Image>().color = new Color(1, 1, 1, value);
        }
    }
    public float FadeRatio;

    public void Flash()
    {
        this.CurrentAlpha = 1;
    }

    void Update()
    {
        this.CurrentAlpha *= this.FadeRatio;
    }
}

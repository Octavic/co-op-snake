using UnityEngine;
using UnityEngine.UI;

public class FadeBlack : MonoBehaviour
{
    public float TargetAlpha;
    public float CurrentAlpha
    {
        get
        {
            return this.GetComponent<Image>().color.a;
        }
        set
        {
            this.GetComponent<Image>().color = new Color(0, 0, 0, value);
        }
    }
    public float FadeVelocity;

    private void Update()
    {
        var diff = this.TargetAlpha - this.CurrentAlpha;
        var movement = this.FadeVelocity * Time.deltaTime;
        if (Mathf.Abs(diff) < movement)
        {
            this.CurrentAlpha = this.TargetAlpha;
        }
        else
        {
            this.CurrentAlpha += movement * Mathf.Sign(diff);
        }
    }

    public void ToWhite()
    {
        this.TargetAlpha = 0;
    }
    public void ToBlack()
    {
        this.TargetAlpha = 1;
    }
}

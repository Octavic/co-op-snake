using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Color[] portalColors;
    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;
    public float cycleTime = 2f;

    private float clock = 0f;

    private SpriteRenderer spriteRender;
    private Color currentColor;
    private bool initialized;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            clock += Time.deltaTime;
            float cycle = (clock % cycleTime) / cycleTime * 2;
            if (cycle > 1f)
            {
                cycle = 2f - cycle;
            }
            currentColor.a = (maxAlpha - minAlpha) * cycle + minAlpha;
            spriteRender.color = currentColor;
        }
    }

    public void AssignColor (int index)
    {
        this.spriteRender = this.GetComponent<SpriteRenderer>();
        if (index < 0 || index >= portalColors.Length)
        {
            throw new System.Exception($"Cannot find portal color with index {index}");
        }
        currentColor = portalColors[index];
        spriteRender.color = currentColor;
        this.initialized = true;
    }
}

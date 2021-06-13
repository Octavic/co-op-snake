using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Color[] portalColors;
    public float minAlpha = 0.7f;
    public float maxAlpha = 0.8f;
    public float cycleTime = 1f;

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
        clock += Time.deltaTime;
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
    }
}

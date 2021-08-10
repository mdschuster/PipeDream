using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleHue : MonoBehaviour
{

    //[SerializeField] private Color startColor;
    [SerializeField] private float cycleSpeed;
    private float time;
    private Color currentColor;

    private MeshRenderer meshRenderer;

    private Material material;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        currentColor = getColor();
        if (cycleSpeed == 0) cycleSpeed = 1;
        time = cycleSpeed / 100;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        if (time <= 0)
        {
            float H, S, V;
            Color.RGBToHSV(currentColor, out H, out S, out V);
            H *= 360;
            H += 1;
            if (H == 360) H = 0;
            currentColor = Color.HSVToRGB(H / 360, S, V);
            setColor(currentColor);
            time = cycleSpeed / 100;
        }
        time -= Time.deltaTime;
    }

    public Color getColor()
    {
        return material.GetColor("maincolor");
    }

    public void setColor(Color color)
    {
        material.SetColor("maincolor", color);
    }

}

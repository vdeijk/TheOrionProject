using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BorderEffectMonobService : MonoBehaviour
{
    [Header("Border Settings")]
    public Image borderImage;
    public Color baseColor = new Color(0, 0.8f, 1f);

    [Header("Glow Effect")]
    public float glowSpeed = 1.5f;
    public float minGlowIntensity = 0.7f;
    public float maxGlowIntensity = 1.5f;

    void Start()
    {
        // Auto-find the image if not set
        if (borderImage == null)
            borderImage = GetComponent<Image>();

        StartCoroutine(GlowEffect());
    }

    IEnumerator GlowEffect()
    {
        while (true)
        {
            // Create smooth sine wave oscillation
            float t = (Mathf.Sin(Time.time * glowSpeed) + 1) / 2;
            float intensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, t);

            // Apply to border color
            borderImage.color = new Color(
                baseColor.r * intensity,
                baseColor.g * intensity,
                baseColor.b * intensity,
                baseColor.a
            );

            yield return null;
        }
    }
}
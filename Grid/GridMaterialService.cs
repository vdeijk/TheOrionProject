using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    public class GridMaterialService : Singleton<GridMaterialService>
    {
        [SerializeField] GameDurations gameDurations;

        // Smoothly fades mesh renderer's material alpha to target value over time
        public IEnumerator Lerp(float target, MeshRenderer meshRenderer)
        {
            if (!meshRenderer.enabled)
            {
                Color color = meshRenderer.material.color;
                color.a = 0;
                meshRenderer.material.color = color;
                meshRenderer.enabled = true;
            }

            float elapsed = 0f;
            Color startColor = meshRenderer.material.color;
            float startAlpha = startColor.a;

            while (elapsed < gameDurations.materialFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / gameDurations.materialFadeDuration);
                Color color = meshRenderer.material.color;
                color.a = Mathf.Lerp(startAlpha, target, t);
                meshRenderer.material.color = color;
                yield return null;
            }

            Color finalColor = meshRenderer.material.color;
            finalColor.a = target;
            meshRenderer.material.color = finalColor;

            // Disable renderer if fully transparent
            if (target == 0)
            {
                meshRenderer.enabled = false;
            }
        }
    }
}

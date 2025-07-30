using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SciFiEdgeEffect : MonoBehaviour
{
    [Header("Line Settings")]
    public float lineWidth = 0.1f;
    public Color lineColor = Color.blue;

    [Header("Position Adjustments")]
    [Tooltip("Vertical distance above terrain")]
    public float yOffset = 0.1f;
    [Tooltip("Height variation to create wave effect")]
    public float waveHeight = 0.1f;
    [Tooltip("Distance outside terrain boundary")]
    public float edgePadding = 0f;

    [Header("Animation")]
    public bool animateWidth = true;
    public float pulseWidthAmount = 0.03f;
    public float pulseWidthSpeed = 1f;

    [HideInInspector] public Vector3 terrainSize;
    [HideInInspector] public Vector3 terrainCenter;

    private LineRenderer lineRenderer;
    private int pointsPerSide = 10;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupLineRenderer(); // Configure material, width, and texture
    }

    private void Update()
    {
        // Animate line width for pulsing effect
        if (animateWidth)
        {
            float widthPulse = Mathf.Sin(Time.time * pulseWidthSpeed) * pulseWidthAmount;
            lineRenderer.startWidth = lineWidth + widthPulse;
            lineRenderer.endWidth = lineWidth + widthPulse;
        }
    }

    // Sets up the line renderer's material and gradient texture
    private void SetupLineRenderer()
    {
        if (lineRenderer.material == null)
        {
            Material newMat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            if (newMat == null)
            {
                newMat = new Material(Shader.Find("Particles/Standard Unlit"));
            }

            newMat.SetColor("_BaseColor", lineColor);
            newMat.SetColor("_EmissionColor", lineColor * 2f);
            newMat.EnableKeyword("_EMISSION");

            newMat.SetFloat("_Surface", 1); // Transparent
            newMat.SetFloat("_Blend", 1);   // Alpha blend
            newMat.renderQueue = 3000;       // Render after opaque

            lineRenderer.material = newMat;
        }

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.loop = true;

        ApplyGradientTexture(); // Adds a soft edge gradient to the line
    }

    // Creates and applies a gradient alpha texture for the line
    private void ApplyGradientTexture()
    {
        Texture2D texture = new Texture2D(256, 8);
        for (int x = 0; x < texture.width; x++)
        {
            float t = x / (float)texture.width;
            float intensity = 1f - Mathf.Abs(t - 0.5f) * 2f;
            intensity = Mathf.Pow(intensity, 0.5f); // Soft falloff
            Color color = new Color(1, 1, 1, intensity);
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        lineRenderer.material.mainTexture = texture;
    }

    // Updates the boundary and material color/emission
    public void UpdateBoundary()
    {
        GenerateDetailedTerrainBoundary(pointsPerSide);
        if (lineRenderer.material != null)
        {
            lineRenderer.material.SetColor("_BaseColor", lineColor);
            lineRenderer.material.SetColor("_EmissionColor", lineColor * 2f);
        }
    }

    // Generates a wavy, rectangular boundary around terrain using multiple points per side
    public void GenerateDetailedTerrainBoundary(int pointsPerSide = 10)
    {
        this.pointsPerSide = pointsPerSide;
        if (terrainSize == Vector3.zero)
        {
            Debug.LogError("Terrain size not set!");
            return;
        }
        float halfWidth = (terrainSize.x / 2) + edgePadding;
        float halfLength = (terrainSize.z / 2) + edgePadding;
        lineRenderer.positionCount = pointsPerSide * 4 + 1;
        int pointIndex = 0;
        // Four sides of the rectangle, each with wave effect
        for (int i = 0; i < pointsPerSide; i++)
        {
            float t = i / (float)(pointsPerSide - 1);
            float x = Mathf.Lerp(-halfWidth, halfWidth, t);
            float z = -halfLength;
            float verticalPosition = terrainCenter.y + yOffset;
            float waveEffect = waveHeight * Mathf.Sin(t * Mathf.PI * 2);
            float y = verticalPosition + waveEffect;
            Vector3 position = new Vector3(
                terrainCenter.x + x,
                y,  
                terrainCenter.z + z
            );
            lineRenderer.SetPosition(pointIndex++, position);
        }
        for (int i = 0; i < pointsPerSide; i++)
        {
            float t = i / (float)(pointsPerSide - 1);
            float x = halfWidth;
            float z = Mathf.Lerp(-halfLength, halfLength, t);
            float verticalPosition = terrainCenter.y + yOffset;
            float waveEffect = waveHeight * Mathf.Sin((t + 0.25f) * Mathf.PI * 2);
            float y = verticalPosition + waveEffect;
            Vector3 position = new Vector3(
                terrainCenter.x + x,
                y,
                terrainCenter.z + z
            );
            lineRenderer.SetPosition(pointIndex++, position);
        }
        for (int i = 0; i < pointsPerSide; i++)
        {
            float t = i / (float)(pointsPerSide - 1);
            float x = Mathf.Lerp(halfWidth, -halfWidth, t);
            float z = halfLength;
            float verticalPosition = terrainCenter.y + yOffset;
            float waveEffect = waveHeight * Mathf.Sin((t + 0.5f) * Mathf.PI * 2);
            float y = verticalPosition + waveEffect;
            Vector3 position = new Vector3(
                terrainCenter.x + x,
                y,
                terrainCenter.z + z
            );
            lineRenderer.SetPosition(pointIndex++, position);
        }
        for (int i = 0; i < pointsPerSide; i++)
        {
            float t = i / (float)(pointsPerSide - 1);
            float x = -halfWidth;
            float z = Mathf.Lerp(halfLength, -halfLength, t);
            float verticalPosition = terrainCenter.y + yOffset;
            float waveEffect = waveHeight * Mathf.Sin((t + 0.75f) * Mathf.PI * 2);
            float y = verticalPosition + waveEffect;
            Vector3 position = new Vector3(
                terrainCenter.x + x,
                y,
                terrainCenter.z + z
            );
            lineRenderer.SetPosition(pointIndex++, position);
        }
        // Close the loop
        lineRenderer.SetPosition(pointIndex, lineRenderer.GetPosition(0));
    }
}
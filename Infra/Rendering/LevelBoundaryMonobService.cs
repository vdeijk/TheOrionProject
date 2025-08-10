using UnityEngine;

// Handles creation and animation of visual boundary effects around the terrain
public class LevelBoundaryEffectMonobService : MonoBehaviour
{
    [System.Serializable]
    public class EdgeLayer
    {
        public GameObject linePrefab;
        [Tooltip("Distance outside terrain boundary")]
        public float padding = 0f;
        [Tooltip("Vertical distance above terrain")]
        public float height = 0f;
        [Tooltip("Height variation to create wave effect")]
        public float waveHeight = 0.1f;
        public Color color = Color.blue;
        public bool animatePadding = false;
        public float paddingSpeed = 0.5f;
        public float paddingAmount = 0.5f;
    }

    public MeshRenderer terrainMesh;
    public EdgeLayer[] edgeLayers;
    [Tooltip("Base height above terrain")]
    public float baseHeight = 0.1f;

    private SciFiEdgeEffectMonobService[] edgeEffects;

    private void Start()
    {
        if (terrainMesh == null)
        {
            return;
        }

        CreateEdgeLayers();
    }

    private void Update()
    {
        // Animate padding for edge layers if enabled
        if (edgeLayers != null && edgeEffects != null)
        {
            for (int i = 0; i < edgeLayers.Length; i++)
            {
                if (i < edgeEffects.Length && edgeLayers[i].animatePadding)
                {
                    float paddingOffset = Mathf.Sin(Time.time * edgeLayers[i].paddingSpeed) *
                                          edgeLayers[i].paddingAmount;

                    edgeEffects[i].edgePadding = edgeLayers[i].padding + paddingOffset;
                    edgeEffects[i].UpdateBoundary();
                }
            }
        }
    }

    // Instantiates edge effect objects for each layer and configures their properties
    private void CreateEdgeLayers()
    {
        if (edgeLayers == null || edgeLayers.Length == 0)
            return;

        Bounds terrainBounds = terrainMesh.bounds;

        edgeEffects = new SciFiEdgeEffectMonobService[edgeLayers.Length];

        for (int i = 0; i < edgeLayers.Length; i++)
        {
            var layer = edgeLayers[i];

            if (layer.linePrefab == null)
            {
                continue;
            }

            GameObject edgeObj = Instantiate(layer.linePrefab, transform);
            edgeObj.transform.position = transform.position;

            SciFiEdgeEffectMonobService edgeEffect = edgeObj.GetComponent<SciFiEdgeEffectMonobService>();
            if (edgeEffect != null)
            {
                edgeEffects[i] = edgeEffect;

                edgeEffect.terrainSize = terrainBounds.size;
                edgeEffect.terrainCenter = terrainBounds.center;
                edgeEffect.edgePadding = layer.padding;
                edgeEffect.yOffset = baseHeight + layer.height;
                edgeEffect.waveHeight = layer.waveHeight;
                edgeEffect.lineColor = layer.color;

                edgeEffect.GenerateDetailedTerrainBoundary(20);
            }
        }
    }

    // Regenerates all edge layers, useful for editor changes
    public void RegenerateAllLayers()
    {
        if (edgeEffects == null)
            return;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        CreateEdgeLayers();
    }

    // Draws gizmos for terrain and edge layers in the editor for visualization
    private void OnDrawGizmosSelected()
    {
        if (terrainMesh == null)
            return;

        Bounds bounds = terrainMesh.bounds;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (edgeLayers != null)
        {
            for (int i = 0; i < edgeLayers.Length; i++)
            {
                EdgeLayer layer = edgeLayers[i];

                Vector3 expandedSize = new Vector3(
                    bounds.size.x + layer.padding * 2,
                    bounds.size.y,
                    bounds.size.z + layer.padding * 2
                );

                Gizmos.color = layer.color;
                Vector3 center = new Vector3(
                    bounds.center.x,
                    bounds.center.y + baseHeight + layer.height,
                    bounds.center.z
                );
                Gizmos.DrawWireCube(center, expandedSize);
            }
        }
    }
}
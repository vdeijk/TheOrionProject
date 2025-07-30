using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class GridSquareData : ScriptableObject
    {
        public Transform[] prefabTransforms;
        public Texture2D noiseMapTexture;
        public float density;
        public float XZRandomness;
        public float YRandomnessMin;
        public float YRandomnessMax;
        public float maxSlope;
        public float maxHeight;
        public float noiseScale;
        public float minScale;
        public float maxScale;
        public int minSpawns;
        public int maxSpawns;
        public float minDistBetweenSpawns;
        public string description;
    }
}

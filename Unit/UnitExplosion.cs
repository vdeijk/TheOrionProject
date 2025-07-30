using System;
using System.Collections;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles explosion effects and audio for a unit
    public class UnitExplosion : MonoBehaviour
    {
        [SerializeField] Unit unit;
        [SerializeField] Transform explosionTransform;
        [SerializeField] Transform explosionAudioPrefab;
        [SerializeField] Transform[] explosionPrefabs;
        [SerializeField] AudioClip[] explosionAudioClips;

        // Triggers explosion visuals and audio at the unit's position
        public void ExplodeUnit()
        {
            float explosionScale = SetExplosionScale();
            Transform explosionPrefab = explosionPrefabs[UnityEngine.Random.Range(0, explosionPrefabs.Length)];
            explosionPrefab.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
            SetAudioClip();
            Instantiate(explosionPrefab, explosionTransform.position, transform.rotation);
            Instantiate(explosionAudioPrefab, explosionTransform.position, transform.rotation);
        }

        // Randomly selects an audio clip for the explosion
        private void SetAudioClip()
        {
            int number = UnityEngine.Random.Range(0, explosionAudioClips.Length);
            explosionAudioPrefab.GetComponent<AudioSource>().clip = explosionAudioClips[number];
        }

        // Calculates explosion scale based on unit's collider size
        private float SetExplosionScale()
        {
            BoxCollider boxCollider = unit.GetComponent<BoxCollider>();
            return .6f * ((boxCollider.size.x + boxCollider.size.y + boxCollider.size.z) / 3);
        }
    }
}
using UnityEngine;

namespace TurnBasedStrategy
{
    public class AudioSourceLifetime : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            // Destroy this object when audio finishes playing
            if (audioSource.isPlaying == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class SpawningAudioSourceController : MonoBehaviour
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
using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy.Infra
{
    public class AudioFadeMonobService : SingletonBaseService<AudioFadeMonobService>
    {
        // Fades the volume of an AudioSource from intialValue to targetValue over duration
        public IEnumerator Fade(float intialValue, float targetValue, AudioSource audioSource, float duration)
        {
            float time = 0;

            if (targetValue == 1)
            {
                audioSource.Play();
            }

            while (time < duration)
            {
                audioSource.volume = Mathf.SmoothStep(intialValue, targetValue, time / duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            audioSource.volume = targetValue;

            if (targetValue == 0)
            {
                audioSource.Stop();
            }
        }
    }
}


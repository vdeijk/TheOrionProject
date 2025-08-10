using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Base class for objects that should be automatically despawned after a set lifetime
    public abstract class SpawningDespawnController : MonoBehaviour
    {
        // Lifetime in seconds before despawning; override in derived classes for custom logic
        public virtual float lifetime { get; protected set; } = 0;

        // Removes the object from the scene; can be overridden for custom cleanup
        public virtual void Despawn()
        {
            Destroy(gameObject);
        }

        // Coroutine that waits for the object's lifetime before despawning
        protected IEnumerator DespawnAfterLifetimeEnds()
        {
            yield return new WaitForSeconds(lifetime);
            Despawn();
        }
    }
}

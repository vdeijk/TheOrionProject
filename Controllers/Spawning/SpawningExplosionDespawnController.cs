using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Handles despawning of explosion effects after their particle system duration
    public class SpawningExplosionDespawnController : SpawningDespawnController
    {
        [SerializeField] ParticleSystem ps;

        // Uses the duration of the particle system as the object's lifetime
        public override float lifetime => ps.main.duration;

        private void OnEnable()
        {
            // Start coroutine to despawn after the particle system finishes
            StartCoroutine(DespawnAfterLifetimeEnds());
        }

        // Removes the explosion object from the scene
        public override void Despawn()
        {
            Destroy(gameObject);
        }
    }
}


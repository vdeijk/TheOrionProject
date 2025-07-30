using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles bullet projectile movement and collision
    public class BulletProjectile : Projectile
    {
        [SerializeField] float MoveSpeed;
        [SerializeField] LayerMask CollisionLayers;

        // Moves the bullet and checks for collision each frame
        private void Update()
        {
            float stepDistance = MoveSpeed * Time.deltaTime;
            Vector3 moveDir = (targetPos - transform.position).normalized;

            if (Physics.Raycast(transform.position, moveDir, out RaycastHit hit, stepDistance, CollisionLayers))
            {
                if (hit.collider)
                {
                    targetUnit = hit.collider.GetComponent<Unit>();
                    if (targetUnit != null) Damage();
                }

                ProjectileHitService.Instance.CreateBulletHit(transform.position);
                ProjectileAudioService.Instance.CreateHitClip(unit, hit.point);

                Destroy(gameObject);
            }
            else
            {
                transform.position += moveDir * stepDistance;
                transform.forward = moveDir;
            }
        }
    }
}


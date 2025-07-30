using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles laser projectile visual effects and damage logic
    public class LaserProjectile : Projectile
    {
        private enum BeamState
        {
            FadingIn,
            Active,
            FadingOut
        }

        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float startWidth = 0.2f;
        [SerializeField] private float endWidth = 0.1f;
        [SerializeField] private Color laserColor = Color.cyan;
        [SerializeField] float fadeDuration = 0.3f;
        [SerializeField] float beamCycleDuration = .9f;
        [SerializeField] float widthVariation = 0.4f;
        [SerializeField] float intensityVariation = 0.4f;
        [SerializeField] private Material laserMaterial;

        private float timer = 0f;
        private BeamState curBeamState = BeamState.FadingIn;
        private AnimationCurve widthCurve;
        private AnimationCurve intensityCurve;
        private Material materialInstance;

        // Initializes curves and material for the laser
        private void Awake()
        {
            lineRenderer.enabled = false;

            widthCurve = new AnimationCurve();
            widthCurve.AddKey(new Keyframe(0f, 1f - widthVariation));
            widthCurve.AddKey(new Keyframe(0.5f, 1f + widthVariation));
            widthCurve.AddKey(new Keyframe(1f, 1f - widthVariation));

            intensityCurve = new AnimationCurve();
            intensityCurve.AddKey(new Keyframe(0f, 1f - intensityVariation));
            intensityCurve.AddKey(new Keyframe(0.5f, 1f));
            intensityCurve.AddKey(new Keyframe(1f, 1f - intensityVariation));

            if (laserMaterial != null)
            {
                Material materialInstance = new Material(laserMaterial);
                lineRenderer.material = materialInstance;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ShootAction.OnStoppedShooting += ShootAction_OnStoppedShooting;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ShootAction.OnStoppedShooting -= ShootAction_OnStoppedShooting;
        }

        // Handles laser beam state transitions
        private void Update()
        {
            switch (curBeamState)
            {
                case BeamState.FadingIn:
                    HandleFadeIn();
                    break;
                case BeamState.FadingOut:
                    HandleFadeOut();
                    break;
                case BeamState.Active:
                    HandleBeam();
                    break;
            }

            timer += Time.deltaTime;
        }

        // Sets up the laser beam between source and target
        public override void Setup(Unit unit, Unit targetUnit)
        {
            this.unit = unit;
            this.targetUnit = targetUnit;

            Vector3 startPosition = transform.position;
            Vector3 direction = (targetUnit.transformData[PartType.Torso].position - startPosition).normalized;
            float maxDistance = Vector3.Distance(startPosition, targetUnit.transformData[PartType.Torso].position) + 5f;
            Vector3 hitPoint;
            Unit hitUnit = null;

            if (Physics.Raycast(startPosition, direction, out RaycastHit hitInfo, maxDistance))
            {
                hitPoint = hitInfo.point;
                targetPos = hitPoint;

                hitUnit = hitInfo.collider.GetComponent<Unit>();
            }
            else
            {
                hitPoint = targetUnit.transformData[PartType.Torso].position;
                targetPos = hitPoint;
            }

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetPos);
            SetWidth(0, 0);

            materialInstance = new Material(laserMaterial);
            lineRenderer.material = materialInstance;
            materialInstance.color = SetColor(0f);

            lineRenderer.enabled = true;

            this.targetUnit = hitUnit != null ? hitUnit : targetUnit;
        }

        // Handles transition to fade out when shooting stops
        private void ShootAction_OnStoppedShooting(object sender, System.EventArgs e)
        {
            timer = 0f;
            curBeamState = BeamState.FadingOut;
        }

        // Handles fade-in effect for the laser
        private void HandleFadeIn()
        {
            if (timer >= fadeDuration)
            {
                SetWidth(startWidth, endWidth);
                materialInstance.color = SetColor(1f);

                curBeamState = BeamState.Active;
                timer = 0;

                Vector3 targetPos = lineRenderer.GetPosition(1);
                ProjectileHitService.Instance.CreateBeamhit(targetPos);

                if (targetUnit != null && targetUnit != unit)
                {
                    Damage();
                }

                return;
            }

            float fadeInFactor = timer / fadeDuration;

            SetWidth(startWidth * fadeInFactor, endWidth * fadeInFactor);
            materialInstance.color = SetColor(fadeInFactor);
        }

        // Handles fade-out effect for the laser
        private void HandleFadeOut()
        {
            if (timer > fadeDuration)
            {
                lineRenderer.enabled = false;
                Destroy(gameObject);

                return;
            }

            float fadeOutFactor = 1f - (timer / fadeDuration);

            SetWidth(startWidth * fadeOutFactor, endWidth * fadeOutFactor);
            materialInstance.color = SetColor(fadeOutFactor);
        }

        // Handles active beam animation (width/intensity variation)
        private void HandleBeam()
        {
            float normalizedTime = (timer % beamCycleDuration) / beamCycleDuration;

            float widthFactor = widthCurve.Evaluate(normalizedTime);
            SetWidth(startWidth * widthFactor, endWidth * widthFactor);

            float intensityFactor = intensityCurve.Evaluate(normalizedTime);
            materialInstance.color = SetColor(intensityFactor);
        }

        // Returns the color for the laser beam
        private Color SetColor(float alpha)
        {
            Color glowColor = laserColor * 2.5f;
            glowColor.a = alpha;
            return glowColor;
        }

        // Sets the start and end width of the laser beam
        private void SetWidth(float startWidth, float endWidth)
        {
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
        }
    }
}


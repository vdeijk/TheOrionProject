using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Infra
{
    public class UnitMonobService : SingletonBaseService<UnitMonobService>
    {
        private UnitMaterialData MaterialData => UnitMaterialService.Instance.Data;


        // Coroutine for spawning unit with fade-in effect
        public IEnumerator SpawnUnit(UnitSingleController unit, SpawnBaseData data)
        {
            Renderer[] renderers = UnitMaterialService.Instance.GetAllRenderers(unit.Data.UnitBodyTransform);
            Material[] spawnMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                spawnMaterials[i] = new Material(data.SpawnMaterial);
                spawnMaterials[i].SetFloat("_SpawnProgress", 0f);
                renderers[i].material = spawnMaterials[i];
            }
            float elapsed = 0f;
            while (elapsed < DurationData.Instance.UnitSpawnDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / DurationData.Instance.UnitSpawnDuration);
                foreach (var mat in spawnMaterials)
                {
                    mat.SetFloat("_SpawnProgress", progress);
                }
                yield return null;
            }
            UnitMaterialService.Instance.SetMesh(unit, MaterialData.LightLayerUintWithHighlight);
        }

        public IEnumerator DelayDeath()
        {
            yield return new WaitForSecondsRealtime(DurationData.Instance.ExplosionDuration);

            TimeScaleManager.Instance.SetTimeScaleNormal();

            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            unit.Data.UnitMindTransform.GetComponent<UnitHealthController>().Die();
        }

        public UnitSingleController InstantiateUnit(SpawnBaseData data, Transform spawnPoint)
        {
            UnitSingleController unit = Instantiate(data.Prefab, spawnPoint.position, Quaternion.identity).GetComponent<UnitSingleController>();

            return unit;
        }

        public Transform InstantiatePart(Transform Prefab)
        {
            return Instantiate(Prefab, Vector3.zero, Quaternion.identity);
        }

        public void DestroyUnit(UnitSingleController unit)
        {
            Destroy(unit.gameObject);
        }
    }
}

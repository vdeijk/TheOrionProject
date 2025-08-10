using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    // Constructs unit parts and spawns units in the scene
    public class UnitConstructService
    {
        private static UnitConstructService _instance;

        public static UnitConstructService Instance => _instance ??= new UnitConstructService();

        // Builds the transform hierarchy for a unit's parts
        public Dictionary<PartType, Transform> GetTransformData(BaseSO baseData, TorsoSO torsoData, WeaponSO weaponPrimaryData, WeaponSO weaponSecondaryData, UnitSingleController unit)
        {
            Transform baseTransform = AddPart(baseData.Prefab, unit.Data.UnitBodyTransform, PartType.Base.ToString());
            Transform torsoTransform = AddPart(torsoData.Prefab, baseTransform.GetComponent<UnitSlotsController>().slots[0], PartType.Torso.ToString());
            Transform weaponPrimarytransform = AddPart(weaponPrimaryData.Prefab, torsoTransform.GetComponent<UnitSlotsController>().slots[0], PartType.WeaponPrimary.ToString());
            Transform weaponSecondarytransform = AddPart(weaponSecondaryData.Prefab, torsoTransform.GetComponent<UnitSlotsController>().slots[1], PartType.WeaponSecondary.ToString());

            return new Dictionary<PartType, Transform>
            {
                { PartType.Base, baseTransform},
                { PartType.Torso, torsoTransform},
                { PartType.WeaponPrimary, weaponPrimarytransform},
                { PartType.WeaponSecondary, weaponSecondarytransform },
            };
        }

        // Spawns units at specified spawn points
        public List<UnitSingleController> SpawnUnits(SpawnBaseData spawnData)
        {
            List<UnitSingleController> units = new List<UnitSingleController>();

            for (int i = 0; i < spawnData.Mechs.Count; i++)
            {
                Transform spawnPoint = spawnData.SpawnPoints[i];

                Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(spawnPoint.position);

                if (GridUnitService.Instance.GetUnit(gridPosition) != null) break;

                UnitSingleController unit = UnitMonobService.Instance.InstantiateUnit(spawnData, spawnPoint);

                MechsState mechData = spawnData.Mechs[i];
                BaseSO baseData = (BaseSO)mechData.mechData[PartType.Base];
                TorsoSO torsoData = (TorsoSO)mechData.mechData[PartType.Torso];
                WeaponSO weaponPrimaryData = (WeaponSO)mechData.mechData[PartType.WeaponPrimary];
                WeaponSO weaponSecondaryData = (WeaponSO)mechData.mechData[PartType.WeaponSecondary];

                Dictionary<PartType, Transform> transformData = GetTransformData(baseData, torsoData, weaponPrimaryData, weaponSecondaryData, unit);

                unit.Init(mechData.mechData, spawnData, transformData);

                GridUnitService.Instance.AddUnit(gridPosition, unit);

                units.Add(unit);
            }

            return units;
        }

        // Instantiates a part prefab and sets its tag and transform
        private Transform AddPart(Transform Prefab, Transform parent, string tagToSet)
        {
            Transform partTransform = UnitMonobService.Instance.InstantiatePart(Prefab);

            SetTagRecursively(partTransform.gameObject, tagToSet);

            partTransform.SetParent(parent);
            partTransform.localPosition = Vector3.zero;
            partTransform.localRotation = Quaternion.identity;
            partTransform.localScale = Vector3.one;

            return partTransform;
        }

        // Recursively sets the tag for a GameObject and its children
        private void SetTagRecursively(GameObject obj, string tag)
        {
            obj.tag = tag;
            foreach (Transform child in obj.transform)
            {
                SetTagRecursively(child.gameObject, tag);
            }
        }
    }
}



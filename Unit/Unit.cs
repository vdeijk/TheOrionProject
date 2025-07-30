using System;
using System.Collections.Generic;
using UnityEngine;
using static TurnBasedStrategy.UnitAssemblyService;

namespace TurnBasedStrategy
{
    // Represents a game unit and its parts
    public class Unit : MonoBehaviour
    {
        public Dictionary<PartType, PartData> partsData { get; private set; }
        public Dictionary<PartType, Transform> transformData { get; private set; }
        [field: SerializeField] public Transform unitEntityTransform { get; private set; }
        [field: SerializeField] public Transform unitMindTransform { get; private set; }
        [field: SerializeField] public Transform unitBodyTransform { get; private set; }

        // Replaces the unit's parts and transforms
        public void ReplaceParts(UnitPartsContext context)
        {
            partsData = context.PartsData;
            transformData = context.TransformData;
            // Creates a new render texture for the unit
            RenderTexture rt = new RenderTexture(256, 256, 24);
            rt.name = "RuntimeRenderTexture";
            rt.Create();
        }

        // Initializes the unit with parts, spawn data, and transforms
        public void Init(Dictionary<PartType, PartData> partsData, SpawnData spawnData,
            Dictionary<PartType, Transform> transformData)
        {
            this.partsData = partsData;
            this.transformData = transformData;
            transform.SetParent(spawnData.factionTransform);
            RenderTexture rt = new RenderTexture(256, 256, 24);
            rt.name = "RuntimeRenderTexture";
            rt.Create();
            UnitFaction unitFaction = unitEntityTransform.GetComponent<UnitFaction>();
            unitFaction.SetFaction(spawnData.factionSide);
        }
    }
}

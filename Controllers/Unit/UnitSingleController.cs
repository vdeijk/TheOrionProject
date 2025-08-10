using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Represents a game unit and its parts
    public class UnitSingleController : MonoBehaviour
    {
        [field: SerializeField] public UnitSingleData Data { get; private set; }

        // Replaces the unit's parts and transforms
        public void ReplaceParts(UnitPartsSettings context)
        {
            Data.PartsData = context.PartsData;
            Data.TransformData = context.TransformData;
            // Creates a new render texture for the unit
            RenderTexture rt = new RenderTexture(256, 256, 24);
            rt.name = "RuntimeRenderTexture";
            rt.Create();
        }

        // Initializes the unit with parts, spawn data, and transforms
        public void Init(Dictionary<PartType, PartSO> partsData, SpawnBaseData data,
            Dictionary<PartType, Transform> transformData)
        {
            Data.PartsData = partsData;
            Data.TransformData = transformData;
            transform.SetParent(data.FactionTransform);
            RenderTexture rt = new RenderTexture(256, 256, 24);
            rt.name = "RuntimeRenderTexture";
            rt.Create();
            UnitFactionController unitFaction = Data.UnitEntityTransform.GetComponent<UnitFactionController>();
            unitFaction.SetFaction(data.FactionSide);
        }
    }
}

/*
        /* public Dictionary<PartType, PartSO> partsData { get; private set; }
         public Dictionary<PartType, Transform> transformData { get; private set; }
         [field: SerializeField] public Transform unitEntityTransform { get; private set; }
         [field: SerializeField] public Transform unitMindTransform { get; private set; }
         [field: SerializeField] public Transform unitBodyTransform { get; private set; }*/

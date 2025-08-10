using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class PartHighlightController : MonoBehaviour
    {
        [field: SerializeField] public PartHighlightData Data { get; private set; }

        private void Awake()
        {
            PartHighlightService.Instance.Init(Data);
        }

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        public void DestroyHighlight()
        {
            Transform part = PartHighlightService.Instance.Data.HighlightedPart;
            Destroy(part?.gameObject);
        }

        public Transform Instantiate(UnitSingleController unit)
        {
            PartSO partData = unit.Data.PartsData[PartHighlightService.Instance.Data.CurPartType];
            return Instantiate(partData.Prefab);
        }

        // Removes highlight and restores renderers when leaving relevant menus
        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            MenuType prevMenu = MenuChangeMonobService.Instance.prevMenu;
            if (prevMenu == MenuType.Assemble || prevMenu == MenuType.Salvage)
            {
                DestroyHighlight();

                UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
                if (unit != null) PartHighlightService.Instance.SetRenderers(unit, true);
            }
        }
    }
}


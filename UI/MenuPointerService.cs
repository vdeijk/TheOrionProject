using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class MenuPointerService : Singleton<MenuPointerService>
    {
        private GraphicRaycaster[] raycasters;

        private void Start()
        {
            raycasters = FindObjectsByType<GraphicRaycaster>(FindObjectsSortMode.None);
        }

        public bool IsPointerOverWorldSpaceUI()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();

            foreach (var raycaster in raycasters)
            {
                if (raycaster.GetComponent<Canvas>().renderMode == RenderMode.WorldSpace)
                {
                    raycaster.Raycast(eventData, results);
                    if (results.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
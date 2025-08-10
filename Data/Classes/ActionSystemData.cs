using System.Collections.Generic;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Data
{
    [System.Serializable]
    public class ActionSystemData
    {
        public BaseActionData SelectedAction { get; set; }
        public ActionBaseService[] BaseActionServices { get; private set; } = new ActionBaseService[]
        {
            ActionMoveService.Instance,
            ActionShootService.Instance,
            ActionPassService.Instance,
        }; 

        // Returns the first active BaseActionService, or null if none are active
        public ActionBaseService IsActive
        {
            get
            {
                foreach (var service in BaseActionServices)
                {
                    var dataProp = GetDataProperty(service);
                    if (dataProp.IsActive)
                        return service;
                }
                return null;
            }
        }

        public MenuType MenuType => MenuChangeMonobService.Instance.curMenu;
        public bool IsInInvalidMenu => MenuType == MenuType.Repair || MenuType == MenuType.Assemble;

        private BaseActionData GetDataProperty(ActionBaseService service)
        {
            var type = service.GetType();
            var dataProp = type.GetProperty("Data");
            return dataProp?.GetValue(service) as BaseActionData;
        }
    }
}

/*

        // Helper to get the Data property from a BaseActionService (if present)
        private BaseActionData GetDataProperty(BaseActionService service)
        {
            var type = service.GetType();
            var dataProp = type.GetProperty("Data");
            return dataProp?.GetValue(service) as BaseActionData;
        }
        public BaseActionService IsSelected
        {
            get
            {
                foreach (var service in BaseActionServices)
                {
                    var dataProp = GetDataProperty(service);
                    if (dataProp.IsSelected)
                        return service;
                }
                return null;
            }
        }
        // Returns the first active BaseActionService, or null if none are active
        public BaseActionService IsActive
        {
            get
            {
                foreach (var service in BaseActionServices)
                {
                    var dataProp = GetDataProperty(service);
                    if (dataProp.IsActive)
                        return service;
                }
                return null;
            }
        }*/
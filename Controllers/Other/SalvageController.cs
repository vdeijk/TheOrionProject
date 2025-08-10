using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class SalvageController : MonoBehaviour
    {
        [field: SerializeField] public UnitSalvageData Data { get; private set; }

        private void Awake()
        {
            SalvageService.Instance.Init(Data);
        }

        public void HandleCoroutines()
        {
            StopAllCoroutines();
            StartCoroutine(UnitMonobService.Instance.DelayDeath());
        }
    }
}

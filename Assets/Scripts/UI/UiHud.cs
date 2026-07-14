using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Zoo
{
    public class UiHud : MonoBehaviour
    {
        public TMP_Text TotalEnemiesSpawned;
        public TMP_Text TotalEnemiesAlive;

        [Inject]
        private UnitService unitService;

        private void Start()
        {
            TotalEnemiesSpawned.text = "0";
            TotalEnemiesAlive.text = "0";
            unitService.OnUnitSpawnedEvent += UpdateCounter;
            unitService.OnUnitDiedEvent += UpdateCounter;
        }

        private void OnDestroy()
        {
            unitService.OnUnitSpawnedEvent -= UpdateCounter;
            unitService.OnUnitDiedEvent -= UpdateCounter;
        }

        private void UpdateCounter()
        {
            TotalEnemiesSpawned.text = unitService.UnitsSpawned.ToString();
            TotalEnemiesAlive.text = unitService.UnitsAlive.ToString();
        }
    }
}

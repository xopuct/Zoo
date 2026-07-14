using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Zoo
{
    public class UiHud : MonoBehaviour
    {
        public TMP_Text TotalUnitsSpawned;
        public TMP_Text TotalUnitsAlive;

        public TMP_Text TotalUnitsPreyKilled;
        public TMP_Text TotalUnitsPredatorKilled;

        [Inject]
        private UnitService unitService;

        private void Start()
        {
            TotalUnitsSpawned.text = "0";
            TotalUnitsAlive.text = "0";
            TotalUnitsPreyKilled.text = "0";
            TotalUnitsPredatorKilled.text = "0";
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
            TotalUnitsSpawned.text = unitService.UnitsSpawned.ToString();
            TotalUnitsAlive.text = unitService.UnitsAlive.ToString();
            TotalUnitsPreyKilled.text = unitService.UnitsPreyDeaths.ToString();
            TotalUnitsPredatorKilled.text = unitService.UnitsPredatorDeaths.ToString();
        }
    }
}

using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Zoo
{
    public class UiHud : MonoBehaviour
    {
        [FormerlySerializedAs("TotalEnemiesSpawned")]
        public TMP_Text TotalUnitsSpawned;
        [FormerlySerializedAs("TotalEnemiesAlive")]
        public TMP_Text TotalUnitsAlive;

        public TMP_Text TotalUnitsPreyKilled;
        public TMP_Text TotalUnitsPredatorKilled;

        [Inject]
        private UnitService unitService;

        private void Start()
        {
            TotalUnitsSpawned.text = "0";
            TotalUnitsAlive.text = "0";
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

using System;
using UnityEngine;

namespace Zoo
{
    [CreateAssetMenu(fileName = "Default.GameDefinition.asset", menuName = "Zoo/New Game Config")]
    public class GameDefinition : ScriptableObject
    {
        [Serializable]
        public struct AnimalSpawnTuple
        {
            public AnimalDefinition Animal;
            public float Weight;
        }

        public AnimalSpawnTuple[] Animals;
        public float AnimalSpawnFirstTime = 0;
        public float AnimalSpawnIntervalMin = 1;
        public float AnimalSpawnIntervalMax = 2;
    }
}

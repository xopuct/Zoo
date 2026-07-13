using Reflex.Core;
using UnityEngine;

namespace Zoo
{
    public class GameInstaller : MonoBehaviour, IInstaller
    {
        public GameDefinition Definition;
        public BoxCollider WorldArea;
        public LayerMask SpawnMask;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(Construct());
            new GameObject("Spawner", typeof(Spawner));
        }

        private GameService Construct()
        {
            return new GameService() { Definition = Definition, WorldArea = WorldArea, SpawnMask = SpawnMask, };
        }
    }
}

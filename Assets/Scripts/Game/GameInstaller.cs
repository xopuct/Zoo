using Reflex.Core;
using UnityEngine;

namespace Zoo
{
    public class GameInstaller : MonoBehaviour, IInstaller
    {
        public GameDefinition Definition;
        public BoxCollider WorldArea;
        public LayerMask SpawnMask;
        public LayerMask CollisionMask;
        public LayerMask GravityTestMask;
        public LayerMask UnitMask;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue(Construct());
            builder.RegisterValue(new CameraService { Camera = Camera.main });
            // Probably better to instantiate it in the scene
            new GameObject("Spawner", typeof(Spawner));
        }

        private GameService Construct()
        {
            return new GameService()
            {
                Definition = Definition,
                WorldArea = WorldArea,
                SpawnMask = SpawnMask,
                CollisionMask = CollisionMask,
                UnitMask = UnitMask,
                GravityTestMask = GravityTestMask
            };
        }
    }
}

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
            var gameService = Construct();
            builder.RegisterValue(gameService);
            builder.RegisterValue(new CameraService { Camera = Camera.main });
            builder.RegisterValue(new UnitService(gameService));
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

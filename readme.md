# Zoo

A small top-down 3D Unity game created as a technical assignment.

Animals periodically spawn inside the visible game area and move around the level using different movement behaviours. Predators can consume prey and other predators, while the UI displays gameplay statistics.

## Unity Version

Unity `6000.3.10f1`

## How to Run

1. Clone the repository.
2. Open the project in Unity Hub using Unity `6000.3.10f1`.
3. Wait until Unity restores all packages.
4. Open:

   `Assets/Scenes/SampleScene.unity`

5. Enter Play Mode.

No additional configuration is required.

## Architecture

The project uses service-based architecture with dependency injection provided by Reflex.

### `GameService`

Contains shared game configuration and world-related dependencies.

Responsibilities include:

- game definition access;
- world bounds;
- physics layer masks;
- kill event dispatching;
- runtime hierarchy folders.

### `UnitService`

Manages the lifecycle of animals.

Responsibilities include:

- unit creation;
- object pooling;
- unit activation and deactivation;
- alive and death statistics;
- returning dead units to their corresponding pools.

Pools are separated by `AnimalDefinition`, which prevents an object configured for one animal type from being reused as another type.

### `Spawner`

Controls when animals are spawned and where they appear.

The spawner:

1. selects an animal using its configured spawn weight;
2. searches for a valid position inside the camera viewport;
3. checks the position for overlaps;
4. uses a fallback strategy when no free position is found;
5. activates the unit only after its position has been assigned.

The final spawn attempt is intentionally reserved for fallback spawning.

The fallback strategy first places the animal on the highest available surface beneath the selected point. If no surface is found, the animal is spawned above the level and falls using physics.

### `AiController`

Provides simple roaming behaviour and handles animal interactions.

It is responsible for:

- selecting movement targets;
- returning animals toward the visible game area;
- reacting to collisions;
- resolving predator and prey interactions.

### Movement

Movement implementations follow the `IMovementController` contract.

Currently supported behaviours:

- `MovementJump`
- `MovementCrawl`

`MovementFactory` creates the appropriate controller based on the animal configuration.

### Pool Lifecycle

Components can implement:

- `IPoolObjectActivateHandler`
- `IPoolObjectDeactivateHandler`

This allows each component to independently reset its internal state when an object enters or leaves the pool.

Examples of reset state include:

- Rigidbody velocity;
- angular velocity;
- rotation;
- movement goal;
- grounded state;
- jump timer.

## Data Configuration

Animal behaviour is configured through ScriptableObjects.

### `AnimalDefinition`

Defines:

- animal name;
- visual prefab;
- health range;
- spawn weight;
- consumption type;
- predator rank;
- movement type;
- movement-specific settings.

### `GameDefinition`

Defines:

- available animals;
- spawn timing;
- spawn fallback height.

## Adding a New Animal

To add a new animal using an existing movement type:

1. Create a visual prefab containing a collider.
2. Create a new `AnimalDefinition`.
3. Assign the visual prefab.
4. Configure health, weight and consumption type.
5. Select a movement type and configure its parameters.
6. Add the animal to the active `GameDefinition`.

To add a new movement behaviour:

1. Implement `IMovementController`.
2. Add the corresponding movement configuration.
3. Register the implementation in `MovementFactory`.
4. Add any required pool activation or deactivation handlers.

## Dependencies

- Reflex — dependency injection
- Tri Inspector — improved Unity inspector tooling
- Universal Render Pipeline
- TextMesh Pro
- Unity uGUI

All dependencies are declared in `Packages/manifest.json`.

## Design Notes

The assignment focuses on architecture and extensibility rather than production graphics.

A lightweight MonoBehaviour-based approach was used instead of ECS, as requested.

Movement creation currently uses a central factory. This keeps the implementation explicit and simple for the scope of the assignment. For a significantly larger project, movement and interaction rules could be converted into polymorphic ScriptableObject strategies.

## Build

Tested successfully with:

- Unity Editor: `6000.3.10f1`


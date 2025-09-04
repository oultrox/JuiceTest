# Fruit Simulation Take-Home Project
![Fruits!](https://github.com/user-attachments/assets/d420913c-acc7-4cfb-afa0-254a341dca07)

---

## Overview

The Fruit Simulation project allows users to **pick, drag, and launch objects (fruits)** that respond to (rather jittery…) physics and play  some fun code-driven animations.

- Built in Unity3D `2022.3.45f1`
- Modular architecture using made-in-home dependency injection, event bus pattern and composition.
- Physics interactions handled with a custom **GravityMotor2D** engine
- Smooth drag input with velocity calculation for realistic object launches
- Some fun Warping effect for the custom feature I added.

---

## Features

- **Drag & Drop Mechanics:** Objects can be clicked and dragged naturally with smooth velocity tracking.
- **Custom Physics Engine based on raycasts:** Includes gravity, deceleration, and bounce factors for realistic fruit interactions. 0 Rigidbodies. I missed them.
- **Event-Driven System:** `ObjectPickedEvent` and `ObjectDroppedEvent` propagate interactions to all listening systems aiming to create decoupled systems or singleton nightmares.
- **Dynamic Animations:** Fruit stretches, squashes, and shrinks using the `FruitAnimator`. Tree reacts to fruit interactions.
- **Spawner System:** `EntitySpawner` generates fruits at specified spawn points with associated animations.
- Domain-driven directory favoring composition and the use of namespaces.

---

## Architecture

```
Assets
│
├─ AnimationService
│   ├─ AnimationService.cs       # Core animation handling (likely static utility)
│   └─ AnimatorService.cs        # Handles animator components and animation triggers
│
├─ CustomRaycastEngine
│   ├─ RaycastMotor.cs           # Base motor for raycast-based movement
│   └─ RaycastMotor2D.cs        # 2D-specific movement motor
│
├─ FruitSimulation
│   ├─ InputConfigs              # Input mappings
│   ├─ Prefabs                   # Fruit/tree prefabs
│   ├─ Scenes
│   ├─ SOs                       # ScriptableObjects for configs / data
│   ├─ Sprites                   # Fruit sprites
│   └─ Source
│       ├─ Controllers
│       │   ├─ DraggerController.cs         # Handles drag mechanics & input
│       │   ├─ FallingObjectController.cs  # Handles physics & fruit animations
│       │   └─ TreeController.cs           # Handles spawning & tree reactions
│       │
│       ├─ PhysicsMotor
│       │   └─ GravityMotor2D.cs           # Custom physics engine
│       │
│       ├─ Animators
│       │   ├─ FruitAnimator.cs
│       │   └─ TreeAnimator.cs
│       │
│       ├─ Entities
│       │   ├─ ObjectSpawner.cs            # Spawns fruits at designated positions
│       │   └─ ObjectDestroyer.cs          # Handles destruction/removal of objects
│       │
│       └─ Events
│           ├─ ObjectPickedEvent.cs
│           └─ ObjectDroppedEvent.cs
│
└─ RippleEffect
    ├─ RippleEffect.cs
    ├─ RippleEffectMaterial.mat   # If this is a material
    └─ RippleEffectPrefab.prefab  # If this is a prefab


```

---

## How to play 
- Open The only scene `TestScene`
- Run the game, enjoy it. 

<img width="610" height="409" alt="image" src="https://github.com/user-attachments/assets/f70425bf-882f-44f7-9ca9-a7882f1d3320" />


---

## Main Controllers


<img width="614" height="184" alt="image" src="https://github.com/user-attachments/assets/0d492f73-4821-4f18-998a-9ad392584993" />


### **1. DraggerController**

Responsible for detecting and dragging objects with mouse input.

```csharp
// Handles picking, dragging, and releasing objects
DragController:
- Registers click input actions
- Casts ray to detect movable objects
- Calculates drag offset and velocity
- Raises ObjectPickedEvent & ObjectDroppedEvent
```

### **2. FallingObjectController**

Manages falling objects and integrates physics and animation responses.

```csharp
FallingObjectController:
- Receives physics config and VFX config
- Uses GravityMotor2D to simulate motion and bounce
- Listens to ObjectPickedEvent / ObjectDroppedEvent
- Plays squash, stretch, and shrink animations via FruitAnimator
```

### **3. TreeController**

Manages fruit spawning and tree reactions.

```csharp
TreeController:
- Spawns fruit via EntitySpawner
- Reacts to fruit interactions using TreeAnimator
- Plays leaf/trunk VFX on fruit grab using TreeAnimator

```

### **4. RippleEffectController**

Manages fruit spawning and tree reactions.

```csharp
RippleEffectController:
- Executes Ripple Effect.

```

## Possible improvements
- The physics engine does not like high speeds against walls...
- Implement my own dependency injection framework here to avoid even more serializations and centralize my Monobehaviors, the less, the better... or rather just use a more formalized single entry point. my components are modular between them so there's no harm as of today.  

And that's it! 

<img width="640" height="406" alt="image" src="https://github.com/user-attachments/assets/53881f03-952a-4e28-9713-e9b0a760ca22" />



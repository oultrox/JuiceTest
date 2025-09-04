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
- **Code Driven** Animations like squash, stretch, wobble, shrink!

### Service animator pattern showcase
```csharp
using System.Collections;
using UnityEngine;

namespace AnimationService
{
    /// <summary>
    /// Component for displaying animations and applying code-driven animations.
    /// </summary>
    public class AnimatorService
    {
        readonly MonoBehaviour _coroutineHost;
        SpriteRenderer _spriteRenderer;
        Vector3 _originalScale;
        Quaternion _originalRotation;

        public AnimatorService(MonoBehaviour coroutineHost, SpriteRenderer spriteRenderer)
        {
            _coroutineHost = coroutineHost;
            _spriteRenderer = spriteRenderer;
            _originalScale = _spriteRenderer.transform.localScale;
            _originalRotation = _spriteRenderer.transform.localRotation;
        }

        /// <summary>
        /// Updates rendering flip side.
        /// </summary>
        public void UpdateFacingDirection(float horizontalInput)
        {
            if (Mathf.Abs(horizontalInput) > 0.01f)
                _spriteRenderer.flipX = horizontalInput < 0;
        }
        
        /// <summary>
        /// Plays a stretch effect by vertically stretching the scale of the sprite and resetting it.
        /// </summary>
        public void PlayShrinkFromSmallToBig(float scale = 0.2f, float duration = 0.5f)
        {
            Vector3 targetScale = new Vector3(_originalScale.x - scale, _originalScale.y - scale, _originalScale.z);
            _coroutineHost.StartCoroutine(DoScale(targetScale, duration));
        }

        /// <summary>
        /// Plays a stretch effect by vertically stretching the scale of the sprite and resetting it.
        /// </summary>
        public void PlayStretch(float intensity = 0.2f, float duration = 0.2f)
        {
            Vector3 targetScale = new Vector3(_originalScale.x - intensity, _originalScale.y + intensity, _originalScale.z);
            _coroutineHost.StartCoroutine(DoScale(targetScale, duration));
        }

        /// <summary>
        /// Plays a squash effect by horizontally stretching the scale of the sprite and resetting it.
        /// </summary>
        public void PlaySquash(float intensity = 0.2f, float duration = 0.2f)
        {
            Vector3 targetScale = new Vector3(_originalScale.x + intensity, _originalScale.y - intensity, _originalScale.z);
            _coroutineHost.StartCoroutine(DoScale(targetScale, duration));
        }

        /// <summary>
        /// Continuous effect: Applying a run wobble by lerping the sprite quaternion.
        /// </summary>
        public void ApplyConstantWobble(float intensity, float duration = 5f)
        {
            if (Mathf.Abs(intensity) < 0.01f)
                return;

            float wobble = Mathf.Sin(Time.time * duration) * 0.05f * Mathf.Abs(intensity);
            Quaternion targetRotation = Quaternion.Euler(0, 0, wobble * 10f);
            _spriteRenderer.transform.localRotation = Quaternion.Lerp(
                _spriteRenderer.transform.localRotation, 
                targetRotation, 
                Time.deltaTime * 8f
            );
        }

        /// <summary>
        /// Continuous effect: Resetting smoothly to the initial render transform local size and rotation.
        /// </summary>
        public void SmoothResetSpriteRender()
        {
            _spriteRenderer.transform.localScale = Vector3.Lerp(
                _spriteRenderer.transform.localScale, 
                _originalScale, 
                Time.deltaTime * 5f
            );
            _spriteRenderer.transform.localRotation = Quaternion.Lerp(
                _spriteRenderer.transform.localRotation, 
                _originalRotation, 
                Time.deltaTime * 5f
            );
        }

        IEnumerator DoScale(Vector3 targetScale, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                _spriteRenderer.transform.localScale = Vector3.Lerp(targetScale, _originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _spriteRenderer.transform.localScale = _originalScale;
        }
    }
}

```

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



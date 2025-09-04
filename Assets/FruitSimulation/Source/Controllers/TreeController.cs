using FruitSimulation.Source.Animators;
using FruitSimulation.Source.Configs;
using FruitSimulation.Source.Entities;
using FruitSimulation.Source.Events;
using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Controllers
{
    /// <summary>
    /// Injects animations and spawning of the fruit.
    /// </summary>
    public class TreeController: MonoBehaviour
    { 
        [Header ("Injection")]
        [SerializeField] GameObject objectPrefab;
        
        [Header ("Animation")]
        [SerializeField] SpriteRenderer treeSprite;
        [SerializeField] SpriteRenderer trunkSprite;
        [SerializeField] GameFeelVFXConfig _leafsEffectConfig;
        [SerializeField] GameFeelVFXConfig _trunkEffectConfig; 
        [SerializeField] Transform[] spawnerPoints;
        
        EntitySpawner _spawner;
        TreeAnimator _treeAnimator;
        EventListener<ObjectPickedEvent> _onFruitGrabbed;

        void Awake()
        {
            SetBehaviors();
        }
        
        void Update()
        {
            _treeAnimator.Tick();
        }
        
        void SetBehaviors()
        {
            _treeAnimator = new TreeAnimator(this, _leafsEffectConfig, _trunkEffectConfig, treeSprite, trunkSprite);
            _spawner = new EntitySpawner(objectPrefab, spawnerPoints, transform);
            _spawner.SpawnEntities();
            _onFruitGrabbed = new EventListener<ObjectPickedEvent>(OnFruitGrabbed);
            EventBus<ObjectPickedEvent>.Register(_onFruitGrabbed);
        }
        
        void OnFruitGrabbed(ObjectPickedEvent evt)
        {
            if (_spawner.CheckPickedUp(evt))
            {
                _treeAnimator.ReactToFruitGrab();
            }
        }
    }
    
}
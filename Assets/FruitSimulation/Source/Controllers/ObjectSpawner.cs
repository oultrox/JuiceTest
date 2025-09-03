using FruitSimulation.Source.Animators;
using FruitSimulation.Source.Configs;
using FruitSimulation.Source.Entities;
using UnityEngine;

namespace FruitSimulation.Source.Controllers
{
    /// <summary>
    /// orchestrates animations and spawning of the fruit.
    /// </summary>
    public class ObjectSpawner: MonoBehaviour
    { 
        [Header ("Animation")]
        [SerializeField] SpriteRenderer treeSprite;
        [SerializeField] SpriteRenderer trunkSprite;
        [SerializeField] GameFeelVFXConfig _leafsEffectConfig;
        [SerializeField] GameFeelVFXConfig _trunkEffectConfig;  
        
        [Header ("Injection")]
        [SerializeField] GameObject objectPrefab;
        [SerializeField] Transform[] spawnerPoints;
        
        EntitySpawner _spawner;
        TreeAnimator _treeAnimator;
        
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
            _spawner = new EntitySpawner(objectPrefab, spawnerPoints);
            _spawner.SpawnEntities();
        }
    }
    
}
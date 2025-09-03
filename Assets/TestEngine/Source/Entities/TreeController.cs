using TestEngine.Source.Juice;
using UnityEngine;

namespace TestEngine.Source.Entities
{
    public class TreeController: MonoBehaviour
    { 
        [SerializeField] GameFeelVFXConfig _leafsEffectConfig;
        [SerializeField] GameFeelVFXConfig _trunkEffectConfig;  
        [SerializeField] SpriteRenderer treeSprite;
        [SerializeField] SpriteRenderer trunkSprite;
        [SerializeField] GameObject objectPrefab;
        [SerializeField] Transform[] spawnerPoints;
        EntitySpawner _spawner;
        AnimatorService _leafsAnimatorService;
        AnimatorService _trunkAnimatorService;
        SpriteRenderer _spriteRenderer;
        
        
        void Awake()
        {
            SetBehaviors();
        }

        void Update()
        {
            _leafsAnimatorService.ApplyConstantWobble(_leafsEffectConfig.WobbleFactor,_leafsEffectConfig.WobbleDuration);
            _trunkAnimatorService.ApplyConstantWobble(_trunkEffectConfig.WobbleFactor,_trunkEffectConfig.WobbleDuration);
        }
        
        void SetBehaviors()
        {
            _leafsAnimatorService = new AnimatorService(this, treeSprite);
            _trunkAnimatorService = new AnimatorService(this, trunkSprite);
            _spawner = new EntitySpawner(objectPrefab, spawnerPoints);
            _spawner.SpawnEntities();
        }
    }
}
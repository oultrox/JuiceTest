using TestEngine.Source.Juice;
using UnityEngine;

namespace TestEngine.Source.Entities
{
    public class TreeController: MonoBehaviour
    { 
        [SerializeField] float leafWobbleIntensity = 0.5f;
        [SerializeField] float leafWobbleSpeed = 3f;
        [SerializeField] float trunkWobbleIntensity = 0.5f;
        [SerializeField] float trunkWobbleSpeed = 3f;
        [SerializeField] SpriteRenderer treeSprite;
        [SerializeField] SpriteRenderer trunkSprite;
        
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
            _leafsAnimatorService.ApplyConstantWobble(leafWobbleIntensity,leafWobbleSpeed);
            _trunkAnimatorService.ApplyConstantWobble(trunkWobbleIntensity,trunkWobbleSpeed);
        }
        
        void SetBehaviors()
        {
            _leafsAnimatorService = new AnimatorService(this, treeSprite);
            _trunkAnimatorService = new AnimatorService(this, trunkSprite);
        }
    }

    public class EntitySpawner
    {
        
    }
}
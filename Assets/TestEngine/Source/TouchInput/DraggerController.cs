using SimpleBus;
using TestEngine.Source.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestEngine.Source.TouchInput
{
    /// <summary>
    /// In charge of the drag mechanics.
    /// </summary>
    public class DraggerController: MonoBehaviour
    {
        [SerializeField] LayerMask movableLayerMask;
        Camera _camera;
        Transform _currentTarget;
        Vector3 _offset;
        PlayerInput _playerInput;
        InputAction _clickAction;
        
        
        void Awake()
        {
            InitBehaviors();
        }

        void OnDisable()
        {
            UnRegisterListeners();
        }
        
        void Update()
        {
            ApplyDragging();
        }
        
        private void InitBehaviors()
        {
            _camera = Camera.main;
            _playerInput = GetComponent<PlayerInput>();
            _clickAction = _playerInput.actions["Click"];
            RegisterListeners();
        }

        void RegisterListeners()
        {
            _clickAction.started += AttemptGrabTarget;
            _clickAction.canceled += ClearTarget;
        }

        void UnRegisterListeners()
        {
            _clickAction.started += AttemptGrabTarget;
            _clickAction.canceled += ClearTarget;
        }
        
        void ClearTarget(InputAction.CallbackContext context)
        {
            _currentTarget = null;
        }

        void AttemptGrabTarget(InputAction.CallbackContext context)
        {
            var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 
                float.MaxValue, movableLayerMask);

            if (hit.collider == null) return;
            
            Debug.Log("OBJECT IS FOUND!: " + hit.collider.gameObject.name);
            _currentTarget = hit.transform;
            _offset = hit.transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
            EventBus<ObjectPickedEvent>.Raise(new ObjectPickedEvent(_currentTarget));
        }

        void ApplyDragging()
        {
            if (_currentTarget != null)
            {
                _currentTarget.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offset;
            }
        }
    }
}
using FruitSimulation.Source.Events;
using SimpleBus;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FruitSimulation.Source.Controllers
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
        Vector3 _lastMouseWorldPos;
        Vector2 _smoothedVelocity;
        
        
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
            CheckTouchVelocity();
        }
        
        void InitBehaviors()
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
        
        void AttemptGrabTarget(InputAction.CallbackContext context)
        {
            var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 
                float.MaxValue, movableLayerMask);

            if (hit.collider == null) return;

            _currentTarget = hit.transform;
            var root = _currentTarget.root;

            _offset = _currentTarget.position - _camera.ScreenToWorldPoint(Input.mousePosition);

            EventBus<ObjectPickedEvent>.Raise(new ObjectPickedEvent(_currentTarget, root));

        }

        void ApplyDragging()
        {
            if (_currentTarget != null)
            {
                _currentTarget.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offset;
            }
        }
        
        void CheckTouchVelocity()
        {
            if (_currentTarget != null)
            {
                ApplyDragging();
                Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                _smoothedVelocity = (mouseWorldPos - _lastMouseWorldPos) / Time.deltaTime;
                _lastMouseWorldPos = mouseWorldPos;
            }
        }
        
        void ClearTarget(InputAction.CallbackContext context)
        {
            if (_currentTarget == null) return; 
            EventBus<ObjectDroppedEvent>.Raise(new ObjectDroppedEvent(_smoothedVelocity, _currentTarget));
            _currentTarget = null;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using FruitSimulation.Source.Events;
using SimpleBus;
using UnityEngine;

namespace TestEngine.Source.Entities
{
    public class EntitySpawner
    {
        readonly GameObject _prefab;
        readonly Transform[] _spawnPoints;
        readonly EventListener<ObjectPickedEvent> _onObjectPicked;
        readonly EventListener<ObjectDroppedEvent> _onObjectDropped;
        readonly Dictionary<Transform, GameObject> _prefabsInstantiatedInPositions;
        
        public EntitySpawner(GameObject prefab, Transform[] spawnPoints)
        {
            _prefab = prefab;
            _spawnPoints = spawnPoints;
            
            _onObjectPicked = new EventListener<ObjectPickedEvent>(CheckPickedUp);
            _onObjectDropped = new EventListener<ObjectDroppedEvent>(AttemptSpawnReplacement);
            EventBus<ObjectPickedEvent>.Register(_onObjectPicked);
            EventBus<ObjectDroppedEvent>.Register(_onObjectDropped);
            
            _prefabsInstantiatedInPositions = _spawnPoints.ToDictionary(sp => sp, sp => (GameObject)null);
        }

        void AttemptSpawnReplacement(ObjectDroppedEvent obj)
        {
            Debug.Log("Spawning!");
            _prefabsInstantiatedInPositions.TryGetValue(obj.Transform, out var instantiated);

            if (instantiated == null)
            {
                SpawnEntities();
            }
        }

        void CheckPickedUp(ObjectPickedEvent obj)
        {
            foreach (var key in _prefabsInstantiatedInPositions.Keys.ToList())
            {
                if (_prefabsInstantiatedInPositions[key] == obj.Transform.gameObject)
                    _prefabsInstantiatedInPositions[key] = null;
            }
        }

        public void SpawnEntities()
        {
            var spawnPoints = Enumerable.ToHashSet(_prefabsInstantiatedInPositions
                .Where(f => f.Value == null)
                .Select(p => p.Key));
            
            foreach (var spawn in spawnPoints)
            {
                var obj = Object.Instantiate(_prefab, spawn);
                _prefabsInstantiatedInPositions[spawn] = obj;
            }
        }
    }
}
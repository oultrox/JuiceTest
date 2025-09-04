using System.Collections.Generic;
using System.Linq;
using FruitSimulation.Source.Events;
using SimpleBus;
using UnityEngine;

namespace FruitSimulation.Source.Entities
{
    /// <summary>
    /// Just takes care of spawning given prefabs. Nothing fancy.
    /// </summary>
    public class ObjectSpawner
    {
        readonly GameObject _prefab;
        readonly EventListener<ObjectDroppedEvent> _onObjectDropped;
        readonly Dictionary<Transform, GameObject> _prefabsInstantiatedInPositions;
        readonly Dictionary<GameObject, Transform> _instancesToSpawns;
        readonly Transform _parent;
        
        public ObjectSpawner(GameObject prefab, Transform[] spawnPoints, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _prefabsInstantiatedInPositions = spawnPoints.ToDictionary(sp => sp, sp => (GameObject)null);
            _instancesToSpawns = new Dictionary<GameObject, Transform>();
            _onObjectDropped = new EventListener<ObjectDroppedEvent>(AttemptSpawnReplacement);
            EventBus<ObjectDroppedEvent>.Register(_onObjectDropped);
        }
        
        /// <summary>
        /// Spawns the composite prefab given how empty the dictionary is based on the transform keys.
        /// </summary>
        public void SpawnEntities()
        {
            var spawnPoints = Enumerable.ToHashSet(_prefabsInstantiatedInPositions
                .Where(f => f.Value == null)
                .Select(p => p.Key));
            
            foreach (var spawn in spawnPoints)
            {
                var obj = Object.Instantiate(_prefab, spawn);
                _prefabsInstantiatedInPositions[spawn] = obj;
                _instancesToSpawns[obj] = spawn;
            }
        }
        
        /// <summary>
        /// Checks if this object spawned is from this spawner.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CheckPickedUp(ObjectPickedEvent obj)
        {
            var objRootTransform = obj.RootTransform;
            var objGameObject = obj.TargetTransform.gameObject;
            
            if (_parent.name != objRootTransform.name)
                return false; 

            objGameObject.transform.SetParent(null);
            _instancesToSpawns.Remove(objGameObject, out var spawn);
            _prefabsInstantiatedInPositions[spawn] = null;
            return true;
        }
        
        void AttemptSpawnReplacement(ObjectDroppedEvent obj)
        {
            _prefabsInstantiatedInPositions.TryGetValue(obj.Transform, out var instantiated);

            if (instantiated == null)
            {
                SpawnEntities();
            }
        }
    }
}
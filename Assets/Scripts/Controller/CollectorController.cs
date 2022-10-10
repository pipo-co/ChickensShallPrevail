using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Interface;
using Manager;
using UnityEngine;
using Utils;

namespace Controller
{
    [RequireComponent(typeof(Collider))]
    public class CollectorController : MonoBehaviour, ICollector  
    {
        private ICollectorStats _stats;    

        private Dictionary<CollectableType, int> collectables;

        public int CollectableLayer => _stats.CollectableLayer;
        private void Start()
        {
            _stats = GetComponent<ICollectorStats>();
            collectables = new Dictionary<CollectableType, int>();
            foreach (var type in EnumUtil.GetValues<CollectableType>())  
            {  
                collectables[type] = 0;
            } 
        }

        private void OnCollisionEnter(Collision otherCollider)
        {
            if (CollectableLayer != otherCollider.gameObject.layer) return;
            
            if (Enum.TryParse(otherCollider.gameObject.name, true, out CollectableType type))
            {
                collectables[type] += 10;
                NotifyCollectableChane(type);
            }
            
            Destroy(otherCollider.gameObject);
        }

        private void NotifyCollectableChane(CollectableType type)
        {
            EventsManager.instance.CollectableChange(type, collectables[type]);
        }
        
        public bool Expend(CollectableType type, int amount)
        {
            if (collectables[type] < amount) return false;
            collectables[type] -= amount;
            NotifyCollectableChane(type);
            return true;
        }
    }
    
    public interface ICollectorStats
    {
        int CollectableLayer { get; }
    }
}
﻿using System;
using System.Collections.Generic;
using Entities;
using Entities.Turrets;
using UnityEngine;
using Utils;

namespace Manager
{
    public class EventsManager : MonoBehaviour
    {
        public static EventsManager instance;

        #region UNITY_EVENTS
        private void Awake()
        {
            if (instance != null) Destroy(this);
            instance = this;
            
            _onCollectableChange = new Dictionary<CollectableType, Action<int>>();
            foreach (var type in EnumUtil.GetValues<CollectableType>())  
            {  
                _onCollectableChange[type] = _ => {};
            } 
        }
        #endregion

        #region GAME_MANAGE
        public event Action<bool>                   OnGameOver;
        public event Action<float, float>           OnFarmLifeChange;
        public event Action<Turret>                 OnTurretChange;
        public event Action<EnemyType, GameObject>  OnEnemySpawn;
        public event Action<int>                    OnEnemyKilled;
        
        private Dictionary<CollectableType, Action<int>> _onCollectableChange;

        public void EventGameOver(bool isVictory)
        {
            OnGameOver?.Invoke(isVictory);
        }
        
        public void FarmLifeChange(float currentLife, float maxLife)
        {
            OnFarmLifeChange?.Invoke(currentLife, maxLife);
        }
        
        public void TurretChange(Turret turret)
        {
            OnTurretChange?.Invoke(turret);
        }
        
        public void CollectableChange(CollectableType type, int currentValue)
        {
            _onCollectableChange[type].Invoke(currentValue);
        }
        
        public void AddOnCollectableChangeHandler(CollectableType type, Action<int> handler)
        {
            _onCollectableChange[type] += handler;
        }

        public void EnemySpawn(EnemyType type,GameObject enemy)
        {
            OnEnemySpawn?.Invoke(type, enemy);
        }
        
        public void EnemyKilled(int id)
        {
            OnEnemyKilled?.Invoke(id);
        }
        
        #endregion
    }
}
using System;
using System.Collections.Generic;
using Command;
using Flyweight;
using Interface;
using Manager;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(IAutoMove))]
    public class Chicken : MonoBehaviour, IDeployer
    {
        private ChickenStat _stats;
        private ChickenStat Stats => _stats ??= GetComponent<StatSupplier>().GetStat<ChickenStat>();
        
        public int Period => Stats.Period;
        public GameObject DeployablePrefab => Stats.EggPrefab;
        public List<int> LayerTarget => Stats.LayerTarget;
        
        private float _nextDeployTime;

        private CmdDeploy _cmdDeploy;
        
        public Collider Collider { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public IAutoMove AutoMove { get; private set; }

        private Transform _eggsTransform;

        private void Awake()
        {
            _eggsTransform = GameObject.Find("Eggs").transform;
            
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
            AutoMove = GetComponent<IAutoMove>();

            Collider.isTrigger = true;
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true; //Inafectable
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            transform.Rotate(((float)Math.PI) * Vector3.up);
            transform.Rotate(2 * Vector3.up);
            
            _nextDeployTime = Time.time;

            _cmdDeploy = new CmdDeploy(this);
        }

        public void Deploy()
        {
            var t = transform;
            var turret = Instantiate(DeployablePrefab, t.position, t.rotation);
            turret.name = CollectableType.Egg.ToString();
            turret.transform.parent = _eggsTransform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!LayerTarget.Contains(other.gameObject.layer)) return;
            transform.Rotate(180 * Vector3.up);
        }

        private void Update()
        {
            AutoMove.Travel();
            if (!(Time.time > _nextDeployTime)) return;
            _nextDeployTime += Period;
            EventQueueManager.instance.AddCommand(_cmdDeploy);
        }
    }
}
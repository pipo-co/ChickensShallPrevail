using System;
using System.Collections.Generic;
using Command;
using Interface;
using Manager;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(IAutoMove))]
    public class Chicken : MonoBehaviour, IDeployer
    {
        [SerializeField] protected GameObject _eggPrefab;

        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private Collider _collider;

        [SerializeField] private IAutoMove _autoMoveController;
        
        private float nextShotTime = 0;
        [SerializeField] private float period;

        private CmdDeploy _cmdDeploy;

        [SerializeField] private List<int> layerTarget;

        #region ACCESORS

        public GameObject BulletPrefab => _eggPrefab;
        public Collider Collider => _collider;
        public Rigidbody Rigidbody => _rigidBody;
        public IAutoMove AutoMove => _autoMoveController;

        #endregion


        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _autoMoveController = GetComponent<IAutoMove>();

            _collider.isTrigger = true;
            _rigidBody.useGravity = false;
            _rigidBody.isKinematic = true; //Inafectable
            _rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            transform.Rotate(((float)Math.PI) * Vector3.up);
            transform.Rotate(2 * Vector3.up);

            _cmdDeploy = new CmdDeploy(this);
        }

        public void Deploy()
        {
            var turret = Instantiate(_eggPrefab, transform.position, transform.rotation);
            turret.name = "Egg";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!layerTarget.Contains(other.gameObject.layer)) return;
            transform.Rotate(180 * Vector3.up);
        }

        private void Update()
        {
            _autoMoveController.Travel();
            if (Time.time > nextShotTime)
            {
                nextShotTime += period;
                EventQueueManager.instance.AddCommand(_cmdDeploy);
            }
        }
    }
}
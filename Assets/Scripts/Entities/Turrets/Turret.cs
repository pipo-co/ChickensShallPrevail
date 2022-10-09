
using System;
using Command;
using Entities;
using Manager;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Turret : MonoBehaviour, ITurret 
{
    
    [SerializeField] protected GameObject _bulletPrefab;
    private IDamageable _damageable;
    protected Collider _collider;

     public TurretType TurretType => _turretType;
     [SerializeField] private TurretType _turretType;

    public int Cost => _cost;
    [SerializeField] private int _cost;
    
    private float nextShotTime = 0;
    [SerializeField] private float period;
    
    #region ACCESORS
    public GameObject BulletPrefab => _bulletPrefab;
    public IDamageable Damageable => _damageable;
    public Collider Collider => _collider;
    
    private CmdAttack _cmdAttack;
    #endregion
    
    public virtual void Attack()
    {
        float height = _collider.bounds.size.y / 4;
        var bullet = Instantiate(_bulletPrefab, transform.position + Vector3.up * height, transform.rotation);
        bullet.name = "Bala Turret";
    }

    protected virtual void Start()
    {
        _damageable = GetComponent<IDamageable>();
        _collider = GetComponent<Collider>();
        _cmdAttack = new CmdAttack(this);
    }

    protected virtual void Update()
    {
        if(Time.time > nextShotTime ) {
            nextShotTime += period;
            EventQueueManager.instance.AddCommand(_cmdAttack);
        }
    }
}

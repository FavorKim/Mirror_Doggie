using Mirror;
using UnityEngine;


public class Favor_Projectile : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody RigidBody_Projectile;

    [Header("Stats")]
    [SerializeField] private float _force = 1000;
    [SerializeField] private float _destroyDelay = 5.0f;



    public override void OnStartServer()
    {
        Invoke("DestroySelf", _destroyDelay);
    }

    private void Start()
    {
        RigidBody_Projectile.AddForce(transform.forward * _force);
    }





    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    
    [ServerCallback]
    private void OnTriggerEnter(Collider collision)
    {
        DestroySelf();
    }
}

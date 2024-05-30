using Mirror;
using UnityEngine;

public class AtkSpawnObject : NetworkBehaviour
{
    public float _destroyDelay = 7.0f;
    public float _force = 1000;

    public Rigidbody RigidBody_AtkObject;


    // 서버, 호스트에서만 (클라 x)
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), _destroyDelay);
    }

    private void Start()
    {
        RigidBody_AtkObject.AddForce(transform.forward * _force);
    }

    // 클라에서 실행 금지!
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        DestroySelf();
    }
}

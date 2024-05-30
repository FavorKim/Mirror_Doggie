using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class PlayerSpawnObject : NetworkBehaviour
{
    [Header("Componenets")]
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;
    public TextMesh TextMesh_HealthBar;
    public TextMesh TextMesh_NetType;
    public Transform Transform_Player;


    [Header("Movement")]
    public float _rotationSpeed = 100.0f;


    [Header("Attack")]
    public KeyCode _atkKey = KeyCode.Space;
    public GameObject Prefab_AtkObject;
    public Transform Transform_AtkSpawnPos;

    [Header("Stats Server")]
    [SyncVar] public int _health = 4;

    // 클라, 서버 두 군데 모두 작동해야 하는 부분을 작성해야 함.
    public void Update()
    {
        SetHealthbarOnUpdate(_health);

        if (CheckIsFocusedOnUpdate() == false) return;

        CheckIsLocalPlayerOnUpdate()
    }

    private void CheckIsLocalPlayerOnUpdate()
    {

    }

    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }

    private void SetHealthbarOnUpdate(int health)
    {
        TextMesh_HealthBar.text = new string('-', health);
    }

    // 클라에서 서버로 호출은 하지만, 동작은 서버만
    [Command]
    private void CommandAtk()
    {

    }

    [ClientRpc]
    private void RpcOnAttack()
    {

    }

    // 클라에서 아래 함수가 실행되지 않도록 ServerCallback을 달아줌 (서버, 클라 둘 다 실행하면 안되는 경우. 서버에서만 실행해야하는 경우.)
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        
    }

}

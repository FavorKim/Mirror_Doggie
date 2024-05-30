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

    // Ŭ��, ���� �� ���� ��� �۵��ؾ� �ϴ� �κ��� �ۼ��ؾ� ��.
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

    // Ŭ�󿡼� ������ ȣ���� ������, ������ ������
    [Command]
    private void CommandAtk()
    {

    }

    [ClientRpc]
    private void RpcOnAttack()
    {

    }

    // Ŭ�󿡼� �Ʒ� �Լ��� ������� �ʵ��� ServerCallback�� �޾��� (����, Ŭ�� �� �� �����ϸ� �ȵǴ� ���. ���������� �����ؾ��ϴ� ���.)
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        
    }

}

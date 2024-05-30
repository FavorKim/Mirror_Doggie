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

        CheckIsLocalPlayerOnUpdate();
    }

    // 로컬에서만 돌리는 부분
    private void CheckIsLocalPlayerOnUpdate()
    {
        if(this.isLocalPlayer == false) return;

        // 로컬 플레이어의 회전
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * _rotationSpeed * Time.deltaTime, 0);

        // 로컬 플레이어의 이동
        float vertical = Input.GetAxis("Vertical");
        Vector3 foward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = foward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        // 로컬 플레이어의 공격
        if (Input.GetKeyDown(_atkKey))
        {
            CommandAtk();   // 커맨드는 클라에서 호출한다. 호출 만
        }

        RotateLocalPlayer();
    }

    private void RotateLocalPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit hit, 100))
        {
            Debug.DrawRay(ray.origin, hit.point);
            Vector3 lookRotate = new Vector3(hit.point.x, Transform_Player.position.y, hit.point.z);
            Transform_Player.LookAt(lookRotate);
        }
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
        GameObject atkObjectForSpawn = Instantiate(Prefab_AtkObject, Transform_AtkSpawnPos.transform.position, Transform_AtkSpawnPos.transform.rotation);
        NetworkServer.Spawn(atkObjectForSpawn);

    }

    [ClientRpc]
    private void RpcOnAttack()
    {

    }

    // 클라에서 아래 함수가 실행되지 않게 함 (서버, 클라 둘 다 실행하면 안되는 경우. 서버에서만 실행해야하는 경우.)
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        
    }

}

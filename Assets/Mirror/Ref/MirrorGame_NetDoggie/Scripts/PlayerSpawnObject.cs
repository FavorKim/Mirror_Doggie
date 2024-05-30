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
        string netTypeStr = isClient ? "클라" : "클라 아님";


        TextMesh_NetType.text = this.isLocalPlayer ?
            $"{netId} : [로컬 / {netTypeStr}]" 
            : $"{netId} : [로컬 아님 / {netTypeStr}]";

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
        RpcOnAttack();  
    }

    /*
    rpc는 모든 클라에서 실행. 커맨드는 호출한 애만 호출, 실행은 서버에서.
    rpc가 호출되었을 때, 애니메이터에 접근하여 트리거를 변경하는데.
    이 경우 트리거를 설정할 애니메이터를 어떻게 찾아서 해당 객체의 애니메이터를 조작하는가?
    Rpc는 모든 클라이언트가 실행하는 것이 아니라 호출한 클라이언트만 실행하는가?
    위의 코드에서, 생성은 서버만 할 것이고, Rpc를 호출하며 모든 클라에서 rpc를 호출하는 것이 아닌가?
    그게 아니라면 다른 클라이언트는 다른 클라이언트의 애니메이션이 실행되는 것을 확인할 방법이 없다.
    */

    [ClientRpc] 
    private void RpcOnAttack()
    {
        Debug.Log($"{this.netId}가 공격");
        Animator_Player.SetTrigger("Atk");
    }

    // 클라에서 아래 함수가 실행되지 않게 함 (서버, 클라 둘 다 실행하면 안되는 경우. 서버에서만 실행해야하는 경우.)
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var atkGenObj = other.GetComponent<AtkSpawnObject>();

        if (atkGenObj == null) return;

        _health--;

        if (_health <= 0) 
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

}

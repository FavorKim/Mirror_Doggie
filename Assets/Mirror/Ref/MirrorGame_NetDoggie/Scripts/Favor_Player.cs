using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Favor_Player : NetworkBehaviour
{
    [Header("Components")]
    public Transform Transform_Player;
    public Animator Animator_Player;
    public Transform Transform_AtkPos;
    public TextMesh TextMesh_Healthbar;
    public TextMesh TextMesh_HUD;
    public NavMeshAgent NavAgent_Player;

    [Header("Attack")]
    public GameObject _projectileObjPrefab;
    public KeyCode atkKey = KeyCode.Space;

    [Header("Movement")]
    public float _rotationSpeed = 100.0f;

    [Header("Stats Server")]
    [SyncVar] public int _health = 4;
    [SyncVar] public string netState;
    //public FavorNetManager FavorNetManager;

    public string _name;


    public override void OnStartClient()
    {
        CheckState();
        FavorNetManager.Instance.listPlayer.Add(this);
        FavorNetManager.Instance.PlayerName();
    }

    private void Update()
    {
        SetHpUIOnUpdate(_health);

        if (CheckIsFocusedOnUpdate())
            LocalPlayerUpdate();
    }

    private void LocalPlayerUpdate()
    {
        if (!this.isLocalPlayer) return;

        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * _rotationSpeed * Time.deltaTime, 0);

        float vertical = Input.GetAxis("Vertical");
        Vector3 foward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = foward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        if (Input.GetKeyDown(atkKey))
            CommandAttack();
    }

    [Command]
    void CommandAttack()
    {
        GameObject projectileObj = Instantiate(_projectileObjPrefab, Transform_AtkPos.position, Transform_AtkPos.rotation);
        NetworkServer.Spawn(projectileObj);
        RpcOnAttack();
    }

    [ClientRpc]
    void RpcOnAttack()
    {
        Animator_Player.SetTrigger("Atk");
    }


    [ClientRpc]
    private void SetHUDLocalorNot()
    {
        netState = isClientOnly ? "클라" : "호스트";

        TextMesh_HUD.text = isLocalPlayer ? $"[{netState}] 로컬"
            : $"[{netState}] 로컬 아님";
    }

    [Command(requiresAuthority =false)]
    void CommandSetHUD()
    {
        SetHUDLocalorNot();
    }

    private void SetHpUIOnUpdate(int hp)
    {
        TextMesh_Healthbar.text = new string('-', _health);
    }
    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }


    private void CheckState()
    {
        // 로컬일때만
        if (isLocalPlayer)
        {
            if (NetworkManager.singleton.mode == NetworkManagerMode.Host)
            {
                NameUp("호스트 플레이어");
            }
            else
            {
                NameUp("플레이어");
            }
        }
    }
    


    [Command]
    public void NameUp(string name)
    {
        _name = name;
        NameDown(_name);
    }

    [ClientRpc]
    public void NameDown(string name)
    {
        TextMesh_HUD.text = name;
    }

    [Command]
    public void CheckName()
    {
        NameDown(_name);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<Favor_Projectile>();
        
        if (obj == null)
        {
            return;
        }
        
        _health--;
        
        if (this._health == 0)
            NetworkServer.Destroy(gameObject);
    }

}

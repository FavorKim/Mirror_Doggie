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

    [ClientRpc]         // ȣ��Ʈ�� �÷��̾� 1,2 Ŭ���� �÷��̾� 1,2
    void RpcOnAttack()
    {
        Animator_Player.SetTrigger("Atk");
    }


    [ClientRpc]
    private void SetHUDLocalorNot()
    {
        netState = isClientOnly ? "Ŭ��" : "ȣ��Ʈ";

        TextMesh_HUD.text = isLocalPlayer ? $"[{netState}] ����"
            : $"[{netState}] ���� �ƴ�";
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
        if (isLocalPlayer)
        {
            if (NetworkManager.singleton.mode == NetworkManagerMode.Host)
            {
                netState = "ȣ��Ʈ �÷��̾�";
            }
            else
            {
                netState = "�÷��̾�";
            }
            NameUp(netState);
        }
    }
    


    [Command]
    public void NameUp(string name)
    {
        Debug.Log($"{netId} - up");
        //_name = name;
        NameDown(name);
    }

    [ClientRpc]
    public void NameDown(string name)
    {
        Debug.Log($"{netId} - down");
        TextMesh_HUD.text = name;
    }

    [Command]
    public void CheckName()
    {
        Debug.Log($"{netId} - Check");
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

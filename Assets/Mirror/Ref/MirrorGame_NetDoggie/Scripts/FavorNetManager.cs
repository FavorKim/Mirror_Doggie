using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class FavorNetManager : NetworkManager
{
    public List<Favor_Player> listPlayer;

    public void PlayerName()
    {
        foreach (var player in listPlayer)
        {
            player.CheckName();
        }
    }


    //public enum STATE
    //{
    //    NONE, HOST, CLIENT
    //}

    private static FavorNetManager instance;
    public static FavorNetManager Instance { get { return instance; } }

    //public Dictionary<int, STATE> idStateDict = new Dictionary<int, STATE>();



    public override void Start()
    {
        base.Start();
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
        DontDestroyOnLoad(instance);
    }

    //public void AddStateDict(NetworkBehaviour player)
    //{
    //    int id = (int)player.netId;

    //    foreach(int ids in idStateDict.Keys)
    //    {
    //        if (id == ids) return;
    //    }

    //    STATE state;
    //    if (player.isClientOnly) state = STATE.CLIENT;
    //    else state = STATE.HOST;

    //    idStateDict.Add(id, state);
    //}

    //public string GetState(int netId)
    //{
    //    return idStateDict[netId].ToString();
    //}

}

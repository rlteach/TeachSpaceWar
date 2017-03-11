using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;

public class Multiplayer : NetworkManager {

    public override void  OnServerConnect(NetworkConnection vConn) {
        base.OnServerConnect(vConn);
        DB.MsgFormat("Connected to {0}", vConn);
    }

    public override void OnServerDisconnect(NetworkConnection vConn) {
        base.OnServerDisconnect(vConn);
        DB.MsgFormat("Disconnected from {0}", vConn);
    }

}

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.Networking.NetworkSystem;

//Helper classes to deal with Server & client events


public class Multiplayer : NetworkManager {     //Add more functions to network manager

    public override void  OnServerConnect(NetworkConnection vConn) {        //Both these are called, note documentation wrong, its an override
        base.OnServerConnect(vConn);
        DB.MsgFormat("Connected to {0}", vConn);
    }

    public override void OnServerDisconnect(NetworkConnection vConn) {  //Both these are called, note documentation wrong, its an override
        base.OnServerDisconnect(vConn);
        DB.MsgFormat("Disconnected from {0}", vConn);
    }
    public override void OnServerAddPlayer(NetworkConnection vConn, short vPlayerControllerId) {        //Documentation WRONG, use this not what they say to get called when clients added
        DB.MsgFormat("OnServerAddPlayer({0},{1})", vConn, vPlayerControllerId);
        base.OnServerAddPlayer(vConn, vPlayerControllerId);         //Call base method to create player from prefab
    }

    public override void OnServerRemovePlayer(NetworkConnection vConn, PlayerController vPlayer) {      //Documentation WRONG, this is never called
        base.OnServerRemovePlayer(vConn, vPlayer);
        DB.MsgFormat("OnServerRemovePlayer({0},{1})", vConn, vPlayer);
    }
}

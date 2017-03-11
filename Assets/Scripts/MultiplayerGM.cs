using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.Networking.NetworkSystem;

//Helper classes to deal with Server & client events

namespace Multiplayer {

    public class MultiplayerGM : NetworkManager {     //Add more functions to network manager

        #region GameManager
        static  MultiplayerGM sMP;

        public void Awake() {
            if(sMP==null) {
                sMP = this;
				UserNameInput.text = System.Environment.UserName;
            }
        }

        #endregion


        #region Player events
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
        #endregion


		#region UI
		public	Text		ScoreField;		//Link in IDE
		public	static 	int	Score {		//Set Score
			set {
				sMP.ScoreField.text = value.ToString ();
			}
		}

		PlayerShip	mLocalPlayer;		//When Local player starts, register them here
		public	static	PlayerShip	LocalPlayerShip {
			get {
				return sMP.mLocalPlayer;
			}
			set {
				sMP.mLocalPlayer = value;
				sMP.NewName ();		//Also send name from input box
			}
		}


		public	InputField	UserNameInput;	//Link input field in inspector
		public void	NewName() {
			if (LocalPlayerShip != null) {		//Set name on local player, if spawned
				LocalPlayerShip.CmdSetUserName (sMP.UserNameInput.text);
			}
		}

		#endregion

    }
}

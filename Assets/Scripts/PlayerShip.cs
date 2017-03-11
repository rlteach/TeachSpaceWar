using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.UI;

namespace Multiplayer {

    public class PlayerShip : NetworkBehaviour {

	    #region IDE Set values
        [Header("User settings")]           //These will show up in inspector

        [Range(180f, 720f)]
        public float RotationSpeed = 180f;

        [Range(0, 5f)]
        public 	float Speed = 0.2f;

        [Range(0, 10f)]
        public float MaxSpeed = 10f;

        [Range(0, 3f)]
	    public 	float BulletSpeed = 1.0f;

        [Range(0, 3f)]
	    public 	float BulletTimeToLive = 1.0f;

	    public	Text	NameText;		//Link Name Text here
	    #endregion


	    #region Player Startup


	    [SyncVar (hook="OnNameChange")]
	    string	mPlayerName="None";

	    SpriteRenderer	mSR;

        Rigidbody2D mRB;

	    public	override void OnStartClient() {
		    mSR=GetComponent<SpriteRenderer>();	//Cache key components for fast reuse
		    OnNameChange (mPlayerName);		//Make sure nam eis shown first time its created
            mRB = GetComponent<Rigidbody2D>();
        }

        public	override void OnStartLocalPlayer () {
		    base.OnStartLocalPlayer ();
		    mSR.color = Color.green;		//Make player ship green, locally
		    CmdSetUserName(System.Environment.UserName);		//Update this players name, with local login on server
	    }

	    [Command]		//This will run on server to update SyncVar, and hence all the names on the players
	    public void	CmdSetUserName(string vName) {
		    mPlayerName = vName;		
	    }

	    void	OnNameChange(string vName) {
		    mPlayerName = vName;		//Reflect change locally
		    NameText.text=vName;	//Show on screen
	    }

        #endregion


        #region PlayerMove

        void Update() {
		    if (isLocalPlayer) {
			    MoveLocalPlayer ();
                LocalPlayerFire();
		    }
        }

	    void	MoveLocalPlayer() {
			if (Input.GetKey (KeyCode.LeftArrow)) {      //Rotate ship, could use torque, but this looks better
                transform.localRotation *= Quaternion.Euler(0f, 0f, RotationSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
                transform.localRotation *= Quaternion.Euler(0f, 0f, -RotationSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.UpArrow)) {    //Apply force in direction of rotation
                mRB.velocity += (Vector2)(transform.localRotation * Vector2.up* Speed);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {    //Apply force in direction of rotation
                mRB.velocity -= (Vector2)(transform.localRotation * Vector2.up * Speed);
            }
            if (mRB.velocity.magnitude > MaxSpeed) {
                mRB.velocity = mRB.velocity.normalized * MaxSpeed;       //Clamp speed
            }
	    }

        #endregion

        #region PlayerFire
        Cooldown mFireCooldown = new Cooldown(0.25f);       //Cooldown Helper
        public Transform BulletSpawnPosition;       //Link in IDE

        void    LocalPlayerFire() {
            if (mFireCooldown.Cool(Time.deltaTime) && Input.GetKey (KeyCode.Space)) {
                CmdFire();              //Work out where bullet should appear and how fast it should go
			}
        }
        public GameObject BulletPrefab;        //Assign in Inspector

        [Command]   //This runs on server copy of Player, which is needed as only server can spawn objects
        public  void    CmdFire() {
            Vector2 tBulletVelocity = transform.rotation*Vector2.up * 10f;
            GameObject tBulletGO = Instantiate(BulletPrefab, BulletSpawnPosition.position, BulletPrefab.transform.rotation) as GameObject;
            Rigidbody2D tRB = tBulletGO.GetComponent<Rigidbody2D>();
            tRB.velocity = tBulletVelocity;
            Destroy(tBulletGO, 2f);
            NetworkServer.Spawn(tBulletGO);
        }
        #endregion


        void OnTriggerEnter2D(Collider2D vOther) {
    //        DB.MsgFormat("{0} collided with {1}", gameObject.name, vOther.gameObject.name);
        }
    }
}

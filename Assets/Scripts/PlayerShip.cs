using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.UI;

namespace Multiplayer {

    public class PlayerShip : Entity {
		#region Idenfication

		public	Text	NameText;		//Link Name Text here

		public	override	EntityType EType {
			get {
				return	EntityType.Player;
			}
		}

		[SyncVar (hook="OnNameChange")]
		string	mPlayerName="None";

		[Command]		//This will run on server to update SyncVar, and hence all the names on the players
		public void	CmdSetUserName(string vName) {
			mPlayerName = vName;		
		}

		void	OnNameChange(string vName) {
			mPlayerName = vName;		//Reflect change locally
			NameText.text=vName;	//Show on screen
		}

		#endregion


	    #region IDE Set values
        [Header("User settings")]           //These will show up in inspector

        [Range(180f, 720f)]
        public float RotationSpeed = 180f;

        [Range(0, 5f)]
        public 	float Speed = 0.2f;

        [Range(0, 10f)]
        public float MaxSpeed = 10f;

	    #endregion

		#region Score
		[SyncVar (hook="OnScoreChange")]
		int		mScore;

		void	OnScoreChange(int vScore) {
			mScore= vScore;		//Update local score variable
			if(isLocalPlayer) {
				MultiplayerGM.Score=mScore;		//Set Score In UI
			}
		}
		#endregion

		#region Health
		public	RectTransform	HealthBar;	//Link Healthbar in IDE

		[SyncVar (hook="OnHealthChange")]
		int		mHealth;

		void	OnHealthChange(int vHealth) {
			mHealth = vHealth;		//Update local health variable
			HealthBar.localScale=new Vector2((float)mHealth/100f,HealthBar.localScale.y);
		}
		#endregion

	    #region Player Startup

	    public	override void OnStartClient() {
		    OnNameChange (mPlayerName);		//Make sure name is shown first time its created
        }

		public	override void OnStartServer () {
			mHealth = 100;
		}

        public	override void OnStartLocalPlayer () {
		    base.OnStartLocalPlayer ();
			GetComponent<SpriteRenderer> ().color = Color.green;	//make local player green
			MultiplayerGM.LocalPlayerShip = this;			//Update static local player
			MultiplayerGM.Score=mScore;		//Set Score In UI
	    }


        #endregion


        #region PlayerMove
		public	override void	ProcessLocalPlayer() {
			MoveLocalPlayer ();
			LocalPlayerFire();
		}

	    void	MoveLocalPlayer() {
			if (Input.GetKey (KeyCode.LeftArrow)) {      //Rotate ship, could use torque, but this looks better
                transform.localRotation *= Quaternion.Euler(0f, 0f, RotationSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
                transform.localRotation *= Quaternion.Euler(0f, 0f, -RotationSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.UpArrow)) {    //Apply force in direction of rotation
                RB.velocity += (Vector2)(transform.localRotation * Vector2.up* Speed);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {    //Apply force in direction of rotation
                RB.velocity -= (Vector2)(transform.localRotation * Vector2.up * Speed);
            }
            if (RB.velocity.magnitude > MaxSpeed) {
                RB.velocity = RB.velocity.normalized * MaxSpeed;       //Clamp speed
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
			tBulletGO.GetComponent<Rigidbody2D>().velocity=tBulletVelocity;
			tBulletGO.GetComponent<Bullet> ().mPlayerID = netId;	//Link bullet to player
			Destroy(tBulletGO, 2f);
            NetworkServer.Spawn(tBulletGO);
        }
        #endregion

		#region PlayerHit		//These must only run on server, won't work on client
		public	override	void	ProcessHit(Entity vOther) {		//As this is only called from a command , it will also process on Server
			if (!isServer) {
				DB.Error ("This must only be called on Server to work");		//Catch if this is run from client
			}
			if (vOther.EType == EntityType.Bullet) {		//Destroy bullet
				Bullet	tBullet=(Bullet)vOther;		//We have a bullet, find who fired it
				PlayerShip tPlayer=FindServerEntity(tBullet.mPlayerID) as PlayerShip;
				tPlayer.GiveScore (10);		//Give score to player who shot us
				Destroy (vOther.gameObject);
				TakeDamage (3);
			}
		}

		public	void	TakeDamage(int vAmount) {
			mHealth -= vAmount;
			if (mHealth < 0) {		//Make sure Health does not go below zero
				mHealth = 0;
			}
		}

		public	void	GiveScore(int vAmount) {
			mScore += vAmount;
		}

		#endregion

    }
}

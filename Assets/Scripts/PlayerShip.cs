using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Multiplayer {

    public class PlayerShip : Entity {
		#region Idenfication

		public	Text	NameText;		//Link Name Text here

		public	override	EntityType EType {
			get {
				return	EntityType.Player;		//Our type
			}
		}

		[SyncVar (hook="OnNameChange")]		//Hook name change so we can update UI
		string	mPlayerName="None";

		[Command]		//This will run on server to update SyncVar, and hence show the new name on all of the clients
		public void	CmdSetUserName(string vName) {
			mPlayerName = vName;		
		}

		void	OnNameChange(string vName) {
			mPlayerName = vName;		//Reflect change locally
			NameText.text=vName;	//Show on screen, in player screen canvas
		}

		public	string	PlayerName {		//Read only access to name
			get {
				return	mPlayerName;
			}
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

		public	int	PlayerScore {		//Read Only access to Score
			get {
				return	mScore;
			}
		}
		void	OnScoreChange(int vScore) {		//Will update UI and internal score
			mScore= vScore;		//Update local score variable
			if(isLocalPlayer) {
				MultiplayerGM.Score=mScore;		//Set Score In UI, but only for local player
			}
		}
		#endregion

		#region Health
		public	RectTransform	HealthBar;	//Link Healthbar in IDE
		[SyncVar (hook="OnHealthChange")]
		int		mHealth;

		void	OnHealthChange(int vHealth) {
			mHealth = vHealth;		//Update local health variable, when told to do so by server
			HealthBar.localScale=new Vector2((float)mHealth/100f,HealthBar.localScale.y);		//Quick healthbar, X scaling green covers static red
		}
		#endregion

	    #region Player Startup

	    public	override void OnStartClient() {
		    OnNameChange (mPlayerName);		//Make sure name is shown first time its created
        }

		public	override void OnStartServer () { //This runs on the server only
			mHealth = 100;		//Give Max health
		}

        public	override void OnStartLocalPlayer () {		//Runs on local player in client
		    base.OnStartLocalPlayer ();
			GetComponent<SpriteRenderer> ().color = Color.green;	//make local player green
			MultiplayerGM.LocalPlayerShip = this;			//Update static local player
			MultiplayerGM.Score=mScore;						//Set Score In UI
			transform.position=RandomPosition();
	    }

		Vector2	RandomPosition() {		//Careful, if players have differnet aspect ratio screens this will give differing results on them as apsect will not be the same
			float	tHeight = Camera.main.orthographicSize;
			float	tWidth = tHeight*Camera.main.aspect;
			return	new Vector2 (Random.Range (-tWidth, tWidth), Random.Range (-tHeight, tHeight));
		}


        #endregion

        #region PlayerMove
		public	override void	ProcessLocalPlayer() {		//Process move & fire
			if (!mStunned) {		//Disable player move & fire on stunned
				MoveLocalPlayer ();
				LocalPlayerFire ();
			}
		}

	    void	MoveLocalPlayer() {		//Move player
			if (Input.GetKey (KeyCode.LeftArrow)) {      //Rotate ship, could use torque, but this looks better
                transform.localRotation *= Quaternion.Euler(0f, 0f, RotationSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
                transform.localRotation *= Quaternion.Euler(0f, 0f, -RotationSpeed * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.UpArrow)) {    //Apply velocity in direction of rotation
                RB.velocity += (Vector2)(transform.localRotation * Vector2.up* Speed);
            }
			if (Input.GetKey(KeyCode.DownArrow)) {    //Apply velocity in direction of rotation
                RB.velocity -= (Vector2)(transform.localRotation * Vector2.up * Speed);
            }
            if (RB.velocity.magnitude > MaxSpeed) {
                RB.velocity = RB.velocity.normalized * MaxSpeed;       //Clamp speed
            }
	    }

        #endregion

        #region PlayerFire    //Client side
        Cooldown mFireCooldown = new Cooldown(0.25f);       //Cooldown Helper, 4 shots per second
        public Transform BulletSpawnPosition;       //Link in IDE
		public GameObject BulletPrefab;        //Assign in Inspector

        void    LocalPlayerFire() {
            if (mFireCooldown.Cool(Time.deltaTime) && Input.GetKey (KeyCode.Space)) {
                CmdFire();              //Work out where bullet should appear and how fast it should go
			}
        }
		#endregion

		#region Fire    //Server side

        [Command]   //These all run on server copy of Player, which is needed as only server can spawn objects
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
			Assert.IsTrue(isServer);			//Must run on server, Assert is a debug feature which is removed on non debug builds See:https://docs.unity3d.com/ScriptReference/Assertions.Assert.html
			if (vOther.EType == EntityType.Bullet) {		//Destroy bullet
				Bullet	tBullet=(Bullet)vOther;		//We have a bullet, find who fired it
				PlayerShip tPlayer=FindServerEntity(tBullet.mPlayerID) as PlayerShip;
				tPlayer.GiveScore (10);		//Give score to player who shot us
				Destroy (vOther.gameObject);
				TakeDamage (30);
			}
			if (vOther.EType == EntityType.Player) {		//If we hit other player, cause stun
				StartCoroutine (StunPlayers (this,(PlayerShip)vOther,3f));		//Stub then Unstun after 3 seconds
			}
		}

		IEnumerator	StunPlayers(PlayerShip vPS1, PlayerShip vPS2, float vTime) {		//CoRoutine to stun wait then unstun, check https://docs.unity3d.com/Manual/Coroutines.html
			vPS1.mStunned = true;							//This will run right away
			vPS2.mStunned = vPS1.mStunned;
			yield	return	new	WaitForSeconds (vTime);		//This will give suspend this CoRoutine for a period of time
			vPS1.mStunned = false;							//When time is up code will keep executing here
			vPS2.mStunned = vPS1.mStunned;
		}

		[SyncVar (hook="OnChangeStunned")]		//SyncVar to tell player they are stunned
		public	bool	mStunned;

		void	OnChangeStunned(bool vStunned) {
			mStunned = vStunned;			//Update client variable
		}

		public	void	TakeDamage(int vAmount) {		//As TakeDamage() is called from ProcessHit, we know is running on the server
			mHealth -= vAmount;
			if (mHealth < 0) {		//Make sure Health does not go below zero
				mHealth = 0;
				StartCoroutine (BoostHealth (2f));		//Pump health back up, takes 2 seconds
			}
		}

		IEnumerator	BoostHealth(float vTime) {
			mStunned = true;		//Stun player
			yield	return new WaitForSeconds (vTime);		//Wait for a number of seconds before recharge
			float	vPeriod= 1/(100*vTime);					//Period between updates
			while (mHealth < 100) {		//Pump Health back up, over a period of time for nice effect, however player stunned till its full
				mHealth++;
				yield	return new WaitForSeconds (vPeriod);		//Wait a 1/Time period between updates
			}
			mStunned = false;		//UnStun player
		}


		public	void	GiveScore(int vAmount) {		//Boost player score, must be called on Server
			mScore += vAmount;
		}

		#endregion
    }
}

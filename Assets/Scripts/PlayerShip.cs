﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.UI;

public class PlayerShip : NetworkBehaviour {

	#region IDE Set values
    [Header("User settings")]           //These will show up in inspector
    [Range(180f, 720f)]
    public float RotationSpeed = 180f;
    [Range(0, 50f)]
    public 	float Speed = 20.0f;
	[Range(0, 3f)]
	public 	float BulletSpeed = 1.0f;
	[Range(0, 3f)]
	public 	float BulletTimeToLive = 1.0f;
	public	GameObject	BulletSpawn;

	public	Text	NameText;		//Link Name Text here
	#endregion


    #region Score


	//Access Score
	int mScore = 0;     //Keep score private
    public int Score {
           get {
            return mScore;
        }
		set {
			mScore = value;
		}
    }

	//Access Lives
	int	mLives = 4;
	public	int Lives {
		get {
			return	mLives;
		}

		set {
			mLives = value;
		}
	}

	public	void	Die() {
		if (GM.Cheat) {
			GetComponent<SpriteRenderer> ().color = Color.red;		//Show cheat mode is active by making ship red
		} else {
			if (mLives > 0) {
				mLives--;
			} 
			//GM.CurrentState = GM.State.PlayerLifeLost;
		}
	}

	#endregion


	#region PlayerStates


	[SyncVar (hook="OnNameChange")]
	string	mPlayerName="None";

	bool	mActive=false;
	SpriteRenderer	mSR;
	Collider2D		mCOL;

	Vector3	mStartPosition;

	public	override void OnStartClient() {
		mRB = GetComponent<Rigidbody2D>(); //Get RB component from GameObject
		mSR=GetComponent<SpriteRenderer>();	//Cache key components for fast reuse
		mCOL = GetComponent<Collider2D> ();
		mStartPosition=transform.position;
		OnNameChange (mPlayerName);		//Make sure nam eis shown first time its created
		Show (true);
	}

	public	override void OnStartLocalPlayer () {
		base.OnStartLocalPlayer ();
		mSR.color = Color.green;		//Make player ship green, locally
		CmdSetUserName(System.Environment.UserName);		//Update this players name, with local login on server
	}

	[Command]		//This will run on server to update SyncVar
	public void	CmdSetUserName(string vName) {
		mPlayerName = vName;		
	}

	void	OnNameChange(string vName) {
		mPlayerName = vName;		//Reflect change locally
		NameText.text=vName;	//Show on screen
	}

	public	void	Show(bool vShow) {		//Show or hide ship
		mActive=vShow;
		mSR.enabled = vShow;
		mCOL.enabled = vShow;
	}

	public	void	Warp() {
		Vector2	vPosition = new Vector2 (Random.Range (-GM.WorldSize.x, GM.WorldSize.x), Random.Range (-GM.WorldSize.y, GM.WorldSize.y));
		mRB.velocity = Vector2.zero;
		transform.position=vPosition;
		transform.rotation = Quaternion.identity;
	}

	public	void	ReSpawn() {
		mSR.color = Color.white;		//Turn ship white again
		mRB.velocity = Vector2.zero;
		transform.position=mStartPosition;
		transform.rotation = Quaternion.identity;
		Show (true);
	}
	#endregion


    #region PhysicsMove
    Rigidbody2D mRB;  //Keep a reference to the RB
    //For Physics we use Fixed Update	
    void FixedUpdate() {
		if (isLocalPlayer) {
			MoveLocalPlayer ();
		}
    }

	void	MoveLocalPlayer() {
		if (mActive) {
			if (Input.GetKey (KeyCode.LeftArrow)) {      //Rotate ship, could use torque, but this looks better
				mRB.MoveRotation (mRB.rotation + (RotationSpeed * Time.deltaTime));
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				mRB.MoveRotation (mRB.rotation - (RotationSpeed * Time.deltaTime));
			}
			if (Input.GetKey (KeyCode.UpArrow)) {    //Apply force in direction of rotation
				Vector2 tForce = Quaternion.Euler (0, 0, mRB.rotation) * Vector2.up * Time.deltaTime * Speed;
				mRB.AddForce (tForce);
			}
			/*
if (CoolDown () && Input.GetKey (KeyCode.Space)) {
				GM.CreateBullet (BulletSpawn.transform.position, (BulletSpawn.transform.position - transform.position).normalized * BulletSpeed,BulletTimeToLive);
			}
			if (Input.GetKey (KeyCode.W)) {      //Warp
				if (GM.CurrentState == GM.State.PlayLevel) {
					GM.CurrentState = GM.State.WarpPlayer;
				}
			}
			*/
		}
			}

    #endregion


	#region Cooldown
	public	float	Fire=0.25f;

	float	mFireTimer=0f;

	bool	CoolDown() {
		if(mFireTimer>=Fire) {
			mFireTimer = 0;
			return	true;
		}
		mFireTimer += Time.deltaTime;
		return	false;
	}
	#endregion

}
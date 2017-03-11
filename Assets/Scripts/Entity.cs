using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;

[RequireComponent(typeof(Rigidbody2D))]
	
public abstract	class Entity : NetworkBehaviour {

	public	enum EntityType {
		None
		,Player
		,Bullet
	}

	public	abstract	EntityType EType{get;}		//Quick way to get object type, better than tags

	NetworkInstanceId mNetID;
	public	NetworkInstanceId NetID {	//Read only access to Unique Net ID
		get {
			return	mNetID;
		}
	}

	Rigidbody2D mRB;				//As we have collisions, everthign needs a RigidBody2D
	public	Rigidbody2D RB {
		get {
			return	mRB;
		}
	}

	void Update () {
		if (isLocalPlayer) {
			ProcessLocalPlayer ();
		}
	}

	public	virtual void	ProcessLocalPlayer() {
	}

	void	Start() {
		mRB = GetComponent<Rigidbody2D> ();
		mNetID = GetComponent<NetworkIdentity> ().netId;
	}


	void OnTriggerEnter2D(Collider2D vOther) {
		if (isServer) {		//Only process collisions on server
			if (EType != EntityType.Bullet) {	//Process hits from players perspective, ie ignore bullet-player collision
				Entity tOtherEntity = vOther.GetComponent<Entity> ();
				if (tOtherEntity != null) {
					CmdHitBy (netId, tOtherEntity.NetID);
				} else {
					DB.ErrorFormat ("Collision with non Entity {0) ignored", vOther.name);
				}
			}
		}
	}

	[Command]
	private void	CmdHitBy(NetworkInstanceId vMe,NetworkInstanceId vOther) {
		Entity tMeEntity = FindServerEntity(vMe);
		Entity tOtherEntity = FindServerEntity (vOther);
		if(tMeEntity!=null && tOtherEntity!=null) {
			tMeEntity.ProcessHit (tOtherEntity);
		} else {
			DB.ErrorFormat ("CmdHitBy({0},{1}) Entities not found on server",vMe,vOther);
		}
	}

	public	virtual	void	ProcessHit(Entity vOther) {
		DB.MsgFormat ("Default {0} hit by {1}", EType, vOther.EType);
	}

	public	static	Entity FindServerEntity(NetworkInstanceId vNetID) {
		GameObject tMe = NetworkServer.FindLocalObject (vNetID);		//Get Server version of Object
		if (tMe != null) {
			return	tMe.GetComponent<Entity> ();
		}
		return	null;
	}
}

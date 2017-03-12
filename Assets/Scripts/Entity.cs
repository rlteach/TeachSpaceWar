using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using RL_Helpers;
using UnityEngine.Assertions;

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

	Rigidbody2D mRB;				//As we have collisions, everthing needs a RigidBody2D, well done Unity!
	public	Rigidbody2D RB {		//Read only access to Rigidbody2D
		get {
			return	mRB;
		}
	}

	void Update () {
		if (isLocalPlayer) {
			ProcessLocalPlayer ();		//Only process GameObject if its local
		}
	}

	public	virtual void	ProcessLocalPlayer() {} //This will do nothing, generally overridden

	void	Start() {		//Get references to common components
		mRB = GetComponent<Rigidbody2D> ();
		mNetID = GetComponent<NetworkIdentity> ().netId;
	}


	void OnTriggerEnter2D(Collider2D vOther) {
		if (isServer) {		//Only process collisions on server
			if (EType != EntityType.Bullet) {	//Process hits from players perspective, ie ignore bullet-player collision
				Entity tOtherEntity = vOther.GetComponent<Entity> ();
				Assert.IsNotNull (tOtherEntity);		//If this fails we have a non Enity object in scene
				CmdHitBy (netId, tOtherEntity.NetID);
			}
		}
	}

	[Command]
	private void	CmdHitBy(NetworkInstanceId vMe,NetworkInstanceId vOther) {	//This is processed on the server, will get the server side GameObjects, and their Entites
		Entity tMeEntity = FindServerEntity(vMe);
		Assert.IsNotNull (tMeEntity);
		Entity tOtherEntity = FindServerEntity (vOther);
		Assert.IsNotNull (tOtherEntity);
		tMeEntity.ProcessHit (tOtherEntity);
	}

	public	virtual	void	ProcessHit(Entity vOther) {
		DB.MsgFormat ("Default {0} hit by {1}", EType, vOther.EType);		//Default, this is normally overridden
	}

	public	static	Entity FindServerEntity(NetworkInstanceId vNetID) {		//Use unique NetID to find GameObject and Entity
		GameObject tMe = NetworkServer.FindLocalObject (vNetID);		//Get Server version of Object
		if (tMe != null) {
			return	tMe.GetComponent<Entity> ();
		}
		return	null;		//Not found
	}
}

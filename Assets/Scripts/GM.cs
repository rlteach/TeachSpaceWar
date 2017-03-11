using UnityEngine;
using System.Collections;
using RL_Helpers;
using UnityEngine.Networking;

public class GM : Singleton {

	static	GM	sGM;


	// Run once only initalisation
	void Awake () {
		if (CreateSingleton (ref sGM)) {
			mMainCamera = Camera.main;
			mWorldSize = new Vector2 (mMainCamera.aspect * mMainCamera.orthographicSize, mMainCamera.orthographicSize);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region Debug
	bool	mCheat=false;
	public static bool	Cheat {
		get {
			return	sGM.mCheat;
		}
	}
	#endregion

	#region WorldInfo
	Camera	mMainCamera;				//Cached version, saves a scene search
	Vector2	mWorldSize;
	public	static	Vector2 WorldSize {		//Global World Size, for easy & consistent access
		get {
			return	sGM.mWorldSize;
		}
	}
    #endregion


}

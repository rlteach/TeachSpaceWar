using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PhysicsWrap : NetworkBehaviour {


	Rigidbody2D	mRB;
	void	Start() {
		mRB = GetComponent<Rigidbody2D> ();
	}

	// LateUpdate is called once per frame, after all other processing is done
	void LateUpdate () {
		float tWidth = GM.WorldSize.x;
		float tHeight = GM.WorldSize.y;       //Height
		if (mRB.position.y > tHeight) {
			mRB.position += Vector2.down * tHeight * 2f;
        }
		if (mRB.position.y < -tHeight) {
			mRB.position += Vector2.up * tHeight * 2f;
        }

		if (mRB.position.x > tWidth) {
			mRB.position += Vector2.left * tWidth * 2f;
        }

		if (mRB.position.x < -tWidth) {
			mRB.position += Vector2.right * tWidth * 2f;
        }
    }
}

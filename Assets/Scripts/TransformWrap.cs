using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TransformWrap : NetworkBehaviour {

	// LateUpdate is called once per frame, after all other processing is done
	void LateUpdate () {
        if(isLocalPlayer) {
		    float tWidth = GM.WorldSize.x;
		    float tHeight = GM.WorldSize.y;       //Height
		    if (transform.position.y > tHeight) {
                transform.position += Vector3.down * tHeight * 2f;
            } else  if (transform.position.y < -tHeight) {
                transform.position += Vector3.up * tHeight * 2f;
            }

		    if (transform.position.x > tWidth) {
                transform.position += Vector3.left * tWidth * 2f;
            } else  if (transform.position.x < -tWidth) {
                transform.position += Vector3.right * tWidth * 2f;
            }
        }
    }
}

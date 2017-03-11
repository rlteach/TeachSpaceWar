using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Multiplayer { 


    public class TransformWrap : NetworkBehaviour {
        Vector2 mBounds;

        private void Start() {
            mBounds = new Vector2(Camera.main.aspect * Camera.main.orthographicSize, Camera.main.orthographicSize);
        }

        // LateUpdate is called once per frame, after all other processing is done
        void LateUpdate () {
            if (isLocalPlayer) {
                float tWidth = mBounds.x;
                float tHeight = mBounds.y;       //Height
                if (transform.position.y > tHeight) {
                    transform.position += Vector3.down * tHeight * 2f;
                }
                else if (transform.position.y < -tHeight) {
                    transform.position += Vector3.up * tHeight * 2f;
                }

                if (transform.position.x > tWidth) {
                    transform.position += Vector3.left * tWidth * 2f;
                }
                else if (transform.position.x < -tWidth) {
                    transform.position += Vector3.right * tWidth * 2f;
                }
            }
        }
    }
}
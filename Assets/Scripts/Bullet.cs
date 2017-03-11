using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Multiplayer {

    public class Bullet : NetworkBehaviour {
        public override void OnStartClient() {
            base.OnStartClient();
	    }
    }
}

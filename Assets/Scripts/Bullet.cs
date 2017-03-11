using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Multiplayer {


	public class Bullet : Entity {

		public	override	EntityType EType {
			get {
				return	EntityType.Bullet;
			}
		}

		public	NetworkInstanceId mPlayerID;

        public override void OnStartClient() {
            base.OnStartClient();
	    }
    }
}

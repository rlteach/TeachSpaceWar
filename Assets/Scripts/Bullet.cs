using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Multiplayer {
	public class Bullet : Entity {
		public	override	EntityType EType {      //Type
			get {
				return	EntityType.Bullet;
			}
		}
		public	NetworkInstanceId mPlayerID;        //Link to player who fired me
    }
}

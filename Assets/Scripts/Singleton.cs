using UnityEngine;
using System.Collections;

//Inhert from this to get a static game manager singleton
namespace RL_Helpers {      //Default namespace for helper code

    abstract public class Singleton : MonoBehaviour {

		#region Singleton
        protected bool CreateSingleton<T>(ref T sGM) where T : Singleton {  //Set Up singleton for a Type
            if (sGM == null) {
                sGM = (T)this;
                DontDestroyOnLoad(gameObject);
                return true;        //Signal back if this is the first time this has been created
            } else if (sGM != this) {
                Destroy(gameObject);
				DB.MsgFormat("Subsequent creation of: {0} ignored",this.GetType().Name);
            }
            return false;   //Don't do it twice
        }
		#endregion
   }
	#region Debug
	public	class DB
	{
		static public bool Show = true;    //Show Debug messages

		//Allows debug string to be output, but allows this to turned off anythere in code by clearing ShowDebug 
		public static void MsgFormat(string vFormat, params object[] vArgs) {
			if (Show) {
				Debug.LogFormat(vFormat,vArgs);
			}
		}
		public static void Msg(string vMessage) {
			if (Show) {
				Debug.Log(vMessage);
			}
		}
		public static void Error(string vMessage) {
			if (Show) {
				Debug.LogError(vMessage);
			}
		}
		public static void ErrorFormat(string vFormat, params object[] vArgs) {
			if (Show) {
				Debug.LogErrorFormat(vFormat,vArgs);
			}
		}
	}
	#endregion
}
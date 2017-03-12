using UnityEngine;
using System.Collections;

namespace RL_Helpers {


    #region Debug
    public class DB {
        static public bool Show = true;    //Show Debug messages

        //Allows debug string to be output, but allows this to turned off anythere in code by clearing ShowDebug 
        public static void MsgFormat(string vFormat, params object[] vArgs) {
            if (Show) {
                Debug.LogFormat(vFormat, vArgs);
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
                Debug.LogErrorFormat(vFormat, vArgs);
            }
        }
    }
        #endregion
}

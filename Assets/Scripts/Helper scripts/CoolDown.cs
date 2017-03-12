using UnityEngine;

namespace RL_Helpers {

    //Simple  cooldown class, returns true on cooldown, needs Time deltas passing in
    public class Cooldown {
        float mCoolDownTime;
        float mCurrentTime;
        public Cooldown(float vTimeOut = 1f) {
            mCoolDownTime = vTimeOut;
            mCurrentTime = mCoolDownTime;
        }

        public bool Cool(float vTimeDelta) {
            if (mCurrentTime + vTimeDelta >= mCoolDownTime) {
                mCurrentTime = 0f;
                return true;    //Timeout expired, set for next one
            }
            mCurrentTime += vTimeDelta;
            return false;       //Not yet Expired
        }
    }
}
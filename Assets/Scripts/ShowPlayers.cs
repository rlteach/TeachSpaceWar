using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

namespace Multiplayer {
	public class ShowPlayers : MonoBehaviour {

		public	Text	PlayersText;		//Link in IDE

		// Use this for initialization
		void Start () {
			StartCoroutine (ShowPlayerList ());
		}

		IEnumerator	ShowPlayerList(float vTime=0.5f) {		//Updates every half second
			do {
				PlayerShip[]	tPlayers=FindObjectsOfType<PlayerShip>(); //Get array of all Player ships
				StringBuilder tSB=new StringBuilder();
				SortPlayersByScore(tPlayers);
				tSB.AppendLine("<color=#ff00ffff> Scores </color>");		//Use rich text colour codes
				foreach(PlayerShip tPS in tPlayers) {
					if(tPS.isLocalPlayer) {
						tSB.AppendFormat("<color=#00ff00FF> {0} {1} </color>\n",tPS.PlayerName,tPS.PlayerScore);		//Own Player
					} else {
						tSB.AppendFormat("<color=#e0e0e0FF> {0} {1} </color>\n",tPS.PlayerName,tPS.PlayerScore);		//Other Players
					}
				}
				PlayersText.text=tSB.ToString();
				yield return new WaitForSeconds(vTime);		//Return control for set time
			} while(true);
		}

		void	SortPlayersByScore(PlayerShip[] tShips) {		//Simple bubble sort, decending order
			if (tShips.Length > 1) {
				bool	tSwap;
				do {				//Bubblesort with early exit if sorted
					tSwap=false;
					for(int tI=0;tI<tShips.Length-1;tI++) {
						if(tShips[tI].PlayerScore<tShips[tI+1].PlayerScore) {		//If next score higher than last current one, swap
							PlayerShip tTPS=tShips[tI];
							tShips[tI]=tShips[tI+1];
							tShips[tI+1]=tTPS;
							tSwap=true;		//Still swapping
						}
					}
				} while(tSwap);		//Will fall through is no swaps were needed
			}
		}
	}
}
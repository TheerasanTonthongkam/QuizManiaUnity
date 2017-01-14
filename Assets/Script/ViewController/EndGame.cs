using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UniRx {
	public class EndGame : MonoBehaviour {

		public InputField inputText;
		public Text point;
		public Text result;
		public Button submitButton;

		public Memory memory;
		public Navigator navigator;


		System.IDisposable observable;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void sendData() {
			submitButton.interactable = false;
			string url = "http://localhost:8080/rank/addRank?score=" + point.text + "&name=" + inputText.text;
			observable = ObservableWWW.Get (url)
				.Subscribe (j => submitDone() ,
					ex => Debug.Log("error " + ex));
		}

		void OnDestroy() {
			if (observable != null) {
				observable.Dispose ();
			}
		}

		void submitDone() {
			GameData data = memory.getGameData();
			data.endGame = true;
			if (result.text.Equals("Win")) {
				data.winTime++;
			}
			if (result.text.Equals("Lose")) {
				data.loseTime++;
			}
			memory.setGameData (data);
			navigator.gotoRankingcreen();
		}
	}
}

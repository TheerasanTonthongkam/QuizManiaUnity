using UnityEngine;
using System.Collections;

namespace UniRx.Examples{
	public class Try : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public void OnClickTry() {
			ObservableWWW.Get ("https://private-e1f58-quizmania.apiary-mock.com/ranking")
				.Subscribe (j => initData(j),
					ex => Debug.Log("error " + ex));
		}

		public void initData(string jsonString) {
			Debug.Log (jsonString);
			RankingSummary rankingSummary = JsonUtility.FromJson<RankingSummary> (jsonString);
			Debug.Log ("PlayTime " + rankingSummary.allRank.Length);
		}
	}
}


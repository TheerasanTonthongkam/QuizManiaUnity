using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UniRx {
	public class RankingViewController : MonoBehaviour {

		[Header("Ranking Summary UI")]
		public Text winTimeText;
		public Text loseTimeText;
		public GameObject loadingDialog;

		[Header("Scroll View")]
		public RectTransform content;
		public GameObject itemView;
		public int height = 150;
		private int spacing = 10;

		public Memory memory;

		System.IDisposable observable;

		// Use this for initialization
		void Start () {
			initData ();
			GameData data = memory.getGameData ();
			winTimeText.text = data.winTime.ToString ();
			loseTimeText.text = data.loseTime.ToString();
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void initData() {
			loadingDialog.SetActive (true);
			observable = ObservableWWW.Get ("http://localhost:8080/rank/allRank")
				.Subscribe (j => inflateDataToView(j),
					ex => Debug.Log("error " + ex));
		}

		void inflateDataToView(string data) {
			loadingDialog.SetActive (false);
			data = "{ \"allRank\": " + data + "}";
			RankingSummary rankingSummary = JsonUtility.FromJson<RankingSummary> (data);
			renderList (rankingSummary.allRank);
		}

		void renderList(Ranking[] ranking) {

			float contentHeight = ranking.Length * ((height) + spacing);
			content.sizeDelta = new Vector2(0, contentHeight);

			for (int i = 0; i < ranking.Length ; i++)
			{
				GameObject g = (GameObject) Instantiate(itemView, this.transform.position, Quaternion.identity);
				g.transform.SetParent(content);
				RectTransform rect = g.GetComponent(typeof(RectTransform)) as RectTransform;
				rect.offsetMin = new Vector2(0, 0);
				rect.offsetMax = new Vector2(0, ((height + spacing) * i * -1));
				rect.sizeDelta = new Vector2(0, height);
				RankingItem item = g.GetComponent(typeof(RankingItem)) as RankingItem;
				item.initialData(ranking[i], i);
				g.transform.localScale = new Vector3(1, 1, 1);
			}
		}

		void OnDestroy() {
			if (observable != null) {
				observable.Dispose ();
			}
		}
	}
}

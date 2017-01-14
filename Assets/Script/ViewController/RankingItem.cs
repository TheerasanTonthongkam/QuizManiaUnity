using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RankingItem : MonoBehaviour {

	[Header("UI")]
	public Text rankText;
	public Text nameText;
	public Text pointText;

	public void initialData(Ranking ranking, int index) {
		rankText.text = (index + 1).ToString ();
		nameText.text = ranking.name;
		pointText.text = ranking.score + "";
	}
}

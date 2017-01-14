using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Navigator : MonoBehaviour {
	
	public void gotoMainScreen() {
		SceneManager.LoadScene ("Main");
	}

	public void gotoQuizScreen() {
		SceneManager.LoadScene ("Quiz");
	}

	public void gotoRankingcreen() {
		SceneManager.LoadScene ("Ranking");
	}
}

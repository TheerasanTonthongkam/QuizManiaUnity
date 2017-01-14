using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UniRx {
	public class QuizController : MonoBehaviour {

		System.IDisposable observable;

		[Header("Memory")]
		public Memory memory;

		[Header("Quiz UI")]
		public Text[] questionText;
		public RawImage image;
		public GameObject loadingGroup;

		[Header("View Group")]
		public GameObject quizWithImage;
		public GameObject quizOnly;

		[Header("Choices text")]
		public Text[] choices;

		[Header("Quiz Progress")]
		public Text progressText;
		public Text pointText;
		public Slider slider;
		public Image clock;

		[Header("End Game Dialog")]
		public GameObject endGameDialog;
		public Text resultText;
		public Text endPointText;
		public InputField inputText;
		public Button submitScoreButton;
		public Color winColor;
		public Color loseColor;
		public Color dueColor;

		private int quizIndex;
		private QuizList quizList;
		private string answer;
		private int questionSize;

		private int maxTime = 5;
		private float currentPointOftime;
		private bool isStartCountDown = false;
		float timeRatio;

		private int point = 0;
		private float status = 0;

		private GameData data;

		// Use this for initialization
		void Start () {
			data = new GameData();
			if (memory.hasSave () 
				&& memory.getGameData ().currentQuiz < memory.getGameData().quizList.quizzes.Length 
				&& !memory.getGameData().endGame) {
				data = memory.getGameData ();
				quizIndex = data.currentQuiz;
				quizList = data.quizList;
				point = data.currentPoint;
				status = data.currentStatus;
				questionSize = quizList.quizzes.Length;
				Debug.Log ("Load  W + " + this.data.winTime + " : L " + this.data.loseTime);
				slider.value = status;
				initQuiz (quizIndex);
			} else {

				if (memory.hasSave ()) {
					data.winTime = memory.getGameData ().winTime;
					data.loseTime = memory.getGameData ().loseTime;
				}

				quizIndex = 0;
				initData ();
			}
				

			slider.interactable = false;
			currentPointOftime = Time.time;
			applyPoint (point);
		}
		
		// Update is called once per frame
		void Update () {
			if (isStartCountDown) {
				timeRatio = ((Time.time - currentPointOftime) / maxTime);
				if (timeRatio <= 1) {
					clock.fillAmount = 1 - timeRatio;
				} else {
					currentPointOftime = Time.time;
					if (slider.value > 0) {
						slider.value = 0;
					}

					slider.value--;
					quizIndex++;
					if (!isEndGame (slider.value)) {
						initQuiz (quizIndex);
					} else {
						endGame (slider.value, quizIndex);
					}
				}
			}
				
			submitScoreButton.interactable = !inputText.text.Equals ("");

		}

		void initData() {
			loadingGroup.SetActive (true);
			observable = ObservableWWW.Get ("http://localhost:8080/Quiz/getQuizzes?rate=" + (data.winTime - data.loseTime))
				.Subscribe (j => inflateDataToView(j),
					ex => Debug.Log("error " + ex));
		}

		void inflateDataToView(string data) {
			loadingGroup.SetActive (false);
			data = "{\"quizzes\":" + data + "}";
			quizList = JsonUtility.FromJson<QuizList> (data);
			questionSize = quizList.quizzes.Length;
			this.data.quizList = quizList;
			this.data.endGame = false;
			initQuiz (quizIndex);
		}

		void initQuiz(int i) {
			if (isOutOfQuestion () || i >= quizList.quizzes.Length) {
				endGame (slider.value, i);
				Debug.Log ("End game");
			} else {
				this.data.currentQuiz = i;
				Quiz q = quizList.quizzes [i];

				if (q.imageUrl != null && !q.imageUrl.Equals ("")) {
					quizWithImage.SetActive (true);
					quizOnly.SetActive (false);
					StartCoroutine (setImage (q, i));
				} else {
					quizWithImage.SetActive (false);
					quizOnly.SetActive (true);
					startCountDown ();
					setAllTextToComponant (q, i);
				}


			}
		}

		void clearAllText() {
			foreach (Text t in questionText) {
				t.text = "";
			}

			foreach (Text t in choices) {
				t.text = "";
			}
		}

		void setAllTextToComponant(Quiz q, int index) {
			foreach (Text t in questionText) {
				t.text = q.question;
			}

			ArrayList arr = new ArrayList ();
			arr.Add (q.correctAnswer);
			arr.Add (q.wrongAnswer1);
			arr.Add (q.wrongAnswer2);
			arr.Add (q.wrongAnswer3);
			answer = arr [0] as string;

			int randomIndex;
			foreach (Text t in choices) {
				randomIndex = Random.Range (0, (arr.Count - 1));
				t.text = arr [randomIndex] as string;
				arr.RemoveAt (randomIndex);
			}

			progressText.text = index + 1 + "/" + questionSize;
			this.data.currentQuiz = index;
			this.data.currentStatus = slider.value;
			memory.setGameData (this.data);
		}

		void OnDestroy() {
			if (observable != null) {
				observable.Dispose ();
			}
		}

		public void SendAnswer(int i) {
			stopCountDown ();
			quizIndex++;
			if (choices [i].text.Equals (answer)) {
				if (slider.value < 0) {
					slider.value = 0;
				}

				slider.value++;
				applyPoint ((int) (100 * slider.value));
			} else {
				if (slider.value > 0) {
					slider.value = 0;
				}

				slider.value--;

			}

			this.data.currentPoint = point;
			this.data.currentQuiz = quizIndex;
			this.data.currentStatus = slider.value;
			memory.setGameData (this.data);

			if (!isEndGame (slider.value)) {
				initQuiz (quizIndex);
			} else {
				endGame (slider.value, quizIndex);
			}
		}

		IEnumerator setImage(Quiz q, int index) {
			image.texture = null;
			loadingGroup.SetActive (true);
			clearAllText ();
			WWW www = new WWW(q.imageUrl);
			yield return www;
		
			image.texture = www.texture;
			image.SetNativeSize ();
			float ratio = Screen.width / image.rectTransform.rect.width ;
			image.rectTransform.sizeDelta = new Vector2 (image.rectTransform.rect.width * ratio, 
				image.rectTransform.rect.height * ratio);
			startCountDown ();
			loadingGroup.SetActive (false);
			setAllTextToComponant (q, index);
		}

		bool isEndGame(float value) {
			return value == -3 || value == 5 || isOutOfQuestion();
		}

		bool isOutOfQuestion() {
			return quizIndex >= quizList.quizzes.Length;
		}

		void endGame(float value, int quizIndex) {
			endGameDialog.SetActive (true);

			if (value == -3) {
				stopCountDown ();
				resultText.text = "Lose";
				resultText.color = loseColor;
			}
			if (value == 5) {
				applyPoint ((int) (100000 / Mathf.Pow (quizIndex, 2)));
				resultText.text = "Win";
				resultText.color = winColor;
			}

			if (isOutOfQuestion()) {
				resultText.text = "End of Question";
				resultText.color = dueColor;
			}


			endPointText.text = "Point: " + point;
			memory.setGameData (data);
		}

		void startCountDown() {
			isStartCountDown = true;
			currentPointOftime = Time.time;
		}

		void stopCountDown() {
			isStartCountDown = false;
		}

		void applyPoint (int p) {
			point += p;
			pointText.text = point.ToString ();
		}
	}
}

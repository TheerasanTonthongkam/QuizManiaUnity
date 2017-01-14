using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData {
	public QuizList quizList;
	public int winTime = 0;
	public int loseTime = 0;
	public int currentPoint = 0;
	public int currentQuiz = 0;
	public float currentStatus = 0;
	public bool endGame = false;
}
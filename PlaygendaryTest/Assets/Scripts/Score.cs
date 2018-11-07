﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDuringGame : MonoBehaviour
{
    [SerializeField]
    Text scoreText;
    [SerializeField]
    AudioSource audio;


    private int score;


    #region Unity lifecycle

    private void Awake()
    {
        scoreText.text = "0";

        Player.OnScoreUp += Score_OnScoreUp;
        Player.OnEndGame += Score_OnEndGame;
    }


    private void OnDestroy()
    {
        Player.OnScoreUp -= Score_OnScoreUp;
        Player.OnEndGame -= Score_OnEndGame;
    }

    #endregion


    #region Event handlers

    private void Score_OnScoreUp()
    {
        score++;

        scoreText.text = score.ToString();

        audio.Play();
    }


    private void Score_OnEndGame()
    {
        gameObject.SetActive(false);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float score;
    public float crystal;
    public UIManager uiManager;


    private void Start()
    {
        uiManager.UpdateScoreText(score, crystal);
    }

    public void AddScore(float s)
    {
        score += s;
        uiManager.UpdateScoreText(score, crystal);
    }

    public void AddCrystal(float c)
    {
        crystal += c;
        uiManager.UpdateScoreText(score, crystal);
    }

    public void UseCrystal(float c)
    {
        crystal -= c;
        uiManager.UpdateScoreText(score, crystal);
    }
}

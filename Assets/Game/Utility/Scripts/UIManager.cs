using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public RectTransform healthBar;
    public TextMeshProUGUI interactText;

    [Space]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI crystalText;

    public void UpdateAmmoText(int cur, int tot)
    {
        ammoText.text = cur + " | " + tot;
    }
    
    public void ClearAmmoText()
    {
        ammoText.text = "-- | --";
    }

    public void UpdateHealthBar(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    public void UpdateInteractText(string text = "")
    {
        interactText.text = text;
    }

    public void UpdateScoreText(float score, float crystal)
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
        crystalText.text = "Crystal: " + Mathf.FloorToInt(crystal);
    }
}

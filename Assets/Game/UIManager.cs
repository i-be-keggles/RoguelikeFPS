using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public RectTransform healthBar;

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
}

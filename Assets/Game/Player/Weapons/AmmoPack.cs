using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AmmoPack : MonoBehaviour
{
    public float ammoMult = 1;

    private Interactable interactable;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.interactedWith += OnPickup;
    }

    public void OnPickup(object sender, EventArgs e)
    {
        PlayerWeaponHandler p = FindObjectOfType<PlayerWeaponHandler>();
        foreach(Weapon weapon in p.weapons)
        {
            weapon.totalAmmo += Mathf.FloorToInt(weapon.startingAmmo * ammoMult);
        }
        p.ui.UpdateAmmoText(p.weapons[p.activeWeapon].curAmmo, p.weapons[p.activeWeapon].totalAmmo);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public Transform cam;

    public LayerMask hitMask;

    public List<Weapon> weapons = new List<Weapon>();
    private int activeWeapon = 0;

    public void Update()
    {
        if(weapons.Count > 0)
        {
            if (weapons[activeWeapon].semiAuto && Input.GetButtonDown("Fire1") || !weapons[activeWeapon].semiAuto && Input.GetButton("Fire1"))
            {
                weapons[activeWeapon].Fire(this);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                weapons[activeWeapon].StartCoroutine(weapons[activeWeapon].Reload(this));
            }
        }
    }

    public void DiscardWeapon()
    {
        weapons[activeWeapon].OnDiscard();
        weapons.RemoveAt(activeWeapon);
    }
}

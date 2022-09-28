using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public Transform cam;
    public Transform weaponHolder;

    public LayerMask hitMask;
    public LayerMask weaponMask;

    public float pickupRange;
    public float weaponThrowForce;

    public List<Weapon> weapons = new List<Weapon>();
    private int activeWeapon = 0;


    private void Start()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(i == 0);
        }
    }

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
            if(weapons.Count > 1 && (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetKeyDown(KeyCode.Alpha1)))
            {
                SwitchWeapon();
            }

            if (Input.GetKeyDown(KeyCode.X)) DropWeapon();
        }

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, pickupRange, weaponMask))
        {
            Weapon weapon = hit.collider.GetComponentInParent<Weapon>();
            
            if(weapon != null)
            {
                //display text
                print("Targeting weapon");
                if (Input.GetKeyDown(KeyCode.E) && weapons.Count < 2)
                {
                    PickupWeapon(weapon);
                }
            }
        }
    }

    public void DiscardWeapon()
    {
        weapons[activeWeapon].OnDiscard();
        weapons.RemoveAt(activeWeapon);
        
        if(weapons.Count > 0) SwitchWeapon();
    }

    public void SwitchWeapon()
    {
        Weapon cur = weapons[activeWeapon];
        cur.gameObject.SetActive(false);
        if (cur.reloading)
        {
            cur.StopAllCoroutines();
            cur.reloading = false;
        }

        if(weapons.Count > 1) activeWeapon = 1 - activeWeapon;
        weapons[activeWeapon].gameObject.SetActive(true);
    }

    public void PickupWeapon(Weapon weapon)
    {
        weapon.anim.enabled = true;

        weapon.rb.isKinematic = true;
        weapon.transform.SetParent(weaponHolder);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localEulerAngles = Vector3.zero;

        Collider[] cols = weapon.GetComponentsInChildren<Collider>();

        foreach(Collider col in cols) col.enabled = false;

        if (weapons.Count > 1 && weapons[activeWeapon] != null) DropWeapon();
        weapons.Insert(0, weapon);

        if(weapons.Count > 1)
        {
            activeWeapon++;
            SwitchWeapon();
        }
    }

    public void DropWeapon()
    {
        Weapon weapon = weapons[activeWeapon];

        weapon.anim.enabled = false;

        weapon.transform.SetParent(null);
        weapon.rb.isKinematic = false;
        weapon.rb.AddForce(cam.transform.forward * weaponThrowForce);

        Collider[] cols = weapon.GetComponentsInChildren<Collider>();
        foreach (Collider col in cols) col.enabled = true;

        if (weapon.reloading)
        {
            weapon.StopAllCoroutines();
            weapon.reloading = false;
        }

        weapons.RemoveAt(activeWeapon);

        if (weapons.Count > 0) weapons[activeWeapon].gameObject.SetActive(true);
    }
}

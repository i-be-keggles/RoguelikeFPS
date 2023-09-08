using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string name;

    public int damage;
    public float fireRate;
    private float timeToFire;
    public float range;

    public bool semiAuto;

    [Space]

    [Tooltip("Max ammo loaded")]            public int magazineSize;
    [Tooltip("Current ammo loaded")]        public int curAmmo;
    [Tooltip("Current total ammo carried")] public int totalAmmo;
    [Tooltip("Initial total ammo carried")] public int startingAmmo;

    [Space]

    public float reloadTime;
    public bool reloading;
    public Animator anim;

    [Space]

    public float recoilV;
    public float recoilH;

    [Space]
    public Transform muzzleFlashPoint;
    public GameObject muzzleFlash;
    public GameObject hitFX;

    public Rigidbody rb;
    public float hitImpactForce;

    [Space]
    public PlayerWeaponHandler weaponHandler;
    public Interactable interactable;

    void Start()
    {
        curAmmo = magazineSize;

        totalAmmo += startingAmmo; //move to on pickup

        rb = GetComponent<Rigidbody>();

        weaponHandler = FindObjectOfType<PlayerWeaponHandler>();
        interactable = gameObject.AddComponent<Interactable>();
        interactable.promptText = "Pickup " + name;
        interactable.interactedWith += OnPickup;
    }

    void Update()
    {
        if (!enabled) return;

        if (timeToFire > 0) timeToFire -= Time.deltaTime;
    }

    public void OnPickup(object sender, System.EventArgs e)
    {
        weaponHandler.PickupWeapon(this, this);
    }

    public void Fire(PlayerWeaponHandler player)
    {
        if (timeToFire > 0 || reloading) return;

        if(curAmmo < 1)
        {
            StartCoroutine(Reload(player));
            return;
        }

        timeToFire = 1f / fireRate;
        curAmmo--;
        Destroy(Instantiate(muzzleFlash, muzzleFlashPoint), 1.5f);
        //playSound;
        anim.SetTrigger("Shoot");
        weaponHandler.ui.UpdateAmmoText(curAmmo, totalAmmo);
        weaponHandler.recoil.Recoil(this);

        RaycastHit hit;
        if(Physics.Raycast(player.cam.position, player.cam.forward, out hit, range, player.hitMask))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
            if (enemy != null) enemy.TakeDamage(damage);
            else
            {
                GameObject _hitFX = Instantiate(hitFX, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(_hitFX, 2f);
            }

            if(rb != null)
            {
                rb.AddForce(player.cam.forward * hitImpactForce);
            }
        }

        if(curAmmo < 1)
        {
            StartCoroutine(Reload(player));
        }
    }

    public IEnumerator Reload(PlayerWeaponHandler player)
    {
        if (totalAmmo < 1)
        {
            if (curAmmo < 1) player.DiscardWeapon();
            StopAllCoroutines();
        }

        reloading = true;

        anim.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        int ammo = Mathf.Min(magazineSize-curAmmo, totalAmmo);
        curAmmo += ammo;
        totalAmmo -= ammo;

        reloading = false;

        weaponHandler.ui.UpdateAmmoText(curAmmo, totalAmmo);
    }

    public void OnDiscard()
    {
        anim.SetTrigger("Discard");
        Destroy(gameObject, 2f);
    }
}

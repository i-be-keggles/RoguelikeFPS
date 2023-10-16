using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensePod : MonoBehaviour
{
    public int damage;
    public float fireRate;
    public int maxAmmo;
    public int ammo;
    public float viewRange;
    public float spread;

    public float turnSpeed;
    public float fireAngle;
    protected float timeToFire;

    public Vector2 xLimit;
    public LayerMask enemyMask;
    public LayerMask hitMask;

    public Transform head;
    public Transform arms;
    public Transform barrellEnd;

    public Enemy target;

    public GameObject tracer;
    protected Interactable interactable;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        ammo = maxAmmo;
    }


    private void Update()
    {
        timeToFire -= Time.deltaTime;
        if (target == null) EvaluateTarget();
        else
        {
            Vector3 dir = target.CentrePoint() - arms.position;
            float angleTo = Vector3.Angle(arms.forward, dir);
            if (angleTo > fireAngle / 3)
            {
                head.eulerAngles = Vector3.Lerp(head.eulerAngles, new Vector3(head.eulerAngles.x, head.eulerAngles.y + Vector3.SignedAngle(head.forward, dir, Vector3.up), head.eulerAngles.z), turnSpeed * Time.deltaTime);
                arms.eulerAngles += new Vector3(Vector3.SignedAngle(arms.forward, dir, arms.right), 0) * turnSpeed * Time.deltaTime;
                arms.eulerAngles = new Vector3(Mathf.Clamp(arms.eulerAngles.x, xLimit.x, xLimit.y), head.eulerAngles.y, 0);
            }

            if (timeToFire <= 0 && angleTo <= fireAngle)
            {
                Fire();
                print("Firing");
            }
        }
    }

    public Enemy EvaluateTarget()
    {
        Collider[] cols = Physics.OverlapSphere(head.position, viewRange, enemyMask);

        Enemy enemy = null;
        float angle = Mathf.Infinity;

        foreach(Collider col in cols)
        {
            Enemy e = col.GetComponentInParent<Enemy>();
            if (e == null) continue;
            Vector3 dir = col.transform.position - arms.position;
            float angleX = Vector3.SignedAngle(arms.forward, dir, arms.right);
            if (TargetInRange(e.CentrePoint()) && Vector3.Angle(arms.forward, dir) < angle){
                enemy = e;
                angle = Vector3.Angle(arms.forward, dir);
                print("Changing enemy to " + e);
            }

        }

        if (enemy != null)
        {
            target = enemy;
            enemy.died += OnLoseTarget;
        }
        return enemy;
    }

    public bool TargetInRange(Vector3 p)
    {
        float angleX = Vector3.SignedAngle(arms.forward, p - arms.position, arms.right);
        if (angleX > 90) angleX = 180 - angleX;

        return angleX >= xLimit.x && angleX <= xLimit.y && Vector3.Distance(p, arms.position) <= viewRange;
    }

    protected void Fire()
    {
        ammo--;
        timeToFire = 1 / fireRate;
        Vector3 s = Random.insideUnitSphere * spread / 100;
        GameObject go = Instantiate(tracer, barrellEnd == null? arms.position + arms.forward * 1.5f : barrellEnd.position, arms.rotation);
        go.transform.forward += s;
        Destroy(go, 2f);

        RaycastHit hit;
        if (Physics.Raycast(arms.position + arms.forward * 1.5f, arms.forward + s, out hit, viewRange * 1.5f, hitMask))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        if(ammo <= 0)
        {
            interactable.promptText = "Ammuntion depleted";
            enabled = false;
        }
        interactable.promptText = "Ammunition: " + 100*ammo/maxAmmo + "%";
    }

    private void OnLoseTarget(object sender, System.EventArgs e)
    {
        target = null;
    }
}

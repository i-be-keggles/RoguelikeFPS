using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class WeaponRecoilHandler : MonoBehaviour
{
    private PlayerWeaponHandler handler;
    public Transform camHolder;

    public float heat;

    public float camRotSpeed;

    public float recoilVariation;
    public float heatDissipationBuffer; //time after firing last shot before resetting camera

    private Vector3 targetRot;
    
    private void Update()
    {
        if (heat <= 0) targetRot = Vector3.zero;
        else heat -= Time.deltaTime;

        camHolder.localRotation = Quaternion.Lerp(camHolder.localRotation, Quaternion.Euler(targetRot), camRotSpeed * Time.deltaTime);
    }


    public void Recoil(Weapon weapon)
    {
        if (targetRot == Vector3.zero) targetRot = camHolder.localEulerAngles;

        heat = 1f/weapon.fireRate * weapon.recoilHeatMultiplier;
        targetRot += new Vector3(-weapon.recoilV * (1f + Random.Range(-recoilVariation, recoilVariation)), weapon.recoilH * (1f + Random.Range(-recoilVariation, recoilVariation)) * (Random.value > 0.5f? 1 : -1), 0);
    }

    public void ClearHeat()
    {
        heat = 0;
    }
}
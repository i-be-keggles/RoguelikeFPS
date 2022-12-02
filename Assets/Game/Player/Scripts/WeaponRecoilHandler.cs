using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerRecoilHandler : MonoBehaviour
{
    private PlayerWeaponhandler handler;
    public Transform camHolder;

    public float heat;

    public float camRotSpeed;

    public float recoilVariation;

    private Vector3 targetRot;
    
    private void Update()
    {
        if (heat <= 0) targetRot = Vector3.zero;
        else heat -= Time.deltaTime;
    }

    public void Recoil(Weapon weapon)
    {
        heat = (1.5f/weapon.fireRate);                                   //float                                                                float                            int (either 0 or 1) --> left or right
        targetRot += new Vector3(weapon.recoilV * (1f + Random.range(-recoilVariation, recoilVariation), weapon.recoilH * (1f + Random.range(-recoilVariation, recoilVariation) * (Random.Range(0,1) == 0? 1 : -1), 0);
        cam.localEulerAngles = Math.lerp(cam.localEulerAngles, targetRot, camRotSpeed);
    }

    public void ClearHeat()
    {
        heat = 0;
    }
}
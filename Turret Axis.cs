using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAxis : WeaponAxis
{
    protected override void Update()
    {
        if (currentTarget != null)
        {
            // ѕолучаем направление, выравниваем плоскости.
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnWeaponPlane = Vector3.ProjectOnPlane(direction, transform.right);

            // ѕолучаем кватернион выровненного направлени€ и расчитываем новый кватернион смещени€ в кадре.
            Quaternion endRotation = Quaternion.LookRotation(directionOnWeaponPlane);
            Quaternion rotationQ = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);

            // ”станавливаем новый квартернион.
            transform.localRotation = WeaponAxis.Clamp(constrainedAngle, Vector3.right, rotationQ);
        }
        else
        {
            // ¬озвращаем турель в изначальный поворот.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }

    // ¬ турельном типе не используетс€.
    public override void SwitchMode(WeaponTargettingMode mode) { }
}

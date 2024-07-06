using UnityEngine;

public class TurretAxis : WeaponAxis
{
    protected override void Update()
    {
        if (currentTarget != null)
        {
            // Получаем направление, выравниваем плоскости.
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnWeaponPlane = Vector3.ProjectOnPlane(direction, transform.right);

            // Получаем кватернион выровненного направления и расчитываем новый кватернион смещения в кадре.
            Quaternion endRotation = Quaternion.LookRotation(directionOnWeaponPlane);
            Quaternion rotationQ = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);

            // Устанавливаем новый квартернион.
            transform.localRotation = WeaponAxis.Clamp(constrainedAngle, Vector3.right, rotationQ);
        }
        else
        {
            // Возвращаем турель в изначальный поворот.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }

    // В турельном типе не используется.
    public override void SwitchMode(WeaponTargettingMode mode) { }
}

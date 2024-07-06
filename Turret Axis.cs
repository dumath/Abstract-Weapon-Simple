using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAxis : WeaponAxis
{
    protected override void Update()
    {
        if (currentTarget != null)
        {
            // �������� �����������, ����������� ���������.
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnWeaponPlane = Vector3.ProjectOnPlane(direction, transform.right);

            // �������� ���������� ������������ ����������� � ����������� ����� ���������� �������� � �����.
            Quaternion endRotation = Quaternion.LookRotation(directionOnWeaponPlane);
            Quaternion rotationQ = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);

            // ������������� ����� �����������.
            transform.localRotation = WeaponAxis.Clamp(constrainedAngle, Vector3.right, rotationQ);
        }
        else
        {
            // ���������� ������ � ����������� �������.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }

    // � ��������� ���� �� ������������.
    public override void SwitchMode(WeaponTargettingMode mode) { }
}

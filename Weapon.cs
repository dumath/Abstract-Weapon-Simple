using UnityEngine;

/* ���������� ����� ������. ������-��������.  */
public abstract class Weapon : MonoBehaviour
{
    #region Object Propertyies
    [SerializeField] protected float rotationSpeed = 15.0f; // �������� ������������� ��������.
    [SerializeField] protected float constrainedAngle = 25.0f; // ������������� � ������������� ���� �����������.

    protected Transform currentTarget; // ������� ����������� ���������. NULL - ������� � �������� ��������.
    #endregion

    #region Mono methods
    // Update is called once per frame
    protected virtual void Update()
    {
        if (currentTarget != null)
        {
            // �������� �����������, ����������� ���������.
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnWeaponPlane = Vector3.ProjectOnPlane(direction, transform.right);

            // �������� ���������� ������������ ����������� � ����������� ����� ���������� �������� � �����.
            Quaternion endRotation = Quaternion.LookRotation(directionOnWeaponPlane);
            Quaternion rotationQ = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);

            //TODO: CLAMP string 34. Bad solution - Clamp setted after transform =. Rewrite after rocket launcher class.
            Vector3 clampedEuler = rotationQ.eulerAngles;

            // ������������� ����� �����������.
            transform.rotation = rotationQ;

            /* ����� - �� ������� ������� � ������� ���. Bad solution. Up.*/
            //���� � ���������, ������� �� ������������ ��������.
            transform.localEulerAngles = new Vector3(Mathf.Clamp(transform.localEulerAngles.x, -constrainedAngle, constrainedAngle), 0f, 0f);
        }
        else
        {
            // ���� �� ������. NULL. ���� � ���������, ������� �� ������������ ��������.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Weapon methods
    // ���������� ��������� �� WeaponController. ������������� ���������� ����. 
    public void OnTargetChange(Transform target) => currentTarget = target;
    #endregion
}

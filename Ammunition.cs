using System;
using UnityEngine;

// ������� ������, ���� �����������. ���������������, ��������, ������������� � �������.
public abstract class Ammunition : MonoBehaviour
{
    #region Object propertyies
    // Callback ��� ���������. ������ ���������.
    protected Action<GameObject> onReturnAction;

    // ������������ ���������?
    protected bool isSpentAmmo = false;

    //TODO: ����� �� ������? ������ �������/����������..��� ��������?
    #endregion

    #region Mono Methods
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //����� ������ �� ���������� �������� ��������, ������ ����.
        if (!isSpentAmmo)
            if (onReturnAction != null)
            {
                Invoke(nameof(DelayReturn), 10f);
                isSpentAmmo = true;
            }
    }

    // ��� ���������� �����, �������� ����������� (������������ �������� ��������, ����� ������ ����� ���� ������������ ��������).
    protected void OnEnable() => isSpentAmmo = false;
    #endregion

    #region Ammunition Methods
    //������ �������� ���� ��� � ������ �� ��������.
    public void SetActionOnReturn(Action<GameObject> action)
    {
        if(onReturnAction == null)
            onReturnAction = action;
    }

    // ������� ������� � ��� ���������� �� �����. ������������ Invoke' ��.
    public void DelayReturn() => onReturnAction(this.gameObject);
    #endregion
}
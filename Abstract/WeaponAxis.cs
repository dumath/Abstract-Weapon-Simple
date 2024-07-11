using UnityEngine;

public abstract class WeaponAxis : MonoBehaviour
{
    #region Object Propertyies
    protected WeaponTargettingMode targetingMode = WeaponTargettingMode.Forward;

    [SerializeField] protected float rotationSpeed = 15.0f; // Скорость вертикального поворота.
    [SerializeField] protected float constrainedAngle = 25.0f; // Модуль вертикального ограничения.

    protected Transform currentTarget; // Текущее направление наведения. NULL - вернуть в искодное значение.
    #endregion

    #region Mono
    // Update is called once per frame
    protected abstract void Update();
    #endregion

    #region Weapon
    // Вызывается делегатом из WeaponController. Устанавливает рассчетную цель. 
    public void OnTargetChange(Transform target) => currentTarget = target;
    public abstract void SwitchMode(WeaponTargettingMode mode);

    /// <summary>
    /// Возвращает локальный кватернион, ограниченный углом выбранной оси.
    /// </summary>
    /// <param name="absoluteAngle"> Абсолютный угол </param>
    /// <param name="axis"> Ось вращения </param>
    /// <param name="rotation"> Кватернион локальный, либо глобальный. </param>
    /// <returns> Кватернион ограниченный углом поворота. </returns>
    public static Quaternion Clamp(float constrainedAngle, Vector3 axis, Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        Vector3 constraint = axis * Mathf.Abs(absoluteAngle); // По сути разворачивает зажим в обратную сторону. Как и axis.
        euler.x = Mathf.Clamp(euler.x, -constraint.x, constraint.x);
        euler.y = Mathf.Clamp(euler.y, -constraint.y, constraint.y);
        euler.z = Mathf.Clamp(euler.z, -constraint.z, constraint.z);
        return Quaternion.Euler(euler);
    }
    #endregion

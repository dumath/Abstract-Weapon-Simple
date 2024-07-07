using UnityEngine;

public class PlatformAxis : MonoBehaviour
{
    #region Object properties
    [SerializeField] protected float rotationSpeed = 30f; // Скорость поворота.

    protected Transform currentTarget; // Текущая цель.
    #endregion

    #region Mono
    protected void Start() { }

    protected void Update()
    {
        if (currentTarget != null)
        {
            // Получаем направление, выравниваем плоскости.
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnFastenerPlane = Vector3.ProjectOnPlane(direction, transform.up);

            // Получаем кватернион выровненного направления.
            Quaternion endRotation = Quaternion.LookRotation(directionOnFastenerPlane, transform.up);

            // Задаем кватернион поворота, со смещением в кадре.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Цель отсутствует. Возвращаем крепеж в изначальный поворот.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Platform axis
    // Вызывается контроллером. Устанавливает новую цель.
    public void OnTargetChange(Transform target) => currentTarget = target;
    #endregion
}

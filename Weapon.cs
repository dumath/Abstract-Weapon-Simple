using UnityEngine;

/* Абстракция башен оружия. Снаряд-ракетные.  */
public abstract class Weapon : MonoBehaviour
{
    #region Object Propertyies
    [SerializeField] protected float rotationSpeed = 15.0f; // Скорость вертикального поворота.
    [SerializeField] protected float constrainedAngle = 25.0f; // Модуль вертикального ограничения.

    protected Transform currentTarget; // Текущее направление наведения. NULL - вернуть в искодное значение.
    #endregion

    #region Mono methods
    // Update is called once per frame
    protected virtual void Update()
    {
        if (currentTarget != null)
        {
            // Получаем направление, выравниваем плоскости.
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnWeaponPlane = Vector3.ProjectOnPlane(direction, transform.right);

            // Получаем кватернион выровненного направления и расчитываем новый кватернион смещения в кадре.
            Quaternion endRotation = Quaternion.LookRotation(directionOnWeaponPlane);
            Quaternion rotationQ = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);

            //TODO: CLAMP string 34. Bad solution - Clamp setted after transform =. Rewrite after rocket launcher class.
            // X вращение, для ограничения не используем, только формула, пока что эта инструкция ничего не делает, кроме присвоения в локальную.
            // Left as is.
            Vector3 clampedEuler = rotationQ.eulerAngles;

            // Устанавливаем новый квартернион.
            transform.rotation = rotationQ;

            /* Далее - не хорошее решение и грязный код. Bad solution. Up.*/
            // Угол в локальных, зависит от пространства родителя.
            transform.localEulerAngles = Vector3.right * Mathf.Clamp(transform.localEulerAngles.x, -constrainedAngle, constrainedAngle);
        }
        else
        {
            // Цель не задана. NULL. Угол в локальных, зависит от пространства родителя.
            // Возвращаем турель в изначальный поворот.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Weapon methods
    // Вызывается делегатом из WeaponController. Устанавливает рассчетную цель. 
    public void OnTargetChange(Transform target) => currentTarget = target;
    #endregion
}

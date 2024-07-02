using System;
using UnityEngine;

// Открыть доступ, если потребуется. Переопределение, сокрытие, загораживание в методах.
[RequireComponent(typeof(Rigidbody))] // Чтобы не забыть.
public abstract class Ammunition : MonoBehaviour
{
    // TODO: Поле для PSystem/(AudioClip? static?);
    #region Object propertyies
    protected Action<Ammunition> onReturnAction; // Callback при попадании. Снаряд отработал.

    protected bool isSpentAmmo = false; // Отработанный боеприпас?

    protected Rigidbody body; // Твердое тело боеприпаса.

    [SerializeField] protected float timeToReturn = 10f; // Время, через которое снаряд вернется в пул.

    [SerializeField] protected float damage = 0; // урон от боеприпаса

    #endregion

    #region Mono
    // Ставим в Awake, чтобы был виден с инициализации префаба.
    protected virtual void Awake() => body = GetComponent<Rigidbody>();
    protected abstract void Start();
    protected abstract void Update();

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // Чтобы физика не продолжала вызывать делегата, ставим флаг.
        if (!isSpentAmmo)
            if (onReturnAction != null)
            {
                isSpentAmmo = true;
                Invoke(nameof(delayedReturn), timeToReturn);
            }
    }

    // При выключении пулом, свойство обновляется (возвращается исходное значение, чтобы снаряд можно было использовать повторно).
    protected virtual void OnEnable() => isSpentAmmo = false;
    #endregion

    #region Ammunition
    // Возврат снаряда в пул происходит не сразу. Используется Invoke' ом.
    protected virtual void delayedReturn() => onReturnAction(this);


    // Крепим метод возврата в пул.
    public virtual void SetActionOnReturn(Action<Ammunition> action)
    {
        // Ссылка ставится один раз и больше не меняется.
        if (onReturnAction == null)
            onReturnAction = action;
    }

    /// <summary>
    /// Применяется к RigidBody. Задает направление и силу снаряда. Объект не кинематический.
    /// </summary>
    /// <param name="position"> Начальная позиция </param>
    /// <param name="rotation"> Начальный поворот </param>
    /// <param name="force"> Силы выстрела </param>
    /// <param name="mode"> По умолчанию - Impulse. </param>
    public virtual void AddForce(Vector3 position, Quaternion rotation , Vector3 force, ForceMode mode = ForceMode.Impulse)
    {
        // Задана точка трансформа, body.position - не нужно. На дуле коллайдера нет.
        gameObject.transform.position = position; 
        gameObject.transform.rotation = rotation;
        body.AddForce(force, mode);
    }

    /// <summary>
    /// Обнуляет примененную силу.
    /// </summary>
    public virtual void ExhaustForce()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }
    #endregion
}

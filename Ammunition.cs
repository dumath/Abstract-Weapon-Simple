using System;
using UnityEngine;

// Открыть доступ, если потребуется. Переопределение, сокрытие, загораживание в методах.
public abstract class Ammunition : MonoBehaviour
{
    #region Object propertyies
    // Callback при попадании. Снаряд отработал.
    protected Action<GameObject> onReturnAction;

    // Отработанный боеприпас?
    protected bool isSpentAmmo = false;

    //TODO: Нужен ли префаб? Снаряд обычный/осколочный..Для Андройда?
    #endregion

    #region Mono Methods
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Чтобы физика не продолжала вызывать делегата, ставим флаг.
        if (!isSpentAmmo)
            if (onReturnAction != null)
            {
                Invoke(nameof(DelayReturn), 10f);
                isSpentAmmo = true;
            }
    }

    // При выключении пулом, свойство обновляется (возвращается исходное значение, чтобы снаряд можно было использовать повторно).
    protected void OnEnable() => isSpentAmmo = false;
    #endregion

    #region Ammunition Methods
    //Ссылка ставится один раз и больше не меняется.
    public void SetActionOnReturn(Action<GameObject> action)
    {
        if(onReturnAction == null)
            onReturnAction = action;
    }

    // Возврат снаряда в пул происходит не сразу. Используется Invoke' ом.
    public void DelayReturn() => onReturnAction(this.gameObject);
    #endregion
}
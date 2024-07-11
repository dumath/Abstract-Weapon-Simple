using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum WeaponClass { Turret, Launcher }
public enum WeaponTargettingMode { Forward, Upward, Mixed }
public enum WeaponAmmunitionType { T1, T2, Mixed }


public class AmmunitionSlot
{
    public readonly Vector3 slotPosition;

    private Ammunition ammunition;

    public AmmunitionSlot(Vector3 slotPosition, Ammunition ammunition)
    {
        this.slotPosition = slotPosition;
        this.ammunition = ammunition;
    }

    public void InsertAmmunition(Ammunition ammunition)
    {
        if (ammunition == null)
            this.ammunition = ammunition;
        else
            throw new NotImplementedException("Slot is full.");
    }

    /// <summary>
    /// Освобождает слот и возвращает боеприпас.
    /// </summary>
    /// <returns></returns>
    public Ammunition GetAmmunition()
    {
        Ammunition ammunition = this.ammunition;

        this.ammunition = null;

        return ammunition;
    }
}

public class AmmunitionPool : IDisposable
{
    // TODO: Сделать адекватную реализацию метода высвобождения. bool isDisposed.Clear();
    // TODO: Нужна обработка исключений.

    // Пул боеприпасов.
    protected Dictionary<WeaponAmmunitionType, ObjectPool<Ammunition>> pool;

    #region Конструкторы.
    public AmmunitionPool(WeaponAmmunitionType ammunitionType, ObjectPool<Ammunition> pool)
    {
        this.pool = new Dictionary<WeaponAmmunitionType, ObjectPool<Ammunition>>();
        this.pool[ammunitionType] = pool;
    }

    public AmmunitionPool(params ObjectPool<Ammunition>[] pools)
    {
        pool = new Dictionary<WeaponAmmunitionType, ObjectPool<Ammunition>>();

        WeaponAmmunitionType ammunitionType = WeaponAmmunitionType.T1;

        foreach (ObjectPool<Ammunition> pool in pools)
        {
            this.pool[ammunitionType] = pool;
            ammunitionType++;
        }
    }
    #endregion


    public virtual void Dispose() => pool.Clear();

    public virtual Ammunition GetByType(WeaponAmmunitionType ammunitionType) => pool[ammunitionType].Get();

    public virtual Action<Ammunition> ReleaseByType(WeaponAmmunitionType ammunitionType) => pool[ammunitionType].Release;
}

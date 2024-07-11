using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AmmunitionDrum : MonoBehaviour
{
    protected AmmunitionPool pool;

    protected WeaponAmmunitionType currentAmmunitionType;

    public int MaxSizePool = 5; // Размер пула одного типа боеприпаса.

    [SerializeField] protected GameObject ammoPrefabFirst; // Шаблон боеприпаса 1-го типа.
    [SerializeField] protected GameObject ammoPrefabSecond; // Шаблон боеприпаса 2-го типа.



    protected abstract void Awake();

    protected abstract void Start();

    protected abstract void Update();

    public virtual Ammunition Get() => pool.GetByType(currentAmmunitionType);

    
    // Меняет боеприпас. Не масштабируемая версия. При расширении изменить реализацию.
    public virtual void SwichAmmunitionType()
    {
        switch(currentAmmunitionType)
        {
            case WeaponAmmunitionType.T1: currentAmmunitionType++; break;
            case WeaponAmmunitionType.T2: currentAmmunitionType--; break;
        }
    }

    #region Object Pool
    // Метод, вызываемый ObjectPool<>.Get(), если пул пустой. Для первого префаба.
    public Ammunition OnCreateAmmoT1()
    {
        // Создаем боеприпас из шаблона и назначаем ему callback от типа боеприпаса. Один раз!
        GameObject instance = Instantiate(ammoPrefabFirst);
        instance.GetComponent<Ammunition>().SetActionOnReturn(pool.ReleaseByType(currentAmmunitionType));

        return instance.GetComponent<Ammunition>();
    }

    // Метод, вызываемый ObjectPool<>.Get(), если пул пустой. Для второго префаба.
    public Ammunition OnCreateAmmoT2()
    {
        // Создаем боеприпас из шаблона и назначаем ему callback от типа боеприпаса. Один раз!
        GameObject instance = Instantiate(ammoPrefabSecond);
        instance.GetComponent<Ammunition>().SetActionOnReturn(pool.ReleaseByType(currentAmmunitionType));

        return instance.GetComponent<Ammunition>();
    }

    // Метод, вызываемый ObjectPool<>.Get().
    public virtual void OnGetAmmoFromPool(Ammunition item) => item.gameObject.SetActive(true);

    // Метод, вызываемый ObjectPool<>.Release().
    public virtual void OnReleaseAmmoToPool(Ammunition item)
    {
        item.ExhaustForce(); // ???
        item.gameObject.SetActive(false);
    }

    // Метод, вызываемый ObjectPool<>.Destroy().
    public virtual void OnDestroyAmmoFromPool(Ammunition item) => Destroy(item.gameObject);
    #endregion
}

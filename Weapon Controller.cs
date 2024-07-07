using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using System;


/* Контроллер оружия. Состоит из 4 частей : Платформа, крепеж, оружие и прицел.
 * Пул реализован на каждой платформе. 
 * Чтобы расширить тип боеприпаса, поскольку барабан не масштабировался под различные форм - условия, нужно: 
    - Добавить поле шаблона и инициализировать его.
    - Реализовать WeaponAmmunitionType литерал значения.
    - Обновить Drum класс. Реализован в самом низу.
    - Добавить еще один OnCreateAmmoT-n (внутри метода задать ссылку на возврат в нужный пул ).
    = В Awake, добавить значение в параметр, в вызове конструктора. */


public abstract class WeaponController : MonoBehaviour
{
    #region Constants
    public const float ACCURACY = 0.9998f;
    #endregion

    #region Object propertyies
    [SerializeField] protected WeaponAxis weaponAxis; // Ось оружия.
    [SerializeField] protected PlatformAxis platformAxis; // Ось платформы.
    [SerializeField] protected Transform weaponSight; // Прицел.

    public WeaponAmmunitionType currentAmmoType; // Текущий тип боеприпаса.
    [SerializeField] protected GameObject ammoPrefabFirst; // Шаблон боеприпаса 1-го типа.
    [SerializeField] protected GameObject ammoPrefabSecond; // Шаблон боеприпаса 2-го типа.

    protected Action<Transform> setTargetAction; // Установщик целей, для наводки 2-х поворотных частей.

    public float Strength = 5f; // Сила выстрела.
    public float RepeatRate = 2.0f; // Темп стрельбы.

    protected float elapsedTime = 0f; // Пройденное время. При необходимости убрать задержку первого выстрела, со старта сцены.

    protected List<Transform> targets; // Список очереди целеуказания.

    protected Drum<Ammunition> drum; // Барабан боеприпасов.

    public int MaxSizePool = 5; // Размер одной части пула.
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        //Инициализируем барабан.
        drum = new Drum<Ammunition>(
            new ObjectPool<Ammunition>(OnCreateAmmoT1, OnGetAmmoFromPool, OnReleaseAmmoToPool, OnDestroyAmmoFromPool, maxSize: MaxSizePool),
            new ObjectPool<Ammunition>(OnCreateAmmoT2, OnGetAmmoFromPool, OnReleaseAmmoToPool, OnDestroyAmmoFromPool, maxSize: MaxSizePool)
            );

        //Инициализируем коллекцию целеуказания.
        targets = new List<Transform>();

        //Крепим к делегату методы уведомлений двух поворотных частей.
        setTargetAction += platformAxis.OnTargetChange;
        setTargetAction += weaponAxis.OnTargetChange;
    }

    // Start is called before the first frame update
    protected abstract void Start();

    // Update is called once per frame
    protected abstract void Update();

    // Точность расчетов пока-что не нужна.
    protected abstract void FixedUpdate();

    protected virtual void OnTriggerEnter(Collider other)
    {
        //  Добавляем трансформ в список, для отслеживания.
        targets.Add(other.gameObject.transform);

        // По умолчанию, первая попавшая цель - будет сопровождена до выхода и коллайдера.
        setTargetAction(targets.First());
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        // Удаляем первый вход, поскольку вышел из области видимости турели.
        targets.Remove(other.gameObject.transform);

        // Обновляем целеуказание, на следующий по очереди Transform.
        // Null - целеуказание отсутствует. Вернуть турель в исходный поворот.
        setTargetAction(targets.FirstOrDefault());
    }
    #endregion

    #region Weapon controller
    public abstract void Fire();
    #endregion

    #region Object Pool
    // Метод, вызываемый ObjectPool<>.Get(), если пул пустой. Для первого префаба.
    public Ammunition OnCreateAmmoT1()
    {
        // Создаем боеприпас из шаблона и назначаем ему callback от типа боеприпаса. Один раз!
        GameObject instance = Instantiate(ammoPrefabFirst);
        instance.GetComponent<Ammunition>().SetActionOnReturn(drum.ReleaseByType(currentAmmoType));

        return instance.GetComponent<Ammunition>();
    }

    // Метод, вызываемый ObjectPool<>.Get(), если пул пустой. Для второго префаба.
    public Ammunition OnCreateAmmoT2()
    {
        // Создаем боеприпас из шаблона и назначаем ему callback от типа боеприпаса. Один раз!
        GameObject instance = Instantiate(ammoPrefabSecond);
        instance.GetComponent<Ammunition>().SetActionOnReturn(drum.ReleaseByType(currentAmmoType));
        
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

public enum WeaponTargettingMode { Forward, Upward, Forwardx2, Forwardx3, AllWeapons }
public enum WeaponAmmunitionType { T1, T2 }



public class Drum<T> : IDisposable where T : class
{
    /* 1 Вариант - реализовать I<ObjectPool>
     * 2 вариант - оболочка 2х - 3х - n'x ObjectPool, без масштабирования.
     * Оставлен 2.  */
    private ObjectPool<T> poolT1;
    private ObjectPool<T> poolT2;

    /// <summary>
    /// Активный конструктор барабана.
    /// </summary>
    /// <param name="t1"> Первый тип боеприпасов </param>
    /// <param name="t2"> Второй тип боеприпасов </param>
    public Drum(ObjectPool<T> t1, ObjectPool<T> t2)
    {
        poolT1 = t1;
        poolT2 = t2;
    }

    // TODO: Сделать нормальную реализацию метода высвобождения.
    public void Dispose()
    {
        poolT1 = null;
        poolT2 = null;
    }

    public T GetByType(WeaponAmmunitionType type)
    {
        if (type == WeaponAmmunitionType.T1)
            return poolT1.Get();

        if (type == WeaponAmmunitionType.T2)
            return poolT2.Get();

        throw new NotImplementedException();
    }

    public Action<T> ReleaseByType(WeaponAmmunitionType type)
    {
        if (type == WeaponAmmunitionType.T1)
            return poolT1.Release;

        if (type == WeaponAmmunitionType.T2)
            return poolT2.Release;

        throw new NotImplementedException();
    }
}

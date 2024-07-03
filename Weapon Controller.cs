using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using System;

public abstract class WeaponController : MonoBehaviour
{
    #region Constants
    public const float ACCURACY = 0.9998f;
    #endregion

    #region Object propertyies
    [SerializeField] protected Weapon weapon; // Турель.
    [SerializeField] protected Fastener fastener; // Крепеж турели.
    [SerializeField] protected Transform weaponSight; // Прицел оружия.

    [SerializeField] private GameObject ammoPrefab; // Шаблон боеприпаса.

    protected Action<Transform> setTargetAction; // Установщик целей, для наводки.

    public float Strength = 5f; // Сила выстрела.
    public float RepeatRate = 2.0f; //Темп стрельбы.

    protected float elapsedTime = 0f; // Пройденное время. При необходимости убрать задержку первого выстрела, со старта сцены.

    protected List<Transform> targets; // Список очереди целеуказания.

    protected ObjectPool<Ammunition> pool; // Пул боеприпасов. 
    public int MaxSizePool = 10;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        //Инициализируем Pool.
        pool = new ObjectPool<Ammunition>(OnCreateAmmo, OnGetAmmoFromPool, OnReleaseAmmoToPool, OnDestroyAmmoFromPool, maxSize: MaxSizePool);

        //Инициализируем коллекцию целеуказания.
        targets = new List<Transform>();

        //Крепим к делегату методы уведомлений двух поворотных частей.
        setTargetAction += fastener.OnTargetChange;
        setTargetAction += weapon.OnTargetChange;
    }

    // Start is called before the first frame update
    protected abstract void Start();

    // Update is called once per frame
    protected abstract void Update();

    protected virtual void OnTriggerEnter(Collider other)
    {
        //  Добавляем трансформ в список, для отслеживания.
        targets.Add(other.gameObject.transform);

        // По умолчанию, первая попавшая цель - будет сопровождена до выхода и коллайдера.
        setTargetAction(targets.First());
    }

    // Пока - что 30fps с ограничителем, на Android'e. Raycast не переносим.
    // Точность расчетов пока-что не нужна.
    protected abstract void FixedUpdate(); 

    protected virtual void OnTriggerExit(Collider other)
    {
        // Удаляем первый вход, поскольку вышел из области видимости турели.
        targets.Remove(other.gameObject.transform);

        // Обновляем целеуказание, на следующий по очереди Transform.
        // Null - целеуказание отсутствует. Вернуть турель в исходный поворот.
        setTargetAction(targets.FirstOrDefault());
    }
    #endregion

    #region Weapon Controller methods
    // Базовая реализация используется для турельного типа.
    public abstract void Fire();
    #endregion

    #region Object Pool
    // Метод, вызываемый ObjectPool<>.Get(), если пул пустой.
    public Ammunition OnCreateAmmo()
    {
        //Создаем объект из префаба и назначаем ему callback. (Один раз!).
        //Мидификатор readonly - не использую. Обычная проверка на NULL.
        GameObject instance = Instantiate(ammoPrefab);
        instance.GetComponent<Ammunition>().SetActionOnReturn(pool.Release);

        return instance.GetComponent<Ammunition>();
    }

    // Метод, вызываемый ObjectPool<>.Get().
    public void OnGetAmmoFromPool(Ammunition item) => item.gameObject.SetActive(true);

    // Метод, вызываемый ObjectPool<>.Release().
    public void OnReleaseAmmoToPool(Ammunition item)
    {
        item.ExhaustForce();
        item.gameObject.SetActive(false);
    }

    // Метод, вызываемый ObjectPool<>.Destroy().
    public void OnDestroyAmmoFromPool(Ammunition item) => Destroy(item.gameObject);
    #endregion
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


/* Контроллер оружия. Состоит из 4 частей : Платформа, крепеж, оружие и прицел.
 * Схожие свойства выделены в абстракцию.
 * Пул реализован на каждой платформе. Ammunition Drum. Так же - абстрактный класс.
 * Чтобы расширить тип боеприпаса, поскольку барабан не масштабировался под различные форм - условия нужно обновить Ammunition Drum.
 * Пока неизвестно, где будет находится перечисление, для удобства. Временно вынесен в класс Additional.
 */


public abstract class WeaponController : MonoBehaviour
{
    #region Constants
    public const float ACCURACY = 0.9998f;
    #endregion

    #region Object propertyies
    [SerializeField] protected WeaponAxis weaponAxis; // Ось оружия.
    [SerializeField] protected PlatformAxis platformAxis; // Ось платформы.
    [SerializeField] protected Transform weaponSight; // Прицел.

    protected AmmunitionDrum ammunitionDrum; // Обновленный барабан, под два типа наведения.

    protected Action<Transform> setTargetAction; // Установщик целей, для наводки 2-х поворотных частей.

    public float Strength = 5f; // Сила выстрела.
    public float RepeatRate = 2.0f; // Темп стрельбы.

    protected float elapsedTime = 0f; // Пройденное время. При необходимости убрать задержку первого выстрела, со старта сцены.

    protected List<Transform> targets; // Список очереди целеуказания.

    
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        ammunitionDrum = GetComponent<AmmunitionDrum>();
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
}

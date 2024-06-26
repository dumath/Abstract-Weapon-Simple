using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using System;

public sealed class WeaponController : MonoBehaviour
{
    #region Constants
    public const float ACCURACY = 0.9998f;
    #endregion

    #region Object Properties
    [SerializeField] private Weapon weapon; // Турель.
    [SerializeField] private Fastener fastener; // Крепеж турели.

    [SerializeField] private GameObject barrel; // Дуло.

    [SerializeField] private GameObject ammoPrefab; // Шаблон боеприпаса.

    private Action<Transform> setTargetAction; // Установщик целей, для наводки.

    public float Strength = 5f; // Сила выстрела.
    public float RepeatRate = 2.0f; //Темп стрельбы.

    private List<Transform> targets; // Список очереди целеуказания.
    private ObjectPool<GameObject> pool; // Пул боеприпасов. 

    private float elapsedTime = 0f; //Пройденное время. При необходимости убрать задержку первого выстрела, со старта сцены.
    #endregion

    #region Mono Methods
    // Start is called before the first frame update
    void Start()
    {
        //Инициализируем Pool.
        pool = new ObjectPool<GameObject> (OnCreateAmmo, OnGetAmmoFromPool, OnReleaseAmmoToPool, OnDestroyAmmoFromPool, maxSize: 10);

        //Инициализируем коллекцию.
        targets = new List<Transform>();

        //Крепим к делегату методы уведомлений двух поворотных частей.
        setTargetAction = new Action<Transform>(fastener.OnTargetChange);
        setTargetAction += weapon.OnTargetChange;
    }

    // Update is called once per frame
    void Update()
    {
        if(targets.Count != 0)
        {
            // Находим вектор направления до цели.
            Vector3 direction = targets.First().transform.position - barrel.transform.position;

            // Смотрим сонаправленность векторов цели и дула.
            float dotCondition = Vector3.Dot(direction.normalized, barrel.transform.forward);

            if(dotCondition > ACCURACY)
            {
                // Реализация простого счетчика пройденного времени.
                // Можно убрать задержку 2f, со старта сцены, присвоив полю, в месте объявления 2f.
                // Left as is.
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= RepeatRate)
                {
                    // Стреляем, обновляем таймер пройденного времени.
                    Fire();
                    elapsedTime = 0f;
                }
            }
        }
        
        // Debug. Оставлен, видеть наводку(угол вертикальный с ограничителем + горизонтальный),
        // Выстрел снаряда на выходе с пула со старта и после.
        Debug.DrawRay(barrel.transform.position, barrel.transform.forward * 20f, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        //  Добавляем трансформ в список, для отслеживания.
        targets.Add(other.gameObject.transform);

        // По умолчанию, первая попавшая цель - будет сопровождена до выхода и коллайдера.
        setTargetAction(targets.First());
    }

    private void OnTriggerExit(Collider other)
    {
        // Удаляем первый вход, поскольку вышел из области видимости турели.
        targets.Remove(other.gameObject.transform);

        // Обновляем целеуказание, на следующий по очереди Transform.
        // Null - целеуказание отсутствует. Вернуть турель в исходный поворот.
        setTargetAction(targets.FirstOrDefault());
    }
    #endregion

    #region Weapon Controller methods
    public void Fire()
    {
        // Берем объект из пула.
        GameObject ammo = pool.Get();
        
        // Задаем начальную точку. В данном случае якорь дула (Якорь выставлен в Blender'e).
        ammo.transform.position = barrel.transform.position;
        ammo.transform.rotation = barrel.transform.rotation;
        ammo.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * Strength, ForceMode.Impulse);
    }
    #endregion

    #region Object Pool
    /* Описание методов Intelli оставлено. На случай убрать под свой пул, либо поменять логику пула Unity. */

    /// <summary>
    /// Метод, вызываемый ObjectPool<>.Get(), если пул пустой.
    /// </summary>
    /// <returns> Созданный объект пула из префаба. </returns>
    public GameObject OnCreateAmmo()
    {
        //Создаем объект из префаба и назначаем ему callback. (Один раз!).
        //Мидификатор readonly - не использую. Обычная проверка на NULL.
        GameObject instance = Instantiate(ammoPrefab);
        instance.GetComponent<Ammunition>().SetActionOnReturn(pool.Release);

        return instance;
    }

    /// <summary>
    /// Метод, вызываемый ObjectPool<>.Get.
    /// </summary>
    /// <param name="item"> Объект, из пула. </param>
    public void OnGetAmmoFromPool(GameObject item)
    {
        // Включаем влияение физики, а затем сам объект.
        //item.GetComponent<Rigidbody>().detectCollisions = true;
        //item.GetComponent<Rigidbody>().useGravity = true;
        item.SetActive(true);
    }

    /// <summary>
    /// Метод, вызываемый ObjectPool<>.Release().
    /// </summary>
    /// <param name="item"> Объект, возвращаемый в пул. </param>
    public void OnReleaseAmmoToPool(GameObject item)
    {
        //// Отключаем влияние физики, расчет применяемых сил, а затем отключаем объект.
        //item.GetComponent<Rigidbody>().detectCollisions = false;
        //item.GetComponent<Rigidbody>().useGravity = false;

        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        item.SetActive(false);
    }

    /// <summary>
    /// Метод, вызываемый ObjectPool<>.Destroy().
    /// </summary>
    /// <param name="item"> Объект, удаляемый из пула. </param>
    public void OnDestroyAmmoFromPool(GameObject item) => Destroy(item.gameObject);
    #endregion
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public sealed class WeaponController : MonoBehaviour
{
    #region Nested classes
    private delegate void SetTargetDelegate(Transform target);
    private delegate void RemoveTargetDelegate();
    #endregion

    #region Object Propertyies
    [SerializeField] private Weapon weapon; // Турель.
    [SerializeField] private Fastener fastener; // Крепеж турели.

    [SerializeField] private GameObject barrel; // Дуло.

    [SerializeField] private GameObject ammoPrefab; // Шаблон боеприпаса.

    private SetTargetDelegate settingTarget;
    private RemoveTargetDelegate removingTarget;

    public float strength = 5f; // Сила выстрела.
    public float delay = 5f; // Задержка перед первым выстрелом.
    public float repeatRate = 2.0f; //Темп стрельбы.

    public List<Transform> targets; // Список очереди целеуказания.
    private ObjectPool<GameObject> pool; // Пул боеприпасов. 
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
        settingTarget = new SetTargetDelegate(fastener.OnTargetChange);
        settingTarget += weapon.OnTargetChange;

        //Заменить.
        InvokeRepeating(nameof(Fire), 5f, repeatRate);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.
        Debug.DrawRay(barrel.transform.position, barrel.transform.forward * 20f, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        //  Добавляем трансформ в список, для отслеживания.
        targets.Add(other.gameObject.transform);
        // По умолчанию, первая попавшая цель - будет сопровождена до выхода и коллайдера.
        settingTarget(targets.First());
    }

    private void OnTriggerExit(Collider other)
    {
        // Удаляем первый вход, поскольку вышел из области видимости турели.
        targets.Remove(other.gameObject.transform);
        // Обновляем целеуказание, на следующий по очереди Transform.
        // Null - целеуказание отсутствует. Вернуть турель в исходный поворот.
        settingTarget(targets.FirstOrDefault());
    }
    #endregion

    #region Weapon Controller methods
    public void Fire()
    {
        //TODO: Need Dot. Condition. Left as is.
        GameObject ammo = pool.Get();
        
        // Задаем начальную точку. В данном случае якорь дула ( Якорь выставлен в Blender'e).
        ammo.transform.position = barrel.transform.position;
        ammo.transform.rotation = barrel.transform.rotation;
        ammo.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * strength, ForceMode.Impulse);
    }
    #endregion

    #region Object Pool
    /// <summary>
    /// Метод, вызываемый ObjectPool<>.Get(), если пул пустой.
    /// </summary>
    /// <returns> Созданный объект пула из префаба. </returns>
    public GameObject OnCreateAmmo()
    {
        //Создаем объект из префаба и назначаем ему callback. (Один раз!).
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
    /// <param name="item"> Объект, удаляемый из пула.</param>
    public void OnDestroyAmmoFromPool(GameObject item) => Destroy(item.gameObject);
    #endregion
}



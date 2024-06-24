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
    [SerializeField] private Weapon weapon; // ������.
    [SerializeField] private Fastener fastener; // ������ ������.

    [SerializeField] private GameObject barrel; // ����.

    [SerializeField] private GameObject ammoPrefab; // ������ ����������.

    private SetTargetDelegate settingTarget;
    private RemoveTargetDelegate removingTarget;

    public float strength = 5f; // ���� ��������.
    public float delay = 5f; // �������� ����� ������ ���������.
    public float repeatRate = 2.0f; //���� ��������.

    public List<Transform> targets; // ������ ������� ������������.
    private ObjectPool<GameObject> pool; // ��� �����������. 
    #endregion

    #region Mono Methods
    // Start is called before the first frame update
    void Start()
    {
        //�������������� Pool.
        pool = new ObjectPool<GameObject> (OnCreateAmmo, OnGetAmmoFromPool, OnReleaseAmmoToPool, OnDestroyAmmoFromPool, maxSize: 10);

        //�������������� ���������.
        targets = new List<Transform>();

        //������ � �������� ������ ����������� ���� ���������� ������.
        settingTarget = new SetTargetDelegate(fastener.OnTargetChange);
        settingTarget += weapon.OnTargetChange;

        //��������.
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
        //  ��������� ��������� � ������, ��� ������������.
        targets.Add(other.gameObject.transform);
        // �� ���������, ������ �������� ���� - ����� ������������ �� ������ � ����������.
        settingTarget(targets.First());
    }

    private void OnTriggerExit(Collider other)
    {
        // ������� ������ ����, ��������� ����� �� ������� ��������� ������.
        targets.Remove(other.gameObject.transform);
        // ��������� ������������, �� ��������� �� ������� Transform.
        // Null - ������������ �����������. ������� ������ � �������� �������.
        settingTarget(targets.FirstOrDefault());
    }
    #endregion

    #region Weapon Controller methods
    public void Fire()
    {
        //TODO: Need Dot. Condition. Left as is.
        GameObject ammo = pool.Get();
        
        // ������ ��������� �����. � ������ ������ ����� ���� ( ����� ��������� � Blender'e).
        ammo.transform.position = barrel.transform.position;
        ammo.transform.rotation = barrel.transform.rotation;
        ammo.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * strength, ForceMode.Impulse);
    }
    #endregion

    #region Object Pool
    /// <summary>
    /// �����, ���������� ObjectPool<>.Get(), ���� ��� ������.
    /// </summary>
    /// <returns> ��������� ������ ���� �� �������. </returns>
    public GameObject OnCreateAmmo()
    {
        //������� ������ �� ������� � ��������� ��� callback. (���� ���!).
        GameObject instance = Instantiate(ammoPrefab);
        instance.GetComponent<Ammunition>().SetActionOnReturn(pool.Release);

        return instance;
    }

    /// <summary>
    /// �����, ���������� ObjectPool<>.Get.
    /// </summary>
    /// <param name="item"> ������, �� ����. </param>
    public void OnGetAmmoFromPool(GameObject item)
    {
        // �������� �������� ������, � ����� ��� ������.
        //item.GetComponent<Rigidbody>().detectCollisions = true;
        //item.GetComponent<Rigidbody>().useGravity = true;
        item.SetActive(true);
    }

    /// <summary>
    /// �����, ���������� ObjectPool<>.Release().
    /// </summary>
    /// <param name="item"> ������, ������������ � ���. </param>
    public void OnReleaseAmmoToPool(GameObject item)
    {
        //// ��������� ������� ������, ������ ����������� ���, � ����� ��������� ������.
        //item.GetComponent<Rigidbody>().detectCollisions = false;
        //item.GetComponent<Rigidbody>().useGravity = false;

        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        item.SetActive(false);
    }

    /// <summary>
    /// �����, ���������� ObjectPool<>.Destroy().
    /// </summary>
    /// <param name="item"> ������, ��������� �� ����.</param>
    public void OnDestroyAmmoFromPool(GameObject item) => Destroy(item.gameObject);
    #endregion
}



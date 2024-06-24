using UnityEngine;

public class Fastener : MonoBehaviour
{
    [SerializeField] protected float rotationSpeed = 30f;
    protected Transform currentTarget;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if(currentTarget != null)
        {
            Vector3 direction = currentTarget.position - transform.position;
            Vector3 directionOnFastenerPlane = Vector3.ProjectOnPlane(direction, transform.up);

            Quaternion endRotation = Quaternion.LookRotation(directionOnFastenerPlane, transform.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnTargetChange(Transform target) => currentTarget = target;
}

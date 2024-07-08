using UnityEngine;

public class LauncherAxis : WeaponAxis
{
    [Tooltip("360 / rotationSpeed.")] public float SwitchingTime; // Время переключения. 

    private float elapsedTime; // Пройденное время.
    private const float UPWARD_TARGETTING_ANGLE = 90f;

    // Update is called once per frame
    protected override void Update()
    {
        if (currentTarget != null)
        {
            if (targetingMode == WeaponTargettingMode.Upward)
            {
                Quaternion upRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(Vector3.left * UPWARD_TARGETTING_ANGLE), rotationSpeed * Time.deltaTime); ;
                transform.localRotation = upRotation;
            }
            else
            {
                // Получаем направление, выравниваем плоскости.
                Vector3 direction = currentTarget.position - transform.position;
                Vector3 directionOnWeaponPlane = Vector3.ProjectOnPlane(direction, transform.right);

                // Получаем кватернион выровненного направления и расчитываем новый кватернион смещения в кадре.
                Quaternion endRotation = Quaternion.LookRotation(directionOnWeaponPlane);
                Quaternion rotationQ = Quaternion.RotateTowards(transform.rotation, endRotation, rotationSpeed * Time.deltaTime);

                // Устанавливаем новый квартернион.
                if (elapsedTime <= 0f)
                    transform.localRotation = WeaponAxis.Clamp(constrainedAngle, Vector3.right, rotationQ);
                else
                    transform.rotation = rotationQ;
            }
        }
        else
        {
            // Цель отсутствует. Возвращаем турель в исходный поворот.
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
        }

    }

    // Вызывается контроллером. Меняет режим стрельбы.
    public override void SwitchMode(WeaponTargettingMode mode)
    {
        this.targetingMode = mode;
        elapsedTime = SwitchingTime;
    }
}

using System.Linq;
using UnityEngine;

public class TurretController : WeaponController
{
    #region Properties
    #endregion

    #region Mono
    // Start is called before the first frame update
    protected override void Start() { }
    // Update is called once per frame
    protected override void Update()
    {
        /* Контроллер наводид оружие и отслеживает цели */
        if (targets.Count != 0)
        {
            // Находим вектор направления до цели.
            Vector3 direction = targets.First().position - weaponSight.position;

            // Смотрим сонаправленность векторов цели и дула.
            float dotCondition = Vector3.Dot(direction.normalized, weaponSight.forward);
            if (dotCondition > ACCURACY)
            {
                // Реализация простого счетчика пройденного времени.
                // Left as is.
                // Внутри Turret controllera?? (Дуло приведено в прицел - общее свойство двух типов орудий)
                // Elapsed Time остается внутри наведения. Т.е. не считается, пока не наведется. Значения будут разные при входе в условие.
                // Left as is.
                elapsedTime += Time.deltaTime;
                RaycastHit hit;

                if (Physics.Raycast(weaponSight.position, direction, out hit, maxDistance: GetComponent<SphereCollider>().radius))
                {
                    // Player - ignore layer or <<layeMask;
                    // maxDistance - выходит за пределы коллайдера. Left as is.
                    /* Think: Отскок снаряда, будет ли реализация после отскока, при попадании в другую цель - еще один отскок;
                     На данном этапе, на ограждениях висит MESH коллайдер без convex , маска слоя для настроек проекта DEFAULT, пересечение выставлено со снарядом.
                     А нужно ли это на B-In, Android? Прострел между балками Convex/Primitive ; CapsuleCast; LayerMask - Ignored Raycast
                     Вариант - оставить прострел, но и оставить mesh - без convex, чтобы убрать Raycast, если понадобится.*/
                    if (hit.transform.gameObject.tag == targets.First().gameObject.tag)
                    {
                        if (elapsedTime >= RepeatRate)
                        {
                            // Стреляем, обновляем таймер пройденного времени.
                            Fire();
                            elapsedTime = 0f;
                        }
                    }
                }           
            }
        }
        // Debug. Оставлен, видеть наводку(угол вертикальный с ограничителем + горизонтальный),
        // Выстрел снаряда на выходе с пула со старта и после.
        Debug.DrawRay(weaponSight.position, weaponSight.forward * 20f, Color.red);
    }

    protected override void FixedUpdate() { }
    #endregion

    #region TurretController
    public override void Fire()
    {
        // Берем объект из пула.
        Ammunition ammo = drum.GetByType(currentAmmoType);

        // Задаем начальную точку. В данном случае якорь дула (Якорь выставлен в Blender'e).
        ammo.AddForce(weaponSight.position, weaponSight.rotation, weaponSight.forward * Strength);
    }
    #endregion
}

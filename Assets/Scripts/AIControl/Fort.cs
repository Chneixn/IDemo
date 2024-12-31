using System.Collections;
using UnityEngine;

public enum FortControl
{
    Auto,
    Handle
}

public class Fort : MonoBehaviour
{
    [Header("炮塔绑定")]
    [SerializeField] private Transform fort;
    [SerializeField] private Transform originalDir;

    [Header("基础参数")]
    [SerializeField] private bool autoRotation_enable;
    [SerializeField] private float max_moveAngle;      //最大自转角
    [SerializeField] private int dir = 1;               //旋转方向(1为顺时针)
    [SerializeField] private float speed_moveAngle;    //每秒自旋转角速度
    [SerializeField] private float search_distance;     //检测距离
    [SerializeField] private float search_angle;        //检测角
    [SerializeField] private LayerMask mask_player;
    [SerializeField] private FortControl FortMode;

    [Header("攻击参数")]
    [SerializeField] private GameObject target;
    [SerializeField] private Transform attack_Pos;
    [SerializeField] private float attack_damage;
    [SerializeField] private float attack_cooldown;
    [SerializeField] private int bullet_count;
    [SerializeField] private bool bullet_instantiate;
    [SerializeField] private GameObject bullet_Prefab;

    //private bool isSearching;
    //private bool isAttacking;
    private float moved_Angle;
    [SerializeField] private bool needAttack;
    [SerializeField] private bool needFllow;
    //public Camera cam;

    private void Start()
    {
        autoRotation_enable = true;
        //isSearching = true;
        //isAttacking = false;
        needAttack = false;
        needFllow = false;
        moved_Angle = 0f;
    }

    private void Update()
    {
        switch (FortMode)
        {
            case FortControl.Auto:
                {
                    //创建一个检测范围为半径的球形区域
                    Collider[] players = Physics.OverlapSphere(fort.position, search_distance, mask_player);
                    if (players.Length > 0)
                    {
                        if (players[0].TryGetComponent(out CharacterStateHolder stateHolder))
                        {
                            //实现锥形检测
                            Vector3 playerPos = players[0].GetComponent<Transform>().position;
                            Vector3 d1 = playerPos - fort.position;
                            float a1 = Vector3.Angle(fort.forward, d1);
                            if (a1 <= search_angle)
                            {
                                //射线检测玩家与炮台视角间是否有阻挡
                                Ray ray = new Ray();
                                ray.origin = fort.position;
                                ray.direction = d1;
                                RaycastHit _hitInfo = new RaycastHit();
                                if (Physics.Raycast(ray, out _hitInfo, search_distance))
                                {
                                    if (_hitInfo.collider.CompareTag("Player"))
                                    {
                                        //增加角色的潜行点数
                                        float p = a1 / search_angle;
                                        p = 2f - p;
                                        stateHolder.discoverState.OnBeDiscovered(p);
                                        Debug.Log("Found Player");
                                    }
                                }
                            }

                            //角色潜行条满后触发
                            if (stateHolder.discoverState.Bediscovered == true)
                            {
                                target = players[0].gameObject;
                                autoRotation_enable = false;

                                //角度判断，当玩家位置超出最大移动角则停下
                                Vector3 d2 = playerPos - originalDir.position;
                                Vector3 d3 = Vector3.ProjectOnPlane(d2, originalDir.up.normalized);
                                float a2 = Vector3.Angle(d3, originalDir.forward);
                                if (Vector3.Cross(d3, originalDir.forward).y < 0)
                                    a2 = -a2;
                                if (a2 > max_moveAngle)
                                {
                                    Quaternion q = Quaternion.Euler(fort.localEulerAngles.x, -max_moveAngle, fort.localEulerAngles.z);
                                    fort.localRotation = q;
                                    //isAttacking = false;
                                    needAttack = false;
                                    needFllow = false;
                                    //恢复自动旋转
                                    autoRotation_enable = true;
                                    Debug.Log("Player Leave!");
                                }
                                else if (a2 < -max_moveAngle)
                                {
                                    Quaternion q = Quaternion.Euler(fort.localEulerAngles.x, max_moveAngle, fort.localEulerAngles.z);
                                    fort.localRotation = q;
                                    //isAttacking = false;
                                    needAttack = false;
                                    needFllow = false;
                                    //恢复自动旋转
                                    autoRotation_enable = true;
                                    Debug.Log("Player Leave!");
                                }
                                //当玩家在移动角范围内则开始攻击
                                else //if (a2 >= -max_move_angle && a2 <= max_move_angle)
                                {
                                    //isAttacking = true;
                                    if (!needAttack)
                                    {
                                        needAttack = true;
                                        needFllow = true;
                                        StartCoroutine(AutoAttack(stateHolder));
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            case FortControl.Handle:
                {

                }
                break;
        }

    }

    private void LateUpdate()
    {
        if (autoRotation_enable)
            AutoRotateY();
        else if (needFllow)
            AutoFollowPlayer(target.transform.position);
    }

    /// <summary>
    /// 沿自身Y轴进行自转
    /// </summary>
    private void AutoRotateY()
    {
        //获取角度增量
        moved_Angle += dir * speed_moveAngle * Time.deltaTime;
        if (moved_Angle >= max_moveAngle || moved_Angle <= -max_moveAngle)
            dir = -dir;
        //应用旋转
        Quaternion q = Quaternion.AngleAxis(moved_Angle, Vector3.up);
        fort.localRotation = q;
    }

    /// <summary>
    /// 自动旋转至目标位置(有平滑处理)
    /// </summary>
    /// <param name="p">目标的position</param>
    private void AutoFollowPlayer(Vector3 p)
    {
        //float t = speed_move_angle * Time.deltaTime;
        //p = Vector3.Lerp(fort.forward, p, t);
        //fort.LookAt(p);

        Vector3 d1 = p - fort.position;
        float t = speed_moveAngle * Time.deltaTime;
        Vector3 d2 = Vector3.Slerp(fort.forward, d1, t);
        fort.rotation = Quaternion.LookRotation(d2);
    }

    private IEnumerator AutoAttack(CharacterStateHolder stateHolder)
    {
        //子弹实例化
        if (bullet_instantiate && bullet_Prefab != null)
        {

            for (int i = 0; i < bullet_count; i++)
            {
                GameObject b = GameObjectPoolManager.SpawnObject(bullet_Prefab, attack_Pos.position, Quaternion.identity);
            }

        }

        //伤害传入
        while (needAttack)
        {
            Debug.Log("Attacking Player");
            stateHolder.healthState.TakeDamage(attack_damage, DamageType.Explosion, (target.transform.position - attack_Pos.position).normalized);
            yield return new WaitForSeconds(attack_cooldown);
        }
        yield break;
    }

    private void OnDrawGizmosSelected()
    {
        if (fort != null)
        {
            //显示监测范围
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(fort.position, search_distance);

            //显示攻击范围
            Gizmos.color = Color.red;
            //float radius = search_distance * Mathf.Tan(search_angle * Mathf.Deg2Rad);
            //Vector3 t = fort.position;
            //Vector3 r = fort.right;
            //Vector3 f = fort.forward;
            //Vector3 u = fort.up;
            //Gizmos.DrawLine(t, t + f * search_distance + u * radius);
            //Gizmos.DrawLine(t, t + f * search_distance - u * radius);
            //Gizmos.DrawLine(t, t + f * search_distance + r * radius);
            //Gizmos.DrawLine(t, t + f * search_distance - r * radius);
            //Gizmos.DrawLine(t + f * search_distance + u * radius, t + f * search_distance - u * radius);
            //Gizmos.DrawLine(t + f * search_distance + r * radius, t + f * search_distance - r * radius);
            Gizmos.matrix = fort.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, search_angle, search_distance, 0f, 1f);
        }
    }
}

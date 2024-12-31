using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Playables;

public class MovingPlatform : MonoBehaviour, IMoverController
{
    private PhysicsMover mover;
    private PlayableDirector director;
    private Transform tr;

    private void Start()
    {
        mover.MoverController = this;
        director = GetComponent<PlayableDirector>();
        tr = GetComponent<Transform>();
    }

    /// <summary>
    /// 告诉平台下一个fixedUpdate的位置应该在哪
    /// </summary>
    /// <param name="goalPosition">下一帧动画时平台的位置</param>
    /// <param name="goalRotation">下一帧动画时平台的旋转</param>
    /// <param name="deltaTime">fixedUpdateTime</param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        //动画播放前缓存平台位置
        Vector3 _positionBeforeAnim = tr.position;
        Quaternion _rotationBeforeAnim = tr.rotation;

        //更新动画，此时平台位置与动画计算后同步
        EvaluateAtTime(Time.time);

        //输出平台动画后位置，PhysicsMover计算物理数据并保存，等待KCC系统读取数据并进行物理模拟
        goalPosition = tr.position;
        goalRotation = tr.rotation;

        //重置平台位置为缓存位置
        tr.SetPositionAndRotation(_positionBeforeAnim, _rotationBeforeAnim);
    }

    public void EvaluateAtTime(double time)
    {
        director.time = time % director.duration;
        director.Evaluate();
    }
}

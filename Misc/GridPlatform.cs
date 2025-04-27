using System.Collections;
using UnityEngine;

public class GridPlatform : MonoBehaviour
{
    public Transform platform;
    public float rotationDuration = 0.5f; // 平滑旋转用时
    public float flipInterval = 2.0f;      // 每隔多久翻转一次，用户可配置

    protected bool m_clockwise = true;
    private Coroutine m_flipCoroutine;

    private void Start()
    {
        m_flipCoroutine = StartCoroutine(AutoFlipRoutine());
    }

    private IEnumerator AutoFlipRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flipInterval);
            Move();
        }
    }

    public virtual void Move()
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine());
        m_flipCoroutine = StartCoroutine(AutoFlipRoutine()); // 重新启动自动翻转
    }

    protected IEnumerator MoveRoutine()
    {
        var elapsedTime = 0f;
        var from = platform.localRotation;
        var to = Quaternion.Euler(0, 0, m_clockwise ? 180 : 0);
        m_clockwise = !m_clockwise;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            platform.localRotation = Quaternion.Lerp(from, to, elapsedTime / rotationDuration);
            yield return null;
        }

        platform.localRotation = to;
    }
}
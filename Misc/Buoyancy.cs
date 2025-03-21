using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    public float force = 10f;
    protected Rigidbody m_rb;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTag.VolumeWater))
        {
            if (transform.position.y < other.bounds.max.y)
            {
                // 模拟浮力
                var multiplier = Mathf.Clamp01(other.bounds.max.y - transform.position.y);
                var buoyancy = Vector3.up * force * multiplier;
                m_rb.AddForce(buoyancy);
            }
        }
    }
}
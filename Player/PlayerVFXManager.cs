using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    private static readonly int Property = Shader.PropertyToID("_dissolve_amount");
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public Material skinnedDeathMaterial;
    
    public float dissolveRate;
    public float dissolveDeltaTime;

    private void Awake()
    {
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    private void Start()
    {
        this.GetComponent<Player>().playerEvents.OnDie.AddListener(DeathMat);
    }
    
    private void DeathMat() => skinnedMeshRenderer.material = skinnedDeathMaterial;
                
}

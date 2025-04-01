using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public OctreeNode root;
    public Bounds bounds;

    public Octree(GameObject[] worldObjects, float minSize)
    {
        CalculateBounds(worldObjects);
        CreateTree(worldObjects, minSize);
    }

    void CreateTree(GameObject[] worldObjects, float minSize)
    {
        root = new OctreeNode(bounds, minSize);

        foreach (var gameObject in worldObjects)
        {
            root.Divide(gameObject);
        }
    }

    void CalculateBounds(GameObject[] worldObjects)
    {
        foreach (var obj in worldObjects)
        {
            bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
            Vector3 size = Vector3.one * Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) * 0.6f;
            bounds.SetMinMax(bounds.center - size, bounds.center + size);
        }
    }
}
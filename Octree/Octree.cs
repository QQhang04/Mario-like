using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    public OctreeNode root;
    public Bounds bounds;
    public Graph graph;
    List<OctreeNode> emptyLeaves = new();

    public Octree(GameObject[] worldObjects, float minSize, Graph graph)
    {
        this.graph = graph;
        
        CalculateBounds(worldObjects);
        CreateTree(worldObjects, minSize);
        
        GetEmptyLeaves(root);
        GetEdges();
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

    void GetEmptyLeaves(OctreeNode node)
    {
        if (node.IsLeaf && node.objects.Count == 0)
        {
            emptyLeaves.Add(node);
            graph.AddNode(node);
        }
        
        if (node.children == null) return;

        foreach (var child in node.children)
        {
            GetEmptyLeaves(child);
        }
        
        // 遍历 node.children 的所有子节点，两两连接，添加到 graph 中
        //	让同一个父节点下的所有叶子节点在 graph 中建立互通路径
        //  确保八叉树中的邻接叶子节点可以互相连通
        for (int i = 0; i < node.children.Length; i++) {
            for (int j = i + 1; j < node.children.Length; j++) {
                if (i == j) continue; 
                graph.AddEdge(node.children[i], node.children[j]);
            }
        }
    }
    
    void GetEdges() {
        foreach (OctreeNode leaf in emptyLeaves) {
            foreach (OctreeNode otherLeaf in emptyLeaves) {
                if (leaf.Equals(otherLeaf)) continue;
                if (leaf.bounds.Intersects(otherLeaf.bounds)) {
                    graph.AddEdge(leaf, otherLeaf);
                }
            }
        }
    }
    
    public OctreeNode FindClosestNode(Vector3 position) => FindClosestNode(root, position);

    public OctreeNode FindClosestNode(OctreeNode node, Vector3 position) {
        OctreeNode found = null;
        for (int i = 0; i < node.children.Length; i++) {
            if (node.children[i].bounds.Contains(position)) {
                if (node.children[i].IsLeaf) {
                    found = node.children[i];
                    break;
                }
                found = FindClosestNode(node.children[i], position);
            }
        }
        return found;
    }
}
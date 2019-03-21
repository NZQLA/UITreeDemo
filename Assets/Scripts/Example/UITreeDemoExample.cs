using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 示例控制脚本
///     本示例旨在随机生成指定数量的节点
/// </summary>
public class UITreeDemoExample : MonoBehaviour
{
    [SerializeField]
    private UITreeRootCtrl UITreeCtrl;



    public int NodeCount = 100;
    public int RootNodeCountMax = 10;
    public int RandChildPower = 70;


    private void Start()
    {
    }

    [ContextMenu("生成节点并替换")]
    private void CreateNodesForTree()
    {
        UITreeCtrl.selfData.SetNodesData(CreateTreeNodes());
    }



    [ContextMenu("生成节点")]
    List<UITreeItemNode> CreateTreeNodes()
    {
        if (UITreeCtrl == null)
        {
            NZQLA.Log.LogAtUnityEditorError("NULL");
            return null;
        }

        List<UITreeItemNode> nodes = new List<UITreeItemNode>();
        int rootNodeCount = Random.Range(1, RootNodeCountMax);
        //添加根节点
        for (int i = 0; i < rootNodeCount; i++)
        {
            nodes.Add(PoolManagerSingltonAuto<UITreeItemNode>.GetIns().GetOneItem());
        }
        NodeCount -= rootNodeCount;

        int indexRootNode = 0;
        while (--NodeCount >= 0)
        {
            UITreeItemNode tempNode = RandOneNode(nodes, RandChildPower);
            if (tempNode == null)
            {
                NZQLA.Log.LogAtUnityEditorError("ERROR");
                break;
            }

            if (tempNode.m_children == null)
            {
                tempNode.m_children = new List<UITreeItemNode>();
            }
            tempNode.m_children.Add(PoolManagerSingltonAuto<UITreeItemNode>.GetIns().GetOneItem());
        }

        return nodes;
    }

    //随机出一个节点
    UITreeItemNode RandOneNode(List<UITreeItemNode> nodes, int randChildPower)
    {
        if (nodes == null || nodes.Count <= 0)
        {
            return null;
        }
        UITreeItemNode rootNode = nodes[Random.Range(0, nodes.Count)];
        UITreeItemNode tempNode = rootNode;
        while (tempNode.HavChild())
        {
            if (Random.Range(0, 100) < randChildPower)
            {
                tempNode = tempNode.m_children[Random.Range(0, tempNode.m_children.Count)];
                break;
            }
        }
        return tempNode;
    }



}

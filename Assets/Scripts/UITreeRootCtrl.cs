using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Serialization;
using NZQLA;
using System;
using LitJson;
using UnityEngine.Profiling;

public class UITreeRootCtrl : MonoBehaviour
{
    private MonoPoolManager<UITreeNodeView> nodeViewPool;
    public MonoPoolManager<UITreeNodeView> NodeViewPool
    {
        get
        {
            if (nodeViewPool == null)
            {
                nodeViewPool = new MonoPoolManager<UITreeNodeView>();
                nodeViewPool.Prefab = UITreeCellViewPrefab;
            }
            return nodeViewPool;
        }
    }


    [SerializeField]
    [Tooltip("元素的预制体")]
    private UITreeNodeView UITreeCellViewPrefab;

    [SerializeField]
    private UITreeRootView SelfView;

    [SerializeField]
    private UITreeRootData selfData = new UITreeRootData();

    private void Awake()
    {
        TestDeseralizeItemNodes();
        InitViewOnStart();
    }


    private void OnEnable()
    {
        RefreshView();
    }





    public Vector3 GetNodeViewAnchorposAtIndex(int m_indexHor, int m_indexVer)
    {
        Vector3 vPosTest = Vector3.zero;
        vPosTest.y -= selfData.m_cellSize.y * m_indexVer;
        vPosTest.x += selfData.m_cellOffsetHorizontal * m_indexHor;
        return vPosTest;
    }


    [ContextMenu("SerializeNodesData")]
    void TestSerializeNodesData()
    {
        //初始化来自编辑器的节点数据
        selfData.InitUITreeNodeIndexAndSetParent();
        //有节点数据生成序列化数据
        var dataSerialize = UITreeExtension.SerializeItemsNodes(selfData.m_children);

        //将可序列化的数据进行序列化
        var jsonData = JsonMapper.ToJson(dataSerialize);
        FileTool.WriteStringToFileByFileStream(GetSerailizeFilePath(), jsonData, null, System.Text.Encoding.UTF8, false);
    }

    string GetSerailizeFilePath()
    {
        return Application.streamingAssetsPath + "/" + "NodesSeralizeData.json";
    }




    [ContextMenu("测试反序列化")]
    void TestDeseralizeItemNodes()
    {
        string strSerailzeData = FileTool.ReadFile(GetSerailizeFilePath(), System.Text.Encoding.UTF8, null);
        var dataDeseralize = JsonMapper.ToObject<Dictionary<string, UITreeItemSerlizeData>>(new JsonReader(strSerailzeData));
        selfData.m_children = UITreeExtension.DeSerializeUtemsFromSerializeData(dataDeseralize);
    }


    //public UITreeCellView GetNext


    public void RefreshView()
    {

    }


    [ContextMenu("InitViewOnStart")]
    public void InitViewOnStart()
    {
        if (selfData == null || SelfView == null)
        {
            //....ERROR
            NZQLA.Log.LogAtUnityEditor("NULL", "#ff00ffff");
            return;
        }

        int cellCountInit = CaclulateCellsCountShow();
        if (cellCountInit == 0)
        {
            //... 这个目录是空的，没有一个元素
            NZQLA.Log.LogAtUnityEditor("这个目录是空的，没有一个元素", "#ff00ffff");
            return;
        }

        //获取元素尺寸
        if (UITreeCellViewPrefab != null)
        {
            selfData.m_cellSize = UITreeCellViewPrefab.selfRect.rect.size;
        }
        selfData.m_cellSize = UITreeCellViewPrefab.selfRect.rect.size;


        //这里进行初始化//实例化足够的View元素并刷新其显示
        selfData.InitNodes(this);
    }


    //TODO 使用对象池
    private UITreeNodeView TempInstateOneItemView(UITreeNodeView prefab, GameObject uiParent)
    {


        if (prefab == null)
        {
            //...ERROR
            NZQLA.Log.LogAtUnityEditor("预制体为空", "#ff00ffff");
            return null;
        }

        if (uiParent == null)
        {
            //...ERROR
            NZQLA.Log.LogAtUnityEditor("未指定元素UI的父物体", "#ff00ffff");
            return null;
        }

        UITreeNodeView tempNodeViewIns = NodeViewPool.GetOneItem();

        //UITreeNodeView tempNodeViewIns = GameObject.Instantiate<UITreeNodeView>(prefab);
        tempNodeViewIns.transform.SetParent(uiParent.transform, false);
        return tempNodeViewIns;
    }
    public UITreeNodeView TempInstateOneItemView()
    {
        return TempInstateOneItemView(UITreeCellViewPrefab, SelfView.UITreeContentScrollView.content.gameObject);
    }

    ////用于计算整个UITree目录的尺寸
    //private Vector2 CaculateContentViewSize(int cellsCount)
    //{
    //    if (selfRootData == null)
    //    {
    //        return Vector2.zero;
    //    }
    //    return cellsCount*
    //    Vector2 vSize = Vector2.zero;
    //    return CaclulateCellsCountShow();
    //}


    /// <summary>
    /// 计算当前所展示出来的元素数量
    /// 这是一个自上而下的主动式统计方式，不适合频繁调用
    /// </summary>
    /// <returns></returns>
    public int CaclulateCellsCountShow()
    {
        if (selfData == null || selfData.m_children == null || selfData.m_children.Count == 0)
        {
            return 0;
        }

        int cellCount = 0;
        for (int i = 0; i < selfData.m_children.Count; i++)
        {
            if (selfData.m_children[i] != null)
            {
                cellCount += selfData.m_children[i].GetCellCountShow();
            }
        }
        return cellCount;
    }


    public void OnNodesFoldOrUnFold()
    {
        selfData.InitNodes(this);
    }


}






/// <summary>
/// 元素数据
/// </summary>
[System.Serializable]
//[XmlRoot]
public class UITreeRootData
{
    //[XmlArray]
    public List<UITreeItemNode> m_children;

    /// <summary>元素行间距</summary>
    public float m_cellOffsetHorizontal;

    /// <summary>元素缩进距离</summary>
    public float m_cellRetract;

    /// <summary>元素尺寸</summary>
    public Vector2 m_cellSize;


    /// <summary>初始化节点//节点index//节点View</summary>
    /// <param name="uITreeRootCtrl"></param>
    public void InitNodes(UITreeRootCtrl uITreeRootCtrl)
    {
        NZQLA.Log.LogAtUnityEditorNormal("Init Root Nodes");
        int indexHor = 0, indexVer = 0;

        for (int i = 0; i < m_children.Count; i++)
        {
            if (m_children[i] != null)
            {
                indexHor = 0;
                m_children[i].InitNodeIndex(ref indexHor, ref indexVer, true);
                m_children[i].InitNode(uITreeRootCtrl);
            }
        }
    }

    public void InitNodesIndex()
    {
        if (m_children.isNull())
        {
            //TODO
            return;
        }

        int indexHor = 0, indexVer = 0;
        for (int i = 0; i < m_children.Count; i++)
        {
            m_children[i].InitNodeIndex(ref indexHor, ref indexVer, true);
        }
    }


}
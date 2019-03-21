using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using NZQLA;


/// <summary>UITree周边的扩展方法</summary>
public static class UITreeExtension
{
    public const string NullItemDataLayerName = "-1";
    public const string ItemIDConnectChar = "-";


    public static void InitNodeIndex(this UITreeItemNode self,ref int indexHor,ref int indexVer,bool bInitChildrenAlso = true)
    {
        if (self == null)
        {
            //TODO
            return;
        }

        self.m_indexHor = indexHor;
        self.m_indexVer = indexVer;
        indexVer++;

        //递归处理子节点
        if (!bInitChildrenAlso || !self.HavChild() || self.m_data.m_openState != UITreeCellOpenState.Open)
        {
            return;
        }
        indexHor++;
        for (int i = 0; i < self.m_children.Count; i++)
        {
            self.m_children[i].InitNodeIndex(ref indexHor, ref indexVer);
        }
    }


    public static UITreeItemCtrl ReadyItemCtrl(this UITreeItemNode self)
    {
        if (self == null)
        {
            return null;
        }

        if (self.m_ctrl != null)
        {
            return self.m_ctrl;
        }

        self.m_ctrl = new UITreeItemCtrl();
        self.m_ctrl.selfData = self;
        self.m_ctrl.OnHideHandler += self.OnHide;

        return self.m_ctrl;
    }

    public static void InitNode(this UITreeItemNode self, UITreeRootCtrl rootCtrl, bool bInitChildrenAlso = true)
    {
        if (self == null || rootCtrl == null)
        {
            //ERROR
            NZQLA.Log.LogAtUnityEditorError("NULL");
            return;
        }

        self.ReadyItemCtrl();
        //TEMP
        self.m_ctrl.OnNodesFoldOrUnFoldHandler -= rootCtrl.OnNodesFoldOrUnFold;
        self.m_ctrl.OnNodesFoldOrUnFoldHandler += rootCtrl.OnNodesFoldOrUnFold;
        self.m_ctrl.Init(rootCtrl);

        //处理子节点(隐藏节点不考虑)
        if (self.HavChild() && bInitChildrenAlso&&self.m_data.m_openState == UITreeCellOpenState.Open)
        {
            for (int i = 0; i < self.m_children.Count; i++)
            {
                self.m_children[i].InitNode(rootCtrl, bInitChildrenAlso);
            }
        }

    }


    /// <summary>将节点下的子节点的Index初始化，这样在编辑器就不用在输入节点的Index了</summary>
    /// <param name="self"></param>
    public static void InitNodeChildrenIndexAndSetParent(this UITreeItemNode self)
    {
        if (self == null || !self.HavChild())
        {
            return;
        }

        for (int i = 0; i < self.m_children.Count; i++)
        {
            self.m_children[i].m_data.m_index = i;
            InitNodeChildrenIndexAndSetParent(self.m_children[i]);

            self.m_children[i].m_parent = self;
        }
    }


    /// <summary>将所有节点的Index初始化，这样在编辑器就不用在输入节点的Index了</summary>
    /// <param name="self"></param>
    public static void InitUITreeNodeIndexAndSetParent(this UITreeRootData self)
    {
        if (self == null || self.m_children == null || self.m_children.Count == 0)
        {
            return;
        }

        for (int i = 0; i < self.m_children.Count; i++)
        {
            self.m_children[i].m_data.m_index = i;
            InitNodeChildrenIndexAndSetParent(self.m_children[i]);
            self.m_children[i].m_parent = null;
        }
    }



    /// <summary>将该节点及以下所有子节点数据转换为一维的序列化数据并缓存进指定的Dictionary</summary>
    /// <param name="self"></param>
    /// <param name="parentID"></param>
    /// <param name="dicSerializeData"></param>
    public static void StatisticsItemsSerializeDataAtNodesData(this UITreeItemNode self, string parentID, ref Dictionary<string, UITreeItemSerlizeData> dicSerializeData)
    {
        if (self == null)
        {
            return;
        }

        if (dicSerializeData == null)
        {
            dicSerializeData = new Dictionary<string, UITreeItemSerlizeData>();
        }

        StringBuilder sbTemp = CreateNodeID(self, parentID);
        self.m_selfID = sbTemp.ToString();
        //生成自身节点并缓存
        if (!dicSerializeData.ContainsKey(self.m_selfID))
        {
            UITreeItemSerlizeData data = new UITreeItemSerlizeData(self);
            dicSerializeData.Add(sbTemp.ToString(), data);
        }

        //递归处理子节点
        for (int i = 0; i < self.m_children.Count; i++)
        {
            StatisticsItemsSerializeDataAtNodesData(self.m_children[i], sbTemp.ToString(), ref dicSerializeData);
        }
    }

    /// <summary>生成节点ID</summary>
    /// <param name="self"></param>
    /// <param name="parentID"></param>
    /// <returns></returns>
    public static StringBuilder CreateNodeID(this UITreeItemNode self, string parentID)
    {
        //处理LayerName
        StringBuilder sbTemp = new StringBuilder();
        if (!string.IsNullOrEmpty(parentID))
        {
            sbTemp.Append(parentID);
            sbTemp.Append(ItemIDConnectChar);
        }
        sbTemp.AppendFormat("{0}", self.m_data.m_index);
        return sbTemp;
    }

    /// <summary>将Node List转换为一维的序列化数据并缓存进指定的Dictionary</summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public static Dictionary<string, UITreeItemSerlizeData> SerializeItemsNodes(List<UITreeItemNode> nodes)
    {
        if (nodes == null || nodes.Count == 0)
        {
            NZQLA.Log.LogAtUnityEditorWarning("Null");
            return null;
        }

        Dictionary<string, UITreeItemSerlizeData> dicSerializeData = new Dictionary<string, UITreeItemSerlizeData>();
        StringBuilder sbTemp = new StringBuilder();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null)
            {
                continue;
            }

            StatisticsItemsSerializeDataAtNodesData(nodes[i], null, ref dicSerializeData);
        }

        return dicSerializeData;
    }


    /// <summary>将序列化元素数据转换为链表式的结构</summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static List<UITreeItemNode> DeSerializeUtemsFromSerializeData(Dictionary<string, UITreeItemSerlizeData> dicItemsSeralizeData)
    {
        if (dicItemsSeralizeData.isNull())
        {
            Log.LogAtUnityEditorWarning("Null");
            return null;
        }

        //List<string> rootNodesIDList = new List<string>();
        List<UITreeItemNode> rootNodesList = new List<UITreeItemNode>();

        //先依据序列化数据一一生成Node数据//并统计出Root节点
        Dictionary<string, UITreeItemNode> dicNodes = new Dictionary<string, UITreeItemNode>();
        var dicItemsSeralizeDataIE = dicItemsSeralizeData.GetEnumerator();
        while (dicItemsSeralizeDataIE.MoveNext())
        {
            if (dicItemsSeralizeDataIE.Current.Value != null)
            {
                UITreeItemNode tempNode = new UITreeItemNode() { m_data = dicItemsSeralizeDataIE.Current.Value.m_data };
                dicNodes.Add(dicItemsSeralizeDataIE.Current.Key, tempNode);
                tempNode.m_selfID = dicItemsSeralizeDataIE.Current.Value.m_selfID;


                if (dicItemsSeralizeDataIE.Current.Value.m_data.m_isRoot)
                {
                    //rootNodesIDList.Add(dicItemsSeralizeDataIE.Current.Key);
                    rootNodesList.Add(tempNode);
                }
            }
        }

        //处理父子节点关系
        var dicNodesIE = dicNodes.GetEnumerator();
        while (dicNodesIE.MoveNext())
        {
            if (dicNodesIE.Current.Value != null)
            {
                UITreeItemNode tempNode = dicNodes[dicNodesIE.Current.Key];
                UITreeItemSerlizeData tempSeralizeData = dicItemsSeralizeData[dicNodesIE.Current.Key];
                //绑定父节点
                if (!string.IsNullOrEmpty(tempSeralizeData.m_parentID))
                {
                    //安全容错
                    tempNode.m_parent = dicNodes[tempSeralizeData.m_parentID];
                }

                //绑定子节点
                if (!tempSeralizeData.m_childrenID.isNull())
                {
                    List<UITreeItemNode> tempChildernNodes = new List<UITreeItemNode>();
                    for (int i = 0; i < tempSeralizeData.m_childrenID.Count; i++)
                    {
                        //UITreeItemNode tempNode1 = null;
                        tempChildernNodes.Add(dicNodes[tempSeralizeData.m_childrenID[i]]);
                    }
                    tempNode.m_children = tempChildernNodes;
                }
            }
        }

        return rootNodesList;
    }


}




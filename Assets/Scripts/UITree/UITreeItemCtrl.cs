using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;


public class UITreeItemCtrl
{
    public UITreeRootCtrl TreeRootCtrl { get; set; }
    public UITreeItemNode selfData { get; set; }
    public UITreeNodeView SelfView { get; set; }
    public bool bHaveInit;

    public event Action OnNodesFoldOrUnFoldHandler;
    public event Action OnHideHandler;


    public void Init(UITreeRootCtrl uITreeRootCtrl)
    {
        TreeRootCtrl = uITreeRootCtrl;
        ReadyView();

        if (!bHaveInit)
        {
            bHaveInit = true;
            selfData.m_data.m_openState = selfData.HavChild() ? UITreeCellOpenState.Close : UITreeCellOpenState.CanNotOpen;
        }
    }


    void ReadyView()
    {
        if (SelfView == null)
        {
            SelfView = TreeRootCtrl.TempInstateOneItemView();
        }
        SelfView.selfCtrl = this;
        SelfView.SetAnchoredPos(TreeRootCtrl.GetNodeViewAnchorposAtIndex(selfData.m_indexHor, selfData.m_indexVer));
        InitViewClickEvents();
        RefreshView();
    }

    public void RefreshView()
    {
        if (SelfView == null)
        {
            //ERROR
            NZQLA.Log.LogAtUnityEditorError("NULL");
            return;
        }

        SelfView.RefreshViewContent();
    }

    public void InitViewClickEvents()
    {
        if (SelfView != null)
        {
            SelfView.InitNodeSwitchEvents(OnClickBtnOpenStateSwitch);
            SelfView.InitNodeEvents(OnClickBtnNode);
        }
    }

    /// <summary>�л�Ԫ��չ����ť�ķ���</summary>
    public void OnClickBtnOpenStateSwitch()
    {
        //TEMP
        NZQLA.Log.LogAtUnityEditor(string.Format("Click Node Switch {0}", selfData.m_selfID), "#ffffffff");


        if (selfData == null || !selfData.HavChild())
        {
            NZQLA.Log.LogAtUnityEditorNormal("�޷�չ��");
            //�޷�չ��
            return;
        }

        FoldOrUnfoldView();
    }

    void FoldOrUnfoldView()
    {
        switch (selfData.m_data.m_openState)
        {
            case UITreeCellOpenState.CanNotOpen:
                //DoNothing
                return;
            case UITreeCellOpenState.Close:
                selfData.m_data.m_openState = UITreeCellOpenState.Open;
                break;
            case UITreeCellOpenState.Open:
                HideChildren();
                selfData.m_data.m_openState = UITreeCellOpenState.Close;
                break;
        }

        //�������۵�/չ������ĸ�������
        OnNodesFoldOrUnFoldHandler();
    }


    public void OnClickBtnNode()
    {
        //TEMP
        NZQLA.Log.LogAtUnityEditor(string.Format("Click Node {0}", selfData.m_selfID), "#ffffffff");
    }


    void HideChildren()
    {
        var IE = selfData.m_children.GetEnumerator();
        while (IE.MoveNext())
        {
            if (IE.Current != null)
            {
                IE.Current.m_ctrl.OnHide();
            }
        }
    }

    public void OnHide()
    {
        OnHideHandler();
        RecycleView();
    }

    //���յ�ǰ�ڵ��Ӧ��View�����ǽڵ�������ݱ��ֲ���
    public void RecycleView()
    {
        SelfView.MonoItemRecycle();
        TreeRootCtrl.NodeViewPool.RecycleItem(SelfView);

        //�ݹ�����ӽڵ��View
        if (selfData.HavChild() && selfData.m_data.m_openState == UITreeCellOpenState.Open)
        {
            var IE = selfData.m_children.GetEnumerator();
            while (IE.MoveNext())
            {
                IE.Current.m_ctrl.OnHide();
            }
        }
    }
}



/// <summary>
/// Ԫ������(��Ϊһ��������ʹ�ã�Ҳ���ʺ�UITree�����ݽṹ��ʽ�����ǲ��ܹ��������ݵ����л��뷴���л�)
/// </summary>
[Serializable]
//[XmlRoot]
public class UITreeItemNode
{
    public UITreeItemCtrl m_ctrl;

    //[XmlElement]
    [HideInInspector]
    public UITreeItemNode m_parent;

    //[XmlArray]
    public List<UITreeItemNode> m_children;

    [HideInInspector]
    public int m_indexHor, m_indexVer;


    //[XmlElement]
    public UITreeItemData m_data;

    [HideInInspector]
    public string m_selfID;


    public UITreeItemNode()
    {
        m_data = new UITreeItemData();
    }


    public bool HavChild()
    {
        return !(m_children == null || m_children.Count == 0);
    }

    /// <summary>��ȡ������������չʾ��Ԫ������</summary>
    /// <returns></returns>
    public int GetCellCountShow()
    {
        int count = 1;
        if (!HavChild())
        {
            return count;
        }

        for (int i = 0; i < m_children.Count; i++)
        {
            count += m_children[i].GetCellCountShow();
        }
        return count;
    }

    public void OnHide()
    {
        m_indexHor = m_indexVer = 0;
    }
}


/// <summary>
/// Ԫ������(֧�����л��뷴���л�)������һά����ʽ���ڷ����ȡ
/// </summary>
[Serializable]
[XmlRoot]
public class UITreeItemSerlizeData
{
    [XmlElement]
    public UITreeItemData m_data;



    [XmlAttribute]
    public string m_selfID;


    [XmlAttribute]
    public string m_parentID;

    [XmlElement]
    public List<string> m_childrenID;


    public UITreeItemSerlizeData() { }
    public UITreeItemSerlizeData(UITreeItemNode node)
    {
        if (node != null)
        {
            m_data = node.m_data;

            //����ID
            m_selfID = node.m_selfID;

            if (node.m_parent != null)
            {
                m_parentID = node.m_parent.m_selfID;
            }

            if (node.HavChild())
            {
                m_childrenID = new List<string>();
                for (int i = 0; i < node.m_children.Count; i++)
                {
                    if (node.m_children[i] != null)
                    {
                        //����ӽڵ��ID��δ���ɣ���ǰ����
                        if (string.IsNullOrEmpty(node.m_children[i].m_selfID))
                        {
                            node.m_children[i].m_selfID = node.m_children[i].CreateNodeID(m_selfID).ToString();
                            //CreateNodeID(node.m_children[i], m_selfID);
                        }
                        m_childrenID.Add(node.m_children[i].m_selfID);
                    }
                }
            }
        }
    }

}


[Serializable]
[XmlRoot]
public class UITreeItemData
{
    [XmlAttribute]
    public string m_cellName;

    [XmlAttribute]
    public bool m_isRoot;

    [HideInInspector]
    [XmlAttribute]
    public int m_index;

    [HideInInspector]
    [XmlAttribute]
    public UITreeCellOpenState m_openState;

}



/// <summary>UIԪ�صĴ�״̬</summary>
public enum UITreeCellOpenState
{
    CanNotOpen,
    Close,
    Open,
}




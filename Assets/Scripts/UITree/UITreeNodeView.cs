using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 元素视图[UGUI]
/// </summary>
public class UITreeNodeView : MonoBehaviour, IMonoPoolItemRecycle<UITreeNodeView>
{
    public RectTransform selfRect;
    public Button UIBtnNodeSwitch;
    public Button UIBtnNode;
    public Text UITextCellName;
    public UITreeItemCtrl selfCtrl;

    public void Init(Action OnOpenStateSwitch)
    {

    }


    public void SetAnchoredPos(Vector3 vPos)
    {
        if (selfRect == null)
        {
            NZQLA.Log.LogAtUnityEditor("Null", "#ff00ffff");
            return;
        }
        selfRect.anchoredPosition3D = vPos;
    }


    public void InitViewPos()
    {

    }


    public void InitNodeSwitchEvents(Action onClickSwitch)
    {
        UIBtnNodeSwitch.BindClickEvents(onClickSwitch);
    }

    public void InitNodeEvents(Action onClickNode)
    {
        UIBtnNode.BindClickEvents(onClickNode);
    }

    public void RemoveViewClickEvents()
    {
        UIBtnNodeSwitch.onClick.RemoveAllListeners();
        UIBtnNode.onClick.RemoveAllListeners();
    }

    public void Reset()
    {
        UITextCellName.text = string.Empty;
        selfRect.gameObject.SetActive(false);
        RemoveViewClickEvents();
        selfCtrl = null;
    }


    /// <summary>依据元素数据刷新显示</summary>
    /// <param name="data"></param>
    public void RefreshViewContent()
    {
        if (selfCtrl == null)
        {
            NZQLA.Log.LogAtUnityEditor("NULL", "#ff00ffff");
            return;
        }

        if (UITextCellName != null)
        {

            //TEST
            UITextCellName.text = string.Format("ID:{0}", selfCtrl.selfData.m_selfID);


            //UITextCellName.text = selfCtrl.selfData.m_data.m_cellName;
            //Temp
            if (!selfRect.gameObject.activeSelf)
            {
                selfRect.gameObject.SetActive(true);
            }
        }
        else
        {
            NZQLA.Log.LogAtUnityEditor("CellView 未成功指定ViewNameText", "#ff00ffff");
            return;
        }
    }

    public void MonoItemRecycle()
    {
        Reset();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public static class MonoUGUIExtension
{

    /// <summary>为UGUI Button绑定点击事件</summary>
    /// <param name="self"></param>
    /// <param name="onClick"></param>
    /// <param name="removeEventsBefore"></param>
    public static int BindClickEvents(this Button self, Action onClick,bool removeEventsBefore = true)
    {
        if (self == null)
        {
            //TODO
            return -1;
        }

        if (onClick == null)
        {
            return -2;
        }

        if (removeEventsBefore)
        {
            self.onClick.RemoveAllListeners();
        }
        self.onClick.AddListener(()=>onClick());

        return 1;
    }


}

/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using UnityEngine;

namespace YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.GridView
{
    /// <summary>
    /// <see cref="FancyCellGroup{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyCellGroupContext
    {
        GameObject CellTemplate { get; set; }
        Func<int> GetGroupCount { get; set; }
    }
}
/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using UnityEngine;
using YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.Scroller;
using YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.ScrollRect;

namespace YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.GridView
{
    /// <summary>
    /// <see cref="FancyGridView{TItemData, TContext}"/> のコンテキスト基底クラス.
    /// </summary>
    public class FancyGridViewContext : IFancyGridViewContext
    {
        ScrollDirection IFancyScrollRectContext.ScrollDirection { get; set; }
        Func<(float ScrollSize, float ReuseMargin)> IFancyScrollRectContext.CalculateScrollSize { get; set; }
        GameObject IFancyCellGroupContext.CellTemplate { get; set; }
        Func<int> IFancyCellGroupContext.GetGroupCount { get; set; }
        Func<float> IFancyGridViewContext.GetStartAxisSpacing { get; set; }
        Func<float> IFancyGridViewContext.GetCellSize { get; set; }
    }
}
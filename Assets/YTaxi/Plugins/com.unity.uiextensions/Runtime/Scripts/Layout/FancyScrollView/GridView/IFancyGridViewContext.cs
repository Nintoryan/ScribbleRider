/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.ScrollRect;

namespace YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.GridView
{
    /// <summary>
    /// <see cref="FancyGridView{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyGridViewContext : IFancyScrollRectContext, IFancyCellGroupContext
    {
        Func<float> GetStartAxisSpacing { get; set; }
        Func<float> GetCellSize { get; set ; }
    }
}
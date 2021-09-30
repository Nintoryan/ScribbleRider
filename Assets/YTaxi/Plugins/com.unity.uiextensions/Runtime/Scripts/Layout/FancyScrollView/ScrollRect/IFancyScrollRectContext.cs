/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.Scroller;

namespace YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.ScrollRect
{
    /// <summary>
    /// <see cref="FancyScrollRect{TItemData, TContext}"/> のコンテキストインターフェース.
    /// </summary>
    public interface IFancyScrollRectContext
    {
        ScrollDirection ScrollDirection { get; set; }
        Func<(float ScrollSize, float ReuseMargin)> CalculateScrollSize { get; set; }
    }
}
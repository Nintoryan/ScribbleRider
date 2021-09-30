/// Credit setchi (https://github.com/setchi)
/// Sourced from - https://github.com/setchi/FancyScrollView

using System;
using YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.Scroller;

namespace YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Layout.FancyScrollView.ScrollRect
{
    /// <summary>
    /// <see cref="FancyScrollRect{TItemData, TContext}"/> のコンテキスト基底クラス.
    /// </summary>
    public class FancyScrollRectContext : IFancyScrollRectContext
    {
        ScrollDirection IFancyScrollRectContext.ScrollDirection { get; set; }
        Func<(float ScrollSize, float ReuseMargin)> IFancyScrollRectContext.CalculateScrollSize { get; set; }
    }
}
/// Credit Erdener Gonenc - @PixelEnvision
/*USAGE: Simply use that instead of the regular ScrollRect */


using UnityEngine;
using UnityEngine.UI;

namespace YTaxi.Plugins.com.unity.uiextensions.Runtime.Scripts.Controls
{
    [AddComponentMenu ("UI/Extensions/MultiTouchScrollRect")]
	public class MultiTouchScrollRect : ScrollRect
	{
		private int pid = -100;

		/// <summary>
		/// Begin drag event
		/// </summary>
		public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			pid = eventData.pointerId;
			base.OnBeginDrag (eventData);
		}

		/// <summary>
		/// Drag event
		/// </summary>
		public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (pid == eventData.pointerId)
				base.OnDrag (eventData);
		}

		/// <summary>
		/// End drag event
		/// </summary>
		public override void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
				pid = -100;
				base.OnEndDrag (eventData);
		}
	}
}
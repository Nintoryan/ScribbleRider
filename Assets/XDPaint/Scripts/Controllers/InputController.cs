// #define VR_ENABLED

#if VR_ENABLED
using System.Collections.Generic;
using UnityEngine.XR;
#endif
using UnityEngine;
using XDPaint.Tools;

namespace XDPaint.Controllers
{
	public class InputController : Singleton<InputController>
	{
		public delegate void OnInputUpdate();
		public delegate void OnInputPosition(Vector3 position);
		public delegate void OnInputPositionPressure(Vector3 position, float pressure = 1.0f);

		public bool IsVRMode;
		public Transform PenTransform;

		public event OnInputUpdate OnUpdate;
		public event OnInputPosition OnMouseHover;
		public event OnInputPositionPressure OnMouseDown;
		public event OnInputPositionPressure OnMouseButton;
		public event OnInputPosition OnMouseUp;
		
		public Camera Camera { private get; set; }
		private int _fingerId = -1;

#if VR_ENABLED
		private List<InputDevice> leftHandedControllers;
#endif
		
		private bool _initialized;
#if UNITY_WEBGL
		private bool _isWebgl = true;
#else
		private bool _isWebgl = false;
#endif

		void Start()
		{
#if VR_ENABLED
			leftHandedControllers = new List<InputDevice>();
			var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
			InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);
#endif
		}
		
		void Update()
		{
			//VR
			if (IsVRMode)
			{
				if (OnUpdate != null)
				{
					OnUpdate();
				}
				
				var screenPoint = -Vector3.one;
				if (OnMouseHover != null)
				{
					screenPoint = Camera.WorldToScreenPoint(PenTransform.position);
					OnMouseHover(screenPoint);
				}

#if VR_ENABLED
				//vr input example
				if (leftHandedControllers.Count > 0 && leftHandedControllers[0].TryGetFeatureValue(CommonUsages.triggerButton, out var triggerValue) && triggerValue)
				{
				}
#endif
				//next line needs to be changed for VR input
				if (Input.GetMouseButtonDown(0))
				{
					if (OnMouseDown != null)
					{
						screenPoint = Camera.WorldToScreenPoint(PenTransform.position);
						OnMouseDown(screenPoint);
					}
				}

				//next line needs to be changed for VR input
				if (Input.GetMouseButton(0))
				{
					if (OnMouseButton != null)
					{
						screenPoint = Camera.WorldToScreenPoint(PenTransform.position);
						OnMouseButton(screenPoint);
					}
				}

				//next line needs to be changed for VR input
				if (Input.GetMouseButtonUp(0))
				{
					if (OnMouseUp != null)
					{
						screenPoint = Camera.WorldToScreenPoint(PenTransform.position);
						OnMouseUp(screenPoint);
					}
				}
			}
			else
			{
				//Touch / Mouse
				if (Input.touchSupported && Input.touchCount > 0 && !_isWebgl)
				{
					foreach (var touch in Input.touches)
					{
						if (OnUpdate != null)
						{
							OnUpdate();
						}

						var pressure = 1f;
						if (Settings.Instance.PressureEnabled)
						{
							pressure = touch.pressure;
						}
			
						if (touch.phase == TouchPhase.Began && _fingerId == -1)
						{
							_fingerId = touch.fingerId;
							if (OnMouseDown != null)
							{
								OnMouseDown(touch.position, pressure);
							}
						}

						if (touch.fingerId == _fingerId)
						{
							if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
							{
								if (OnMouseButton != null)
								{
									OnMouseButton(touch.position, pressure);
								}
							}

							if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
							{
								_fingerId = -1;
								if (OnMouseUp != null)
								{
									OnMouseUp(touch.position);
								}
							}
						}
					}
				}
				else
				{
					if (OnUpdate != null)
					{
						OnUpdate();
					}
					
					if (OnMouseHover != null)
					{
						OnMouseHover(Input.mousePosition);
					}
					
					if (Input.GetMouseButtonDown(0))
					{
						if (OnMouseDown != null)
						{
							OnMouseDown(Input.mousePosition);
						}
					}

					if (Input.GetMouseButton(0))
					{
						if (OnMouseButton != null)
						{
							OnMouseButton(Input.mousePosition);
						}
					}

					if (Input.GetMouseButtonUp(0))
					{
						if (OnMouseUp != null)
						{
							OnMouseUp(Input.mousePosition);
						}
					}
				}
			}
		}
	}
}
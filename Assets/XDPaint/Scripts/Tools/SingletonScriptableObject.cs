using UnityEngine;

namespace XDPaint.Tools
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
		private static T _instance;
		private const string SettingsFilename = "XDPaintSettings";

		public static T Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = Resources.Load<T>(SettingsFilename);
				}
				return _instance;
			}
		}
	}
}
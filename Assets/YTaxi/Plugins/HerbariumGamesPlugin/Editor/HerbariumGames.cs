#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YTaxi.Plugins.HerbariumGamesPlugin.Editor
{
    public class Screenshoter
    {
        [MenuItem("HerbariumGames/Screenshoter/TakeScreenshot %&S")]
        private static void NewMenuOption()
        {
            if (!Directory.Exists(Application.dataPath + "/Screenshots"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Screenshots");
                if (File.Exists(Directory.GetParent(Application.dataPath) + "/.gitignore"))
                {
                    var a = File.ReadAllText(Directory.GetParent(Application.dataPath) + "/.gitignore");
                    if (!a.Contains("[Ss]creenshots/"))
                    {
                        File.WriteAllText(Directory.GetParent(Application.dataPath) + "/.gitignore",
                            a + "\n[Ss]creenshots/\n");
                    }
                }
                   
            }

            if (File.Exists(Application.dataPath + "/Screenshots/Screenshot.png"))
            {
                int i = 1;
                while (File.Exists(Application.dataPath + $"/Screenshots/Screenshot{i}.png"))
                {
                    i++;
                }
                ScreenCapture.CaptureScreenshot(Application.dataPath + $"/Screenshots/Screenshot{i}.png"); 
            }
            else
            {
                ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshots/Screenshot.png"); 
            }
            
        }
    }

    public class PlayerPrefsUtils
    {
        [MenuItem("HerbariumGames/Clear PlayerPrefs %&D")]
        private static void NewMenuOption()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif
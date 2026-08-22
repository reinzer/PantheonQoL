using BepInEx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using GlobalEnums;
using System.Collections;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;

namespace PantheonQoL
{
    [BepInPlugin("bepinex.plugin.test", "PantheonQoL", "0.1.1.0")]
    public partial class PantheonQoL : BaseUnityPlugin
    {
		public class BindingsData{
			public bool boundNail = false;
			public bool boundSoul = false;
			public bool boundCharms = false;
			public bool boundShell = false;
		};
        public static PantheonQoL instance;
		public static bool enabled = true;
		public static bool scenePreloadEnabled = false;
		public static bool skipTransitionAnimation = true;
		public static bool disFastDeathForAll = false; //disable for all
		public static bool disFastDeathForPermaThreat = true; //disable only for bosses with permanent threats on arena (like no floor)
		public static float origVengeflyKingsRoarFps = -2f;
		public static float origKnightWarpInFps = -2f;
		public static bool isRadiantRun = false;
		public static bool doSkipLoreScenes = false;
		public static bool doSkipSpaScenes = false;
        public static string currentSceneName;
        public static Scene currentScene;
        public static ScenePreload.SceneInfo currentSceneInfo;
        string pathToModData = BepInEx.Paths.ConfigPath + "/" +"PantheonQoL.dat";
        public static object obj;
		public static bool ggBossSceneMutex = true;
		public static BindingsData previousBindings = new BindingsData();
        private static string[] assetBundleNames =
        {
			
        };
        public static List<AssetBundle> assetBundles = new List<AssetBundle>();
        public static BepInEx.Logging.ManualLogSource Log;
        public AssetBundle LoadBundle(string bundleName)
        {
            AssetBundle bundle;
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {
                string name = Path.GetExtension(res).Substring(1);
                Logger.LogInfo(name);
                if (name != bundleName) continue;

                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    if (s == null) continue;
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();
                    Logger.LogInfo("Loading bundle " + bundleName);
                    bundle = AssetBundle.LoadFromMemory(buffer);
                }
                return bundle;
            }
            return null;
        }
//        public void LoadModData()
//        {
//            if (File.Exists(pathToModData))
//            {
//                var json = File.ReadAllText(pathToModData);
//                PlayerDataMod.instance = JsonConvert.DeserializeObject<PlayerDataMod>(json);
//            }
//            else
//            {
//                PlayerDataMod.instance = new PlayerDataMod();
//                SaveModData();
//            }
//        }
//        public void SaveModData()
//        {
//            var jsonString = JsonConvert.SerializeObject(PlayerDataMod.instance, Formatting.Indented);
//            File.WriteAllText(pathToModData, jsonString);
//        }
        private void Awake()
        {
            instance = this;
            Log = this.Logger;

            var harmony = new Harmony("com.PantheonQoL");
            Harmony.CreateAndPatchAll(typeof(PantheonQoL));

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => currentScene = scene;

            foreach(string name in assetBundleNames)
            {
                var bundle = LoadBundle(name);
                assetBundles.Add(bundle);
            }

			this.StartCoroutine(ScenePreload.Start());

            Logger.LogInfo($"Plugin is loaded!");
        }

        void Update()
        {
			if(Keyboard.current.leftBracketKey.wasPressedThisFrame && !HeroController.instance.cState.hazardDeath){
				GameManager.UnsafeInstance.BeginSceneTransition(new GameManager.SceneLoadInfo
				{
					SceneName = PlayerData.instance.dreamReturnScene,
					EntryGateName = "door_dreamReturn",
					EntryDelay = 0f,
					Visualization = GameManager.SceneLoadVisualizations.GodsAndGlory,
					PreventCameraFadeOut = true,
					WaitForSceneTransitionCameraFade = false,
					AlwaysUnloadUnusedAssets = false
				});
			}
        }
    }
}

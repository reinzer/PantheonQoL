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
using static UnityEngine.SceneManagement.SceneManager;

namespace PantheonQoL;

public static class ScenePreload{
	public class SceneInfo{
		public Scene scene;
		public string sceneName;
		public AsyncOperation op;
		public bool isReadyForActivate = false;
		public bool isActivated = false;
		public bool isInUse = false;
		public bool isBossSceneControllerAwakePatched = false;
		public List<GameObject> disabledObjects = new List<GameObject>();

		public SceneInfo(AsyncOperation op, Scene scene){
			this.op = op;
			this.scene = scene;
			this.sceneName = scene.name;
		}
		
		public void Destroy(){
			PantheonQoL.instance.StartCoroutine(IDestroy());
		}
		public IEnumerator IDestroy(){
			while(!isReadyForActivate) yield return null;
			if(scene != null && !isActivated) UnloadSceneAsync(scene);
		}
	}
	public static bool scenePreloaderWorking = false;
	public static List<SceneInfo> scenes = new List<SceneInfo>();
	public static SceneInfo returnScene = null;
	public static SceneInfo firstBossScene = null;
	public static SceneInfo nextBossScene = null;

	public static SceneInfo FindScene(string sceneName){
//		foreach(var scene in scenes){
//			if(scene.scene.name == sceneName) return scene;
//		}
		if(returnScene != null && returnScene.sceneName == sceneName) return returnScene;
		if(firstBossScene != null && firstBossScene.sceneName == sceneName) return firstBossScene;
		if(nextBossScene != null && nextBossScene.sceneName == sceneName) return nextBossScene;
		return null;
	}

	public static void Reset(){
		returnScene = null;
		firstBossScene = null;
		nextBossScene = null;
		foreach(var scene in scenes){
			if(scene.scene != null) UnloadSceneAsync(scene.scene);
		}
		scenes = new List<SceneInfo>();
	}

	public static void DoPreload(SceneInfo sceneInfo){
		PantheonQoL.instance.StartCoroutine(IDoPreload(sceneInfo));
	}
	public static IEnumerator IDoPreload(SceneInfo sceneInfo){
		sceneInfo.op.allowSceneActivation = true;
		yield return sceneInfo.op;
		foreach(var obj in sceneInfo.scene.GetRootGameObjects()){
			if(obj.activeInHierarchy){
				obj.SetActive(false);
				sceneInfo.disabledObjects.Add(obj);
			}
		}
		sceneInfo.isReadyForActivate = true;
	}
	public static void UpdateScenes(){
		if(returnScene != null && (returnScene.scene == null || returnScene.isInUse)) returnScene = null;
		if(firstBossScene != null && (firstBossScene.scene == null || firstBossScene.isInUse)) firstBossScene = null;
		if(nextBossScene != null && (nextBossScene.scene == null || nextBossScene.isInUse)) nextBossScene = null;
	}

	public static IEnumerator Start(){
		if(scenePreloaderWorking) yield break;
		scenePreloaderWorking = true;

		var currentSequence = typeof(BossSequenceController).GetField("currentSequence", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		var bossScenes = typeof(BossSequence).GetField("bossScenes", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
		var bossSceneName = typeof(BossScene).GetField("sceneName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

		while(PantheonQoL.scenePreloadEnabled){
			if(BossSceneController.Instance != null && !BossSequenceController.IsInSequence){
//				yield return null;
				while(BossSceneController.Instance != null){
					UpdateScenes();
					if(returnScene == null || returnScene.sceneName != PlayerData.instance.dreamReturnScene){
						if(returnScene != null) returnScene.Destroy();
						PantheonQoL.Log.LogInfo("1111111111111111111111111111");
						var op = LoadSceneAsync(PlayerData.instance.dreamReturnScene, LoadSceneMode.Additive);
						op.allowSceneActivation = false;
						var scene = GetSceneAt(sceneCount - 1);
						var sceneInfo = new SceneInfo(op, scene);
						returnScene = sceneInfo;
//						scenes.Add(sceneInfo);
						DoPreload(sceneInfo);
					}
					if(firstBossScene == null || firstBossScene.sceneName != BossSceneController.Instance.gameObject.scene.name){
						if(firstBossScene != null) firstBossScene.Destroy();
						PantheonQoL.Log.LogInfo("222222222222222222222222222");
						var op = LoadSceneAsync(BossSceneController.Instance.gameObject.scene.name, LoadSceneMode.Additive);
						op.allowSceneActivation = false;
						var scene = GetSceneAt(sceneCount - 1);
						var sceneInfo = new SceneInfo(op, scene);
						firstBossScene = sceneInfo;
//						scenes.Add(sceneInfo);
						DoPreload(sceneInfo);
					}

					yield return null;
				}
			}
			if(BossSequenceController.IsInSequence){
				string dreamReturnScene = PlayerData.instance.dreamReturnScene;
//				yield return null;
				while(BossSequenceController.IsInSequence){
					UpdateScenes();
					var currentSequenceValue = (BossSequence)currentSequence.GetValue(null);
					var bossScenesValue = (BossScene[])bossScenes.GetValue(currentSequenceValue);
					string firstBossSceneName = null;
					string nextBossSceneName = null;
					if(BossSequenceController.BossCount > 0) firstBossSceneName = (string)bossSceneName.GetValue(bossScenesValue[0]);
					if(BossSequenceController.BossIndex + 1 < BossSequenceController.BossCount) nextBossSceneName = (string)bossSceneName.GetValue(bossScenesValue[BossSequenceController.BossIndex + 1]);

					if(returnScene == null || returnScene.sceneName != PlayerData.instance.dreamReturnScene){
						if(returnScene != null) returnScene.Destroy();
						PantheonQoL.Log.LogInfo("4444444444444444444444444444");
						var op = LoadSceneAsync(PlayerData.instance.dreamReturnScene, LoadSceneMode.Additive);
						op.allowSceneActivation = false;
						var scene = GetSceneAt(sceneCount - 1);
						var sceneInfo = new SceneInfo(op, scene);
						returnScene = sceneInfo;
//						scenes.Add(sceneInfo);
						DoPreload(sceneInfo);
					}
					if(firstBossSceneName != null && (firstBossScene == null || firstBossScene.sceneName != firstBossSceneName)){
						if(firstBossScene != null) firstBossScene.Destroy();
						PantheonQoL.Log.LogInfo("55555555555555555555555555555555555");
						var op = LoadSceneAsync(firstBossSceneName, LoadSceneMode.Additive);
						op.allowSceneActivation = false;
						var scene = GetSceneAt(sceneCount - 1);
						var sceneInfo = new SceneInfo(op, scene);
						firstBossScene = sceneInfo;
//						scenes.Add(sceneInfo);
						DoPreload(sceneInfo);
					}
					if(nextBossSceneName != null && (nextBossScene == null || nextBossScene.sceneName != nextBossSceneName)){
						if(nextBossScene != null) nextBossScene.Destroy();
						PantheonQoL.Log.LogInfo("666666666666666666666666666666666");
						var op = LoadSceneAsync(nextBossSceneName, LoadSceneMode.Additive);
						op.allowSceneActivation = false;
						var scene = GetSceneAt(sceneCount - 1);
						var sceneInfo = new SceneInfo(op, scene);
						nextBossScene = sceneInfo;
//						scenes.Add(sceneInfo);
						DoPreload(sceneInfo);
					}

					yield return null;
				}
			}

			yield return null;
		}

//		Reset();

		scenePreloaderWorking = false;
	}
}

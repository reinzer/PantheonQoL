using BepInEx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Reflection;
using HarmonyLib;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine.EventSystems;
using System.Diagnostics;
using GlobalEnums;

namespace PantheonQoL
{
    public partial class PantheonQoL : BaseUnityPlugin
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), "PlayerDead")]
        public static IEnumerator PlayerDead_Prefix(IEnumerator __result, GameManager __instance, float waitTime)
        {
//			while(__result.MoveNext()) yield return __result.Current;
			__instance.cameraCtrl.FreezeInPlace(freezeTargetAlso: true);
			__instance.NoLongerFirstGame();
			__instance.ResetSemiPersistentItems();
			bool finishedSaving = false;
			__instance.GetType().GetMethod("SaveGame", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{ __instance.profileID, (bool a) =>
			{
				finishedSaving = true;
			}
        });
			__instance.cameraCtrl.FadeOut(GlobalEnums.CameraFadeType.HERO_DEATH);
			while(!finishedSaving) yield return null;
			if(__instance.playerData.permadeathMode == 0) __instance.ReadyForRespawn(isFirstLevelForPlayer: false);
			else if (__instance.playerData.permadeathMode == 2) __instance.LoadScene("PermaDeath");
		}

//        [HarmonyPostfix]
//        [HarmonyPatch(typeof(HeroController), "Respawn")]
//        public static IEnumerator Respawn_Postfix(IEnumerator __result, HeroController __instance)
//        {
//			__instance.playerData = PlayerData.instance;
//			__instance.playerData.disablePause = true;
//			__instance.gameObject.layer = 9;
//			var renderer = (MeshRenderer)__instance.GetType().GetField("renderer", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
//			renderer.enabled = true;
////			__instance.rb2d.isKinematic = false;
//			var rb2d = (Rigidbody2D)__instance.GetType().GetField("rb2d", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
//			rb2d.GetType().GetProperty("isKinematic", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(rb2d, false);
//			__instance.cState.dead = false;
//			__instance.cState.onGround = true;
//			__instance.cState.hazardDeath = false;
//			__instance.cState.recoiling = false;
//			__instance.GetType().GetField("enteringVertically", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);
//			__instance.GetType().GetField("airDashed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);
//			__instance.GetType().GetField("doubleJumped", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);
//			__instance.CharmUpdate();
//			__instance.MaxHealth();
//			__instance.ClearMP();
//			__instance.GetType().GetMethod("ResetMotion", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{});
//			__instance.ResetHardLandingTimer();
//			__instance.GetType().GetMethod("ResetAttacks", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{});
//			__instance.GetType().GetMethod("ResetInput", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{});
//			__instance.CharmUpdate();
//			Transform spawnPoint = __instance.LocateSpawnPoint();
//			if (spawnPoint != null)
//			{
//				__instance.transform.SetPosition2D(__instance.FindGroundPoint(spawnPoint.transform.position));
//				PlayMakerFSM component = spawnPoint.GetComponent<PlayMakerFSM>();
//				if (component != null)
//				{
//					FSMUtility.GetVector3(component, "Adjust Vector");
//				}
//				else if ((bool)__instance.GetType().GetField("verboseMode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
//				{
//					Debug.Log("Could not find Bench Control FSM on respawn point. Ignoring Adjustment offset.");
//				}
//			}
//			else
//			{
//				Debug.LogError("Couldn't find the respawn point named " + __instance.playerData.respawnMarkerName + " within objects tagged with RespawnPoint");
//			}
//			if ((bool)__instance.GetType().GetField("verboseMode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance))
//			{
//				Debug.Log("HC Respawn Type: " + __instance.playerData.respawnType);
//			}
//			GameCameras.instance.cameraFadeFSM.SendEvent("RESPAWN");
//			if (__instance.playerData.respawnType == 1)
//			{
//				__instance.AffectedByGravity(gravityApplies: false);
//				PlayMakerFSM benchFSM = FSMUtility.LocateFSM(spawnPoint.gameObject, "Bench Control");
//				if (benchFSM == null)
//				{
//					Debug.LogError("HeroCtrl: Could not find Bench Control FSM on this spawn point, respawn type is set to Bench");
//					yield break;
//				}
//				benchFSM.FsmVariables.GetFsmBool("RespawnResting").Value = true;
//				yield return new WaitForEndOfFrame();
////				__instance.SendHeroInPosition(forceDirect: false);
//				__instance.GetType().GetMethod("SendHeroInPosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{});
////				__instance.proxyFSM.SendEvent("HeroCtrl-Respawned");
//				var proxyFSM = (PlayMakerFSM)__instance.GetType().GetField("proxyFSM", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
//				proxyFSM.SendEvent("HeroCtrl-Respawned");
////				__instance.FinishedEnteringScene();
//				__instance.GetType().GetMethod("FinishedEnteringScene", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{});
//				benchFSM.SendEvent("RESPAWN");
//			}
//			else
//			{
//				yield return new WaitForEndOfFrame();
//				__instance.IgnoreInput();
//				RespawnMarker component2 = spawnPoint.GetComponent<RespawnMarker>();
//				if ((bool)component2)
//				{
//					if (component2.respawnFacingRight)
//					{
//						__instance.FaceRight();
//					}
//					else
//					{
//						__instance.FaceLeft();
//					}
//				}
//				else
//				{
//					Debug.LogError("Spawn point does not contain a RespawnMarker");
//				}
////				__instance.SendHeroInPosition(forceDirect: false);
//				__instance.GetType().GetMethod("SendHeroInPosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{true});
//				if (GameManager.instance.GetSceneNameString() != "GG_Atrium")
//				{
////					var animCtrl = (HeroAnimationController)__instance.GetType().GetField("animCtrl", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
////					float clipDuration = animCtrl.GetClipDuration("Wake Up Ground");
////					animCtrl.PlayClip("Wake Up Ground");
////					__instance.StopAnimationControl();
////					__instance.controlReqlinquished = true;
//					__instance.StartAnimationControl();
//					__instance.controlReqlinquished = false;
//					Log.LogInfo("YWOHDLSHDJKLHFOISHOSIDHFIOSHFOISHIFHSODOHF:S");
//				}
//				__instance.proxyFSM.SendEvent("HeroCtrl-Respawned");
////				__instance.FinishedEnteringScene();
//				__instance.GetType().GetMethod("FinishedEnteringScene", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[]{});
//			}
//			__instance.playerData.disablePause = false;
//			__instance.playerData.isInvincible = false;
//		}
//

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayMakerFSM), "Awake")]
        private static void PlayMakerPatch_Postfix(PlayMakerFSM __instance)
        {
			if(!PantheonQoL.enabled) return;

            var orig = __instance.gameObject;
			string objName = orig.gameObject.name;
			string fsmName = __instance.FsmName;
			string sceneName = orig.scene.name;
            int objNameHash = objName.GetHashCode();
            int fsmNameHash = fsmName.GetHashCode();
            int sceneNameHash = sceneName.GetHashCode();

            Log.LogInfo(__instance.gameObject.name + "     " + __instance.FsmName + "        " + __instance.gameObject.scene.name);

            for(int i = 0; i < PatchedFsm.globalPatchedFsms.Length; i++)
            {
                if(objNameHash == PatchedFsm.globalPatchedFsms[i].objNameHash && fsmNameHash == PatchedFsm.globalPatchedFsms[i].fsmNameHash){
					PatchedFsm.globalPatchedFsms[i].method(__instance.Fsm);
					break;
				}
            }

            int index1 = 0;
            for(; index1 < PatchedFsm.patchedFsms.Length; index1++)
            {
                if(sceneNameHash == PatchedFsm.patchedFsms[index1].sceneNameHash) break;
                if(index1 == PatchedFsm.patchedFsms.Length - 1) return;
            }

            var patchedFsm = PatchedFsm.patchedFsms[index1];

            foreach(var item in patchedFsm.fsms)
            {
				if(!item.isSingleFsm && objName.Contains(item.objName) && fsmNameHash == item.fsmNameHash){
                    Log.LogInfo(__instance.FsmName + "YAAAAAAAAAAAAAAAAAY" + objName);
                    item.method(__instance.Fsm);
				}
            }
            foreach(var item in patchedFsm.fsms)
            {
                if(objNameHash == item.objNameHash && fsmNameHash == item.fsmNameHash)
                {
                    Log.LogInfo(__instance.FsmName + "YAAAAAAAAAAAAAAAAAYOYO" + objName);
                    item.method(__instance.Fsm);
                    return;
                }
			}

//            foreach(var item in patchedFsm.fsms)
//            {
//                if(objNameHash == item.objNameHash && fsmNameHash == item.fsmNameHash)
//                {
//                    Log.LogInfo(__instance.FsmName + "YAAAAAAAAAAAAAAAAAY");
//                    item.method(__instance.Fsm);
//                    return;
//                }
//			}
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BossDoorChallengeUI), "ShowSequence")]
        private static IEnumerator BossDoorChallengeUIShowSequence_Postfix(IEnumerator __result, BossDoorChallengeUI __instance)
        {
			var group = (CanvasGroup)__instance.GetType().GetField("group", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			var animator = (Animator)__instance.GetType().GetField("animator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			animator.speed = float.MaxValue;
			group.interactable = false;
			EventSystem.current.SetSelectedGameObject(null);
			yield return null;
			if ((bool)animator)
			{
				animator.Play("Open");
				yield return null;
				yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			}
			group.interactable = true;
			var buttons = (BossDoorChallengeUIBindingButton[])__instance.GetType().GetField("buttons", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
			if (buttons.Length != 0)
			{
				for(int i = 0; i < buttons.Length; i++){
					if(buttons[i].gameObject.name == "NailButton" && previousBindings.boundNail) buttons[i].OnSubmit(new BaseEventData(EventSystem.current));
					if(buttons[i].gameObject.name == "HeartButton" && previousBindings.boundShell) buttons[i].OnSubmit(new BaseEventData(EventSystem.current));
					if(buttons[i].gameObject.name == "CharmsButton" && previousBindings.boundCharms) buttons[i].OnSubmit(new BaseEventData(EventSystem.current));
					if(buttons[i].gameObject.name == "SoulButton" && previousBindings.boundSoul) buttons[i].OnSubmit(new BaseEventData(EventSystem.current));
				}
				EventSystem.current.SetSelectedGameObject(buttons[0].gameObject.transform.parent.parent.Find("BeginButton").gameObject);
			}
			InputHandler.Instance.StartUIInput();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BossDoorChallengeUIBindingButton), "OnSubmit")]
        private static void BossDoorChallengeUIBindingButton_Postfix(BossDoorChallengeUIBindingButton __instance, BaseEventData eventData)
        {
			if(__instance.gameObject.name == "NailButton") previousBindings.boundNail = __instance.Selected;
			if(__instance.gameObject.name == "HeartButton") previousBindings.boundShell = __instance.Selected;
			if(__instance.gameObject.name == "CharmsButton") previousBindings.boundCharms = __instance.Selected;
			if(__instance.gameObject.name == "SoulButton") previousBindings.boundSoul = __instance.Selected;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartManager), "Start")]
        private static IEnumerator Start_Manager_Start_Postfix(IEnumerator __result, StartManager __instance)
        {
			__instance.startManagerAnimator.speed = float.MaxValue;
			PantheonQoL.Log.LogInfo("START ANIM IS FAST NOW, finally...");

			while(__result.MoveNext()) yield return __result.Current;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BossSceneController), "Awake")]
        private static void BossSceneController_Awake_Postfix(BossSceneController __instance)
        {
			var sceneName = __instance.gameObject.scene.name;

			if(PantheonQoL.enabled && (sceneName == "GG_Spa" || sceneName == "GG_Unn" || sceneName == "GG_Wyrm" || sceneName == "GG_Engine" || sceneName == "GG_Engine_Root")){
				var doTransition = __instance.GetType().GetField("doTransition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var doTransitionIn = __instance.GetType().GetField("doTransitionIn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var doTransitionOut = __instance.GetType().GetField("doTransitionOut", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				doTransition.SetValue(__instance, false);
				doTransitionIn.SetValue(__instance, false);
				doTransitionOut.SetValue(__instance, false);
	//			__instance.bossesDeadWaitTime = 0f;
			}

			var arrivalAnimator = BossSceneController.Instance.transform.Find("Dream Entry/Knight Dream Arrival").gameObject.GetComponent<tk2dSpriteAnimator>();
			var warpInClip = arrivalAnimator.GetClipByName("Warp In");
			if(PantheonQoL.origKnightWarpInFps == -2f) PantheonQoL.origKnightWarpInFps = warpInClip.fps;
			else warpInClip.fps = PantheonQoL.origKnightWarpInFps;

//			while(__result.MoveNext()) yield return __result.Current;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BossSceneController), "EndBossScene")]
        private static bool BossSceneController_EndBossScene_Prefix(BossSceneController __instance)
        {
			if(PantheonQoL.ggBossSceneMutex) return false;
			return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BossSequenceController), "SetupNewSequence")]
        private static void BossSequenceController_SetupNewSequence_Postfix()
        {
			if(!PantheonQoL.enabled || PantheonQoL.doSkipLoreScenes && PantheonQoL.doSkipSpaScenes) return;

			var currentSequence = (BossSequence)typeof(BossSequenceController).GetField("currentSequence", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			var bossScenes = currentSequence.GetType().GetField("bossScenes", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var bossScenesValue = (BossScene[])bossScenes.GetValue(currentSequence);

			for(int i = 0; i < bossScenesValue.Length; i++){
				if(PantheonQoL.doSkipLoreScenes && bossScenesValue[i].name == "Engine Boss Scene"){
					var tmp = bossScenesValue.ToList();
					tmp.RemoveAt(i);
					bossScenesValue = tmp.ToArray();
					bossScenes.SetValue(currentSequence, tmp.ToArray());
					i--;
				}
				if(PantheonQoL.doSkipLoreScenes && bossScenesValue[i].name == "Wyrm Engine Boss Scene"){
					var tmp = bossScenesValue.ToList();
					tmp.RemoveAt(i);
					bossScenesValue = tmp.ToArray();
					bossScenes.SetValue(currentSequence, tmp.ToArray());
					i--;
				}
				if(PantheonQoL.doSkipLoreScenes && PlayerData.instance.hasAcidArmour && bossScenesValue[i].name == "Unn Engine Boss Scene"){
					var tmp = bossScenesValue.ToList();
					tmp.RemoveAt(i);
					bossScenesValue = tmp.ToArray();
					bossScenes.SetValue(currentSequence, tmp.ToArray());
					i--;
				}
				if(PantheonQoL.doSkipSpaScenes && PantheonQoL.isRadiantRun && bossScenesValue[i].name == "Spa Boss Scene"){
					var tmp = bossScenesValue.ToList();
					tmp.RemoveAt(i);
					bossScenesValue = tmp.ToArray();
					bossScenes.SetValue(currentSequence, tmp.ToArray());
					i--;
				}
			}
			return;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneLoad), "Begin")]
        private static bool SceneLoad_Begin_Prefix(SceneLoad __instance)
        {
			PantheonQoL.ggBossSceneMutex = false;
			return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(HeroController), "RegainControl")]
        private static void HeroController_RegainControl_Postfix(HeroController __instance)
        {
			PantheonQoL.Log.LogInfo("------------------------------------Regain Control---------------------------------");
			var stackTrace = new StackTrace();
			foreach(var frame in stackTrace.GetFrames()){
				var method = frame.GetMethod();
				PantheonQoL.Log.LogInfo(method.DeclaringType?.FullName);
			}
			PantheonQoL.Log.LogInfo("||||||||||||||||||||||||||||||||||||||||Regain Control||||||||||||||||||||||||||||||||||||||||||||||");
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneLoad), "Begin")]
        private static bool Prefix(SceneLoad __instance)
        {
            var runner = __instance.GetType().GetField("runner", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            ((MonoBehaviour)runner.GetValue(__instance)).StartCoroutine(BeginRoutine_Patched(__instance));
            return false;
        }
        private static IEnumerator BeginRoutine_Patched(SceneLoad __instance)
        {
            FieldInfo operationHandle = __instance.GetType().GetField("operationHandle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo _tempOps = typeof(SceneLoad).GetField("_tempOps", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var runner = __instance.GetType().GetField("runner", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var RecordBeginTime = __instance.GetType().GetMethod("RecordBeginTime", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var RecordEndTime = __instance.GetType().GetMethod("RecordEndTime", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var targetSceneName = (string)__instance.GetType().GetField("targetSceneName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            SceneAdditiveLoadConditional.loadInSequence = true;
			yield return ((MonoBehaviour)runner.GetValue(__instance)).StartCoroutine(ScenePreloader.FinishPendingOperations());
			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.FetchBlocked});
			while(!__instance.IsFetchAllowed) yield return null;
			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.FetchBlocked});
			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.Fetch});

			AsyncOperation loadOperation;
			Scene scene;

			var sceneInfo = ScenePreload.FindScene(targetSceneName);
			if(sceneInfo == null){
				loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
				loadOperation.allowSceneActivation = false;
				scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(UnityEngine.SceneManagement.SceneManager.sceneCount - 1);
				while(loadOperation.progress < 0.9f) yield return null;
			}
			else{
				sceneInfo.isInUse = true;
				loadOperation = sceneInfo.op;
				scene = sceneInfo.scene;
				currentSceneInfo = sceneInfo;
				while(!sceneInfo.isReadyForActivate) yield return null;
//				ScenePreload.scenes.Remove(sceneInfo);
			}

			PantheonQoL.currentScene = scene;
			PantheonQoL.currentSceneName = scene.name;

			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.Fetch});
            var FetchComplete = (SceneLoad.FetchCompleteDelegate)__instance.GetType().GetField("FetchComplete", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (FetchComplete != null)
            {
                try
                {
                    FetchComplete();//this.FetchComplete();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.FetchComplete. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex);
                }
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD4.1");
			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.ActivationBlocked});
            while (!__instance.IsActivationAllowed)
            {
                yield return null;
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD4.1.1");
			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.ActivationBlocked});
			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.Activation});

            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD4.2");

            var WillActivate = (SceneLoad.WillActivateDelegate)__instance.GetType().GetField("WillActivate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (WillActivate != null)
            {
                try
                {
                    WillActivate();
                }
                catch (Exception ex2)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.WillActivate. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex2);
                }
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD5");
            
			loadOperation.allowSceneActivation = true;
			yield return loadOperation;

			if(sceneInfo != null){
				foreach(var obj in sceneInfo.disabledObjects){
					obj.SetActive(true);
				}
				sceneInfo.isActivated = true;
			}

			UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);

            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD5.1");

			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.Activation});

            var ActivationComplete = (SceneLoad.ActivationCompleteDelegate)__instance.GetType().GetField("ActivationComplete", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (ActivationComplete != null)
            {
                try
                {
                    ActivationComplete();
                }
                catch (Exception ex3)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.ActivationComplete. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex3);
                }
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD6");
			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.UnloadUnusedAssets});
            if (__instance.IsUnloadAssetsRequired)
            {
				yield return Resources.UnloadUnusedAssets();
            }

			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.UnloadUnusedAssets});
			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.GarbageCollect});

            if (__instance.IsGarbageCollectRequired)
            {
				GCManager.Collect();
            }
			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.GarbageCollect});

            var Complete = (SceneLoad.CompleteDelegate)__instance.GetType().GetField("Complete", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (Complete != null)
            {
                try
                {
                    Complete();
                }
                catch (Exception ex4)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.Complete. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex4);
                }
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD8");

			RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.StartCall});
			yield return null;
			RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.StartCall});

            var StartCalled = (SceneLoad.StartCalledDelegate)__instance.GetType().GetField("StartCalled", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (StartCalled != null)
            {
                try
                {
                    StartCalled();
                }
                catch (Exception ex5)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.StartCalled. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex5);
                }
            }
            if (SceneAdditiveLoadConditional.ShouldLoadBoss)
            {
				RecordBeginTime.Invoke(__instance, new object[]{SceneLoad.Phases.LoadBoss});
                yield return ((MonoBehaviour)runner.GetValue(__instance)).StartCoroutine(SceneAdditiveLoadConditional.LoadAll());
				RecordEndTime.Invoke(__instance, new object[]{SceneLoad.Phases.LoadBoss});
                try
                {
					var BossLoaded = (SceneLoad.BossLoadCompleteDelegate)__instance.GetType().GetField("BossLoaded", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                    if (BossLoaded != null)
                    {
                        BossLoaded();
                    }
                    if ((bool)GameManager.instance)
                    {
                        GameManager.instance.LoadedBoss();
                    }
                }
                catch (Exception ex6)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.BossLoaded. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex6);
                }
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD9");
            try
            {
                ScenePreloader.Cleanup();
            }
            catch (Exception ex7)
            {
                UnityEngine.Debug.LogError("Exception in responders to ScenePreloader.Cleanup. Attempting to continue load regardless.");
                UnityEngine.Debug.LogException(ex7);
            }
            Log.LogInfo("PAAAAAAAAATTTTTCCCCHHHEEEDDD10");

			var Finish = (SceneLoad.FinishDelegate)__instance.GetType().GetField("Finish", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (Finish != null)
            {
                try
                {
                    Finish();
                }
                catch (Exception ex8)
                {
                    UnityEngine.Debug.LogError("Exception in responders to SceneLoad.Finish. Attempting to continue load regardless.");
                    UnityEngine.Debug.LogException(ex8);
                }
            }
        }
		[HarmonyPrefix]
		[HarmonyPatch(typeof(BossSceneController), "Awake")]
		private static bool BossSceneController_Awake_Prefix(BossSceneController __instance)
		{
			var sceneInfo = ScenePreload.FindScene(__instance.gameObject.scene.name);
			if(sceneInfo == null) return true;
			else if(sceneInfo.isBossSceneControllerAwakePatched) return true;

			IEnumerator enumerator(){
				while(!sceneInfo.isActivated) yield return null;

				var awake = __instance.GetType().GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				awake.Invoke(__instance, new object[]{});
			}
			PantheonQoL.instance.StartCoroutine(enumerator());

			sceneInfo.isBossSceneControllerAwakePatched = true;

			return false;
		}

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), "BeginSceneTransitionRoutine")]
        private static IEnumerator GameManager_BeginSceneTransitionRoutine_Postfix(IEnumerator __result, GameManager __instance, GameManager.SceneLoadInfo info)
        {
			var sceneLoad = __instance.GetType().GetField("sceneLoad", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var IsInSceneTransition = __instance.GetType().GetProperty("IsInSceneTransition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var isLoading = __instance.GetType().GetField("isLoading", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var loadVisualization = __instance.GetType().GetField("loadVisualization", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var targetScene = __instance.GetType().GetField("targetScene", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var tilemapDirty = __instance.GetType().GetField("tilemapDirty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var waitForManualLevelStart = __instance.GetType().GetField("waitForManualLevelStart", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var UnloadingLevel = __instance.GetType().GetField("UnloadingLevel", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var DestroyPersonalPools = __instance.GetType().GetField("DestroyPersonalPools", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var OnFinishedSceneTransition = __instance.GetType().GetField("OnFinishedSceneTransition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var entryDelay = __instance.GetType().GetField("entryDelay", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var sceneLoadsWithoutGarbageCollect = __instance.GetType().GetField("sceneLoadsWithoutGarbageCollect", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var SceneTransitionBegan = __instance.GetType().GetField("SceneTransitionBegan", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			float time = 0f;
			IEnumerator enumerator(){
				while(time > -1f){
					time += Time.deltaTime;
					yield return null;
				}
			}
			PantheonQoL.instance.StartCoroutine(enumerator());

			if (((SceneLoad)sceneLoad.GetValue(__instance)) != null)
			{
				UnityEngine.Debug.LogErrorFormat(__instance, "Cannot scene transition to {0}, while a scene transition is in progress", info.SceneName);
				yield break;
			}
			IsInSceneTransition.SetValue(__instance, true);

			sceneLoad.SetValue(__instance, new SceneLoad(__instance, info.SceneName));
			isLoading.SetValue(__instance, true);
			loadVisualization.SetValue(__instance, info.Visualization);
			if (__instance.hero_ctrl != null)
			{
				if (__instance.hero_ctrl.cState.superDashing)
				{
					__instance.hero_ctrl.exitedSuperDashing = true;
				}
				if (__instance.hero_ctrl.cState.spellQuake)
				{
					__instance.hero_ctrl.exitedQuake = true;
				}
				__instance.hero_ctrl.proxyFSM.SendEvent("HeroCtrl-LeavingScene");
				__instance.hero_ctrl.SetHeroParent(null);
				__instance.hero_ctrl.IsEnteringDream = ((GameManager.SceneLoadVisualizations)loadVisualization.GetValue(__instance)) == GameManager.SceneLoadVisualizations.Dream || ((GameManager.SceneLoadVisualizations)loadVisualization.GetValue(__instance)) == GameManager.SceneLoadVisualizations.GrimmDream || ((GameManager.SceneLoadVisualizations)loadVisualization.GetValue(__instance)) == GameManager.SceneLoadVisualizations.GodsAndGlory;
			}
			if (!info.IsFirstLevelForPlayer)
			{
				__instance.NoLongerFirstGame();
			}
			__instance.SaveLevelState();
			if (__instance.gameState != GameState.CUTSCENE)
			{
				__instance.SetState(GameState.EXITING_LEVEL);
			}
			__instance.entryGateName = info.EntryGateName ?? "";
			targetScene.SetValue(__instance, info.SceneName);
			if (__instance.hero_ctrl != null)
			{
				__instance.hero_ctrl.LeaveScene(info.HeroLeaveDirection);
			}
			if (!info.PreventCameraFadeOut)
			{
				__instance.cameraCtrl.FreezeInPlace(freezeTargetAlso: true);
				__instance.cameraCtrl.FadeOut(CameraFadeType.LEVEL_TRANSITION);
			}
			tilemapDirty.SetValue(__instance, true);
			__instance.startedOnThisScene = false;
			__instance.nextSceneName = info.SceneName;
			waitForManualLevelStart.SetValue(__instance, true);
			if ((UnloadingLevel.GetValue(__instance)) != null)
			{
				((GameManager.UnloadLevel)UnloadingLevel.GetValue(__instance)).Invoke();
			}
			string lastSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			var lastScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			((SceneLoad)sceneLoad.GetValue(__instance)).FetchComplete += delegate
			{
				info.NotifyFetchComplete();
			};
			((SceneLoad)sceneLoad.GetValue(__instance)).WillActivate += delegate
			{
				if ((DestroyPersonalPools.GetValue(__instance)) != null)
				{
					((GameManager.DestroyPooledObjects)DestroyPersonalPools.GetValue(__instance)).Invoke();
				}
				entryDelay.SetValue(__instance, info.EntryDelay);
			};
			((SceneLoad)sceneLoad.GetValue(__instance)).ActivationComplete += delegate
			{
//				foreach(var obj in lastScene.GetRootGameObjects()) obj.SetActive(false);
				var op = UnityEngine.SceneManagement.SceneManager.UnloadScene(lastSceneName);
				__instance.RefreshTilemapInfo(info.SceneName);
				((SceneLoad)sceneLoad.GetValue(__instance)).IsUnloadAssetsRequired = info.AlwaysUnloadUnusedAssets || __instance.IsUnloadAssetsRequired(lastSceneName, info.SceneName);
				bool flag2 = true;
				if (!((SceneLoad)sceneLoad.GetValue(__instance)).IsUnloadAssetsRequired)
				{
					float? beginTime = ((SceneLoad)sceneLoad.GetValue(__instance)).BeginTime;
					if (beginTime.HasValue && Time.realtimeSinceStartup - beginTime.Value > Platform.Current.MaximumLoadDurationForNonCriticalGarbageCollection && ((int)sceneLoadsWithoutGarbageCollect.GetValue(__instance)) < Platform.Current.MaximumSceneTransitionsWithoutNonCriticalGarbageCollection)
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					sceneLoadsWithoutGarbageCollect.SetValue(__instance, 0);
				}
				else
				{
					sceneLoadsWithoutGarbageCollect.SetValue(__instance, ((int)sceneLoadsWithoutGarbageCollect.GetValue(__instance)) + 1);
				}
				((SceneLoad)sceneLoad.GetValue(__instance)).IsGarbageCollectRequired = flag2;
				Platform.Current.FlushSocialEvents();
			};
			((SceneLoad)sceneLoad.GetValue(__instance)).Complete += delegate
			{
				__instance.SetupSceneRefs(refreshTilemapInfo: false);
				__instance.BeginScene();
				if (__instance.gameMap != null)
				{
					__instance.gameMap.GetComponent<GameMap>().LevelReady();
				}
			};
			((SceneLoad)sceneLoad.GetValue(__instance)).Finish += delegate
			{
				sceneLoad.SetValue(__instance, null);
				Platform.Current.SetSceneLoadState(isInProgress: false);
				isLoading.SetValue(__instance, false);
				waitForManualLevelStart.SetValue(__instance, false);
				info.NotifyFinished();
				__instance.OnNextLevelReady();
				IsInSceneTransition.SetValue(__instance, false);
				if (OnFinishedSceneTransition.GetValue(__instance) != null)
				{
					((GameManager.SceneTransitionFinishEvent)OnFinishedSceneTransition.GetValue(__instance)).Invoke();
				}
			};
			if (SceneTransitionBegan.GetValue(__instance) != null)
			{
				try
				{
					((GameManager.SceneTransitionBeganDelegate)SceneTransitionBegan.GetValue(__instance)).Invoke((SceneLoad)sceneLoad.GetValue(__instance));
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogError("Exception in responders to GameManager.SceneTransitionBegan. Attempting to continue load regardless.");
					UnityEngine.Debug.LogException(exception);
				}
			}
			((SceneLoad)sceneLoad.GetValue(__instance)).IsFetchAllowed = !info.forceWaitFetch && (Platform.Current.FetchScenesBeforeFade || info.PreventCameraFadeOut);
			((SceneLoad)sceneLoad.GetValue(__instance)).IsActivationAllowed = false;
			VibrationManager.StopAllVibration();
			Platform.Current.SetSceneLoadState(isInProgress: true);
			Platform.Current.OnScreenFaded();
			((SceneLoad)sceneLoad.GetValue(__instance)).Begin();
			float cameraFadeTimer = 0f;
//			PantheonQoL.Log.LogInfo($"||||||||||||||||||||||||||||||time:{time}");
			while (true)
			{
				bool flag = false;
				cameraFadeTimer -= Time.unscaledDeltaTime;
				if (info.WaitForSceneTransitionCameraFade && cameraFadeTimer > 0f)
				{
					flag = true;
				}
				if (!info.IsReadyToActivate())
				{
					flag = true;
				}
				if (!flag)
				{
					break;
				}
				yield return null;
			}

//			PantheonQoL.Log.LogInfo($"||||||||||||||||||||||||||||||time:{time}");
			Platform.Current.SetSceneLoadState(isInProgress: true, isHighPriority: true);
			((SceneLoad)sceneLoad.GetValue(__instance)).IsFetchAllowed = true;
			((SceneLoad)sceneLoad.GetValue(__instance)).IsActivationAllowed = true;

			time = -2f;
//			while(__result.MoveNext()) yield return __result.Current;
        }
    }
}

//[HarmonyPatch]
//public static class AwakeStartVoidPatcher
//{
//    // 1. Tell Harmony WHICH methods to patch dynamically at startup
//    public static IEnumerable<MethodBase> TargetMethods()
//    {
//        List<MethodBase> methodsToPatch = new List<MethodBase>();
//
//        // Scan loaded assemblies (optimize by specifying your game assembly if needed)
//        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
//        {
//            Type[] types;
//            try
//            {
//                types = assembly.GetTypes();
//            }
//            catch (Exception)
//            {
//                continue; // Skip assemblies that throw reflection errors
//            }
//
//            foreach (Type type in types)
//            {
//                // Only look at concrete classes inheriting from MonoBehaviour
//                if (typeof(MonoBehaviour).IsAssignableFrom(type) && !type.IsAbstract)
//                {
//                    // Check for 'Awake'
//                    MethodInfo awakeMethod = type.GetMethod("Awake", 
//                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
//                    
//                    if (awakeMethod != null && awakeMethod.ReturnType == typeof(void))
//                        methodsToPatch.Add(awakeMethod);
//
//                    // Optional: You can also target 'Start' if needed
//                    MethodInfo startMethod = type.GetMethod("Start", 
//                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
//                    
//                    if (startMethod != null && startMethod.ReturnType == typeof(void))
//                        methodsToPatch.Add(startMethod);
//                }
//            }
//        }
//
//        PantheonQoL.PantheonQoL.Log.LogInfo($"[Patcher] Successfully queued {methodsToPatch.Count} lifecycle methods for patching.");
//        return methodsToPatch;
//    }
//
//    // 2. The Shared Prefix that applies to EVERY method returned by TargetMethods()
//    public static bool Prefix(MethodBase __originalMethod, MonoBehaviour __instance)
//    {
//        // __instance is the MonoBehaviour component running the method
//        // __originalMethod is the specific method info (Awake or Start)
//
//        if (!GlobalAwakeAllowed)
//        {
//            // Gate check failed: block execution of this method entirely
//            // (You can also cache __instance here into a list if you want to invoke it later)
//            return false; 
//        }
//
//        // Gate check passed: let the original method run normally
//        return true;
//    }
//
//    public static bool GlobalAwakeAllowed = false;
//}

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
                    Log.LogInfo(__instance.FsmName + "YAAAAAAAAAAAAAAAAAY");
                    item.method(__instance.Fsm);
				}
            }
            foreach(var item in patchedFsm.fsms)
            {
                if(objNameHash == item.objNameHash && fsmNameHash == item.fsmNameHash)
                {
                    Log.LogInfo(__instance.FsmName + "YAAAAAAAAAAAAAAAAAY");
                    item.method(__instance.Fsm);
                    return;
                }
			}

            foreach(var item in patchedFsm.fsms)
            {
                if(objNameHash == item.objNameHash && fsmNameHash == item.fsmNameHash)
                {
                    Log.LogInfo(__instance.FsmName + "YAAAAAAAAAAAAAAAAAY");
                    item.method(__instance.Fsm);
                    return;
                }
			}
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

			if(!PantheonQoL.enabled) return;
			if(sceneName != "GG_Spa" && sceneName != "GG_Unn" && sceneName != "GG_Wyrm" && sceneName != "GG_Engine" && sceneName != "GG_Engine_Root") return;

			var doTransition = __instance.GetType().GetField("doTransition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var doTransitionIn = __instance.GetType().GetField("doTransitionIn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			var doTransitionOut = __instance.GetType().GetField("doTransitionOut", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			doTransition.SetValue(__instance, false);
			doTransitionIn.SetValue(__instance, false);
			doTransitionOut.SetValue(__instance, false);
//			__instance.bossesDeadWaitTime = 0f;
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
    }
}

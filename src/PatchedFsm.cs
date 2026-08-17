using System.Resources;
using GlobalEnums;
using PantheonQoL;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.Events;
using HarmonyLib;
using System.Drawing;
using InControl.NativeDeviceProfiles;
using System.Collections;
using UnityEngine.EventSystems;

namespace PantheonQoL;

public class PatchedFsm
{
    public static Func<object, MethodInfo, object[], object> InvokeMethod = (instance, method, obj) => method.Invoke(instance, obj);
	public static float bossRoarWait = 0.5f;
    public class CustomLogicFsm : FsmStateAction
    {
        public Action<Fsm> action;
        public Action updateAction;
        public float time;
        public bool finishOnEnter;
        public Fsm fsm;
        public override void OnEnter()
        {
            if(time == 0f) action?.Invoke(fsm);
            else PantheonQoL.instance.StartCoroutine(DoActionWithDelay());
            if(updateAction == null && time == 0f) Finish();
            if(finishOnEnter) Finish();
        }
        public override void OnUpdate()
        {
            updateAction?.Invoke();
        }
        public IEnumerator DoActionWithDelay()
        {
            yield return new WaitForSeconds(time);
            action?.Invoke(fsm);
            Finish();
        }
        public CustomLogicFsm(Fsm fsm, float time = 0f, bool finishOnEnter = false)
        {
            this.fsm = fsm;
            this.time = time;
            this.finishOnEnter = finishOnEnter;
        }
    }
    private class CustomWaitConditionFsm : FsmStateAction
    {
    }
    private class CustomTrigger : MonoBehaviour
    {
        public Action<Fsm, FsmStateAction> action;
        public FsmStateAction fsmAction;
        public Fsm fsm;
        private void OnTriggerStay2D(Collider2D collider)
        {
            action?.Invoke(fsm, fsmAction);
            Destroy(this.gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            action?.Invoke(fsm, fsmAction);
            Destroy(this.gameObject);
        }
    }
    public class FsmPatch
    {
        public string objName;
        public string fsmName;
        public int objNameHash;
        public int fsmNameHash;
		public bool isSingleFsm;
        public Func<Fsm, bool> method;
        public FsmPatch(string objName, string fsmName, Func<Fsm, bool> method, bool isSingleFsm = true)
        {
            this.objName = objName;
            this.fsmName = fsmName;
            this.method = method;
			this.isSingleFsm = isSingleFsm;

            this.objNameHash = objName.GetHashCode();
            this.fsmNameHash = fsmName.GetHashCode();
        }
    }
    public static string bossDeadEvent = "BOSS DEAD EVENT MOD";
    public string sceneName;
    public int sceneNameHash;
    public FsmPatch[] fsms;

    PatchedFsm(string sceneName, FsmPatch[] fsms)
    {
        this.sceneName = sceneName;
        this.fsms = fsms;

        this.sceneNameHash = this.sceneName.GetHashCode();
    }
    public static FsmPatch[] globalPatchedFsms = new FsmPatch[]{
		new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_TransitionsControl),
	};
    public static PatchedFsm[] patchedFsms = new PatchedFsm[]{
        new PatchedFsm("GG_Atrium_Roof", new FsmPatch[]
        {
            new FsmPatch("Inspect", "Challenge UI", PatchFsm_DoorAnimControl),
            new FsmPatch("Inspect", "npc_control", PatchFsm_NPCControl),
            new FsmPatch("door_dreamReturnGG", "Boss Sequence Finish", PatchFsm_DreamReturnControl),
        }),
        new PatchedFsm("GG_Spa", new FsmPatch[]
        {
            new FsmPatch("right1", "Custom Fade", PatchFsm_CustomFadeControl),
            new FsmPatch("Dream Entry", "Control", PatchFsm_DreamEntryControl),
            new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_EngineTransitionsControl),
        }),
        new PatchedFsm("GG_Engine", new FsmPatch[]
        {
            new FsmPatch("right1", "Custom Fade", PatchFsm_CustomFadeControl),
            new FsmPatch("Dream Entry", "Control", PatchFsm_DreamEntryControl),
            new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_EngineTransitionsControl),
        }),
        new PatchedFsm("GG_Unn", new FsmPatch[]
        {
            new FsmPatch("right1", "Custom Fade", PatchFsm_CustomFadeControl),
            new FsmPatch("Dream Entry", "Control", PatchFsm_DreamEntryControl),
            new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_EngineTransitionsControl),
        }),
        new PatchedFsm("GG_Engine_Prime", new FsmPatch[]
        {
            new FsmPatch("right1", "Custom Fade", PatchFsm_CustomFadeControl),
            new FsmPatch("Dream Entry", "Control", PatchFsm_DreamEntryControl),
            new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_EngineTransitionsControl),
        }),
        new PatchedFsm("GG_Engine_Root", new FsmPatch[]
        {
            new FsmPatch("right1", "Custom Fade", PatchFsm_CustomFadeControl),
            new FsmPatch("Dream Entry", "Control", PatchFsm_DreamEntryControl),
            new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_EngineTransitionsControl),
        }),
        new PatchedFsm("GG_Wyrm", new FsmPatch[]
        {
            new FsmPatch("right1", "Custom Fade", PatchFsm_CustomFadeControl),
            new FsmPatch("Dream Entry", "Control", PatchFsm_DreamEntryControl),
            new FsmPatch("gg_battle_transitions(Clone)", "Transitions", PatchFsm_EngineTransitionsControl),
        }),
        new PatchedFsm("DontDestroyOnLoad", new FsmPatch[]
        {
            new FsmPatch("Hero Death", "Hero Death Anim", PatchFsm_HeroDeathAnimControl),
        }),
        new PatchedFsm("Knight_Pickup", new FsmPatch[]
        {
            new FsmPatch("Knight", "Dream Return", PatchFsm_KnightDreamReturnControl),
        }),
        new PatchedFsm("GG_Workshop", new FsmPatch[]
        {
            new FsmPatch("door_dreamReturnGG", "Return from boss", PatchFsm_WorkshopDreamReturnControl, isSingleFsm: false),
            new FsmPatch("Inspect", "GG Boss UI", PatchFsm_StatueInspectControl, isSingleFsm: false),
            new FsmPatch("Inspect", "npc_control", PatchFsm_NPCControl),
        }),
        new PatchedFsm("GG_Gruz_Mother", new FsmPatch[]
        {
            new FsmPatch("Corpse Big Fly 1(Clone)", "corpse", PatchFsm_GruzzMotherCorpseControl),
//            new FsmPatch("Corpse Big Fly Burster(Clone)", "burster", PatchFsm_GruzzMotherCorpseBursterControl),
        }),
        new PatchedFsm("GG_Gruz_Mother_V", new FsmPatch[]
        {
            new FsmPatch("Corpse Big Fly 1(Clone)", "corpse", PatchFsm_GruzzMother_V_CorpseControl),
//            new FsmPatch("Corpse Big Fly Burster(Clone)", "burster", PatchFsm_GruzzMotherCorpseBursterControl),
        }),
        new PatchedFsm("GG_Vengefly", new FsmPatch[]
        {
            new FsmPatch("Dream Entry", "Control", PatchFsm_VengeflyKingDreamEntryControl),
            new FsmPatch("Corpse Giant Buzzer Col(Clone)", "corpse", PatchFsm_VengeflyKingCorpseControl),
        }),
        new PatchedFsm("GG_Vengefly_V", new FsmPatch[]
        {
            new FsmPatch("Dream Entry", "Control", PatchFsm_VengeflyKingDreamEntryControl),
            new FsmPatch("Giant Buzzer Col", "Big Buzzer", PatchFsm_VengeflyKingControl),
            new FsmPatch("Giant Buzzer Col (1)", "Big Buzzer", PatchFsm_VengeflyKingControl),
            new FsmPatch("Corpse Giant Buzzer Col(Clone)", "corpse", PatchFsm_VengeflyKingDoubleCorpseControl),
        }),
        new PatchedFsm("GG_Brooding_Mawlek", new FsmPatch[]
        {
            new FsmPatch("Corpse Egg Guardian(Clone)", "corpse", PatchFsm_BroodingMawlekCorpseControl),
            new FsmPatch("Mawlek Body", "Mawlek Control", PatchFsm_BroodingMawlekControl),
        }),
        new PatchedFsm("GG_Brooding_Mawlek_V", new FsmPatch[]
        {
            new FsmPatch("Corpse Egg Guardian(Clone)", "corpse", PatchFsm_BroodingMawlekCorpseControl),
            new FsmPatch("Mawlek Body", "Mawlek Control", PatchFsm_BroodingMawlekControl),
            new FsmPatch("Battle Scene", "Activate Boss", PatchFsm_BroodingMawlek_V_ActivateBoss),
        }),
        new PatchedFsm("GG_False_Knight", new FsmPatch[]
        {
            new FsmPatch("False Knight New", "FalseyControl", PatchFsm_FalseKnightControl),
        }),
        new PatchedFsm("GG_Failed_Champion", new FsmPatch[]
        {
            new FsmPatch("False Knight Dream", "FalseyControl", PatchFsm_FalseKnightDreamControl),
        }),
        new PatchedFsm("GG_Hornet_1", new FsmPatch[]
        {
            new FsmPatch("Corpse Hornet GG(Clone)", "Control", PatchFsm_HornetCorpseControl),
        }),
        new PatchedFsm("GG_Hornet_2", new FsmPatch[]
        {
            new FsmPatch("Corpse Hornet GG(Clone)", "Control", PatchFsm_HornetCorpseControl),
        }),
        new PatchedFsm("GG_Mega_Moss_Charger", new FsmPatch[]
        {
            new FsmPatch("Mega Moss Charger", "Mossy Control", PatchFsm_MegaMossChargerControl),
            new FsmPatch("Corpse Mega Moss Charger(Clone)", "Corpse", PatchFsm_MegaMossChargerCorpseControl),
        }),
        new PatchedFsm("GG_Flukemarm", new FsmPatch[]
        {
            new FsmPatch("Corpse Fluke Mother(Clone)", "Corpse Control", PatchFsm_FlukemarmCorpseControl),
        }),
        new PatchedFsm("GG_Mantis_Lords", new FsmPatch[]
        {
            new FsmPatch("Mantis Battle", "Battle Control", PatchFsm_MantisLordsBattleControl),
            new FsmPatch("Battle Sub", "Start", PatchFsm_MantisLordsBattleSub),
            new FsmPatch("Mantis Battle", "FSM", PatchFsm_MantisLordsBattleFSMControl),
            new FsmPatch("Corpse Mantis Lord First(Clone)", "Corpse", PatchFsm_MantisLordsFirstMantisCorpseControl),
            new FsmPatch("Mantis Lord Throne 2", "Mantis Throne Main", PatchFsm_MantisLordsMantisLordThrone2),
        }),
        new PatchedFsm("GG_Mantis_Lords_V", new FsmPatch[]
        {
            new FsmPatch("Mantis Battle", "Battle Control", PatchFsm_MantisLordsTripleBattleControl),
            new FsmPatch("Mantis Battle", "FSM", PatchFsm_MantisLordsBattleFSMControl),
//            new FsmPatch("Mantis Lord", "Mantis Lord", PatchFsm_MantisLordsMantisControl),
            new FsmPatch("Corpse Mantis Lord First(Clone)", "Corpse", PatchFsm_MantisLordsFirstMantisCorpseControl),
            new FsmPatch("Battle Sub", "Start", PatchFsm_MantisLordsTripleBattleSub),
            new FsmPatch("Mantis Lord Throne 2", "Mantis Throne Main", PatchFsm_MantisLordsTripleMantisLordThrone2),
        }),
        new PatchedFsm("GG_Oblobbles", new FsmPatch[]
        {
            new FsmPatch("Mega Fat Bee", "FSM", PatchFsm_ObblobleFSM),
        }),
        new PatchedFsm("GG_Hive_Knight", new FsmPatch[]
        {
            new FsmPatch("Hive Knight", "Control", PatchFsm_HiveKnightControl),
            new FsmPatch("Corpse Hive Knight(Clone)", "corpse", PatchFsm_HiveKnightCorpseControl),
        }),
        new PatchedFsm("GG_Broken_Vessel", new FsmPatch[]
        {
            new FsmPatch("Infected Knight", "IK Control", PatchFsm_BrokenVesselIKControl),
            new FsmPatch("Corpse Infected Knight(Clone)", "corpse", PatchFsm_BrokenVesselCorpseControl),
        }),
        new PatchedFsm("GG_Lost_Kin", new FsmPatch[]
        {
            new FsmPatch("Lost Kin", "IK Control", PatchFsm_LostKinIKControl),
            new FsmPatch("Corpse Infected Knight Dream(Clone)", "corpse", PatchFsm_LostKinCorpseControl),
        }),
        new PatchedFsm("GG_Nosk", new FsmPatch[]
        {
            new FsmPatch("Mimic Spider", "Mimic Spider", PatchFsm_NoskControl),
            new FsmPatch("Corpse Mimic Spider(Clone)", "corpse", PatchFsm_NoskCorpseControl),
        }),
        new PatchedFsm("GG_Nosk_V", new FsmPatch[]
        {
            new FsmPatch("Mimic Spider", "Mimic Spider", PatchFsm_NoskControl),
            new FsmPatch("Corpse Mimic Spider(Clone)", "corpse", PatchFsm_NoskCorpseControl),
        }),
        new PatchedFsm("GG_Nosk_Hornet", new FsmPatch[]
        {
            new FsmPatch("Battle Scene", "Battle Control", PatchFsm_WingedNoskBattleScene),
            new FsmPatch("Corpse Hornet Nosk(Clone)", "corpse", PatchFsm_WingedNoskCorpseControl),
        }),
        new PatchedFsm("GG_Collector", new FsmPatch[]
        {
             new FsmPatch("Jar Collector", "Control", PatchFsm_CollectorControl),
            new FsmPatch("Jar Collector", "Death", PatchFsm_CollectorDeathControl),
        }),
        new PatchedFsm("GG_Collector_V", new FsmPatch[]
        {
            new FsmPatch("Jar Collector", "Control", PatchFsm_CollectorControl),
            new FsmPatch("Jar Collector", "Death", PatchFsm_Collector_V_DeathControl),
        }),
         new PatchedFsm("GG_God_Tamer", new FsmPatch[]
         {
//             new FsmPatch("Entry Object", "Control", PatchFsm_GodTamerEntryObject),
             new FsmPatch("Corpse Lobster(Clone)", "Death", PatchFsm_GodTamerLobsterCorpseControl),
         }),
         new PatchedFsm("GG_Crystal_Guardian", new FsmPatch[]
         {
             new FsmPatch("Corpse Mega Zombie Beam Miner Esc(Clone)", "Control", PatchFsm_CrystalGuardianCorpseControl),
         }),
         new PatchedFsm("GG_Crystal_Guardian_2", new FsmPatch[]
         {
             new FsmPatch("Corpse Mega Zombie Beam Miner(Clone)", "corpse", PatchFsm_CrystalGuardian2CorpseControl),
         }),
         new PatchedFsm("GG_Uumuu", new FsmPatch[]
         {
             new FsmPatch("Corpse Mega Jellyfish(Clone)", "Death", PatchFsm_UumuuCorpseControl),
         }),
         new PatchedFsm("GG_Uumuu_V", new FsmPatch[]
         {
             new FsmPatch("Corpse Mega Jellyfish(Clone)", "Death", PatchFsm_UumuuCorpseControl),
         }),
        new PatchedFsm("GG_Traitor_Lord", new FsmPatch[]
        {
            new FsmPatch("Mantis Traitor Lord", "Mantis", PatchFsm_TraitorLordControl),
            new FsmPatch("Battle Scene", "Battle Control", PatchFsm_TraitorLordBattleControl),
            new FsmPatch("Corpse Traitor Lord(Clone)", "FSM", PatchFsm_TraitorLordCorpseControl),
        }),
        new PatchedFsm("GG_Grey_Prince_Zote", new FsmPatch[]
        {
            new FsmPatch("Grey Prince", "Control", PatchFsm_GreyPrinceZoteControl),
            new FsmPatch("Grey Prince Title", "Control", PatchFsm_GreyPrinceZoteTitleControl),
            new FsmPatch("Corpse Grey Prince(Clone)", "Control", PatchFsm_GreyPrinceZoteCorpseControl),
        }),
        new PatchedFsm("GG_Mage_Knight", new FsmPatch[]
        {
            new FsmPatch("Mage Knight", "Mage Knight", PatchFsm_SoulWarriorControl),
        }),
        new PatchedFsm("GG_Mage_Knight_V", new FsmPatch[]
        {
            new FsmPatch("Mage Knight", "Mage Knight", PatchFsm_SoulWarriorControl),
        }),
        new PatchedFsm("GG_Soul_Master", new FsmPatch[]
        {
            new FsmPatch("Mage Lord", "Mage Lord", PatchFsm_SoulMasterControl),
            new FsmPatch("Mage Lord Phase2", "Mage Lord 2", PatchFsm_SoulMasterPhase2Control),
            new FsmPatch("Corpse Mage Lord 1(Clone)", "Corpse", PatchFsm_SoulMasterCorpse1Control),
            new FsmPatch("Corpse Mage Lord 2(Clone)", "corpse", PatchFsm_SoulMasterCorpse2Control),
        }),
        new PatchedFsm("GG_Soul_Tyrant", new FsmPatch[]
        {
            new FsmPatch("Dream Mage Lord", "Mage Lord", PatchFsm_SoulTyrantControl),
            new FsmPatch("Dream Mage Lord Phase2", "Mage Lord 2", PatchFsm_SoulTyrantPhase2Control),
            new FsmPatch("Corpse Dream Mage Lord 1(Clone)", "Get Quake", PatchFsm_SoulTyrantCorpse1Control),
            new FsmPatch("Corpse Dream Mage Lord 2(Clone)", "corpse", PatchFsm_SoulTyrantCorpse2Control),
        }),
        new PatchedFsm("GG_Dung_Defender", new FsmPatch[]
        {
            new FsmPatch("Dung Defender", "Dung Defender", PatchFsm_DungDefenderControl),
            new FsmPatch("Corpse Dung Defender(Clone)", "Corpse", PatchFsm_DungDefenderCorpseControl),
        }),
        new PatchedFsm("GG_White_Defender", new FsmPatch[]
        {
            new FsmPatch("White Defender", "Dung Defender", PatchFsm_WhiteDefenderControl),
            new FsmPatch("Corpse White Defender(Clone)", "Control", PatchFsm_WhiteDefenderCorpseControl),
        }),
        new PatchedFsm("GG_Watcher_Knights", new FsmPatch[]
        {
            new FsmPatch("Black Knight 1", "Black Knight", PatchFsm_WatcherKnight1Control),
            new FsmPatch("Battle Control", "Battle Control", PatchFsm_WatcherKnightBattleControl),
//            new FsmPatch("Corpse Black Knight 1(Clone)", "Corpse Black Knight", PatchFsm_WatcherKnight1CorpseControl),
        }),
        new PatchedFsm("GG_Ghost_No_Eyes", new FsmPatch[]
        {
            new FsmPatch("Ghost Death No Eyes(Clone)", "Control", PatchFsm_NoEyesCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_No_Eyes_V", new FsmPatch[]
        {
            new FsmPatch("Ghost Death No Eyes(Clone)", "Control", PatchFsm_NoEyesCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Marmu", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Marmu(Clone)", "Control", PatchFsm_MarmuCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Marmu_V", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Marmu(Clone)", "Control", PatchFsm_Marmu_V_CorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Xero", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Xero(Clone)", "Control", PatchFsm_XeroCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Xero_V", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Xero(Clone)", "Control", PatchFsm_XeroCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Markoth", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Markoth(Clone)", "Control", PatchFsm_MarkothCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Markoth_V", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Markoth(Clone)", "Control", PatchFsm_Markoth_V_CorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Galien", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Galien(Clone)", "Control", PatchFsm_GalienCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Gorb", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Slug(Clone)", "Control", PatchFsm_GorbCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Gorb_V", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Slug(Clone)", "Control", PatchFsm_GorbCorpseControl),
        }),
        new PatchedFsm("GG_Ghost_Hu", new FsmPatch[]
        {
            new FsmPatch("Ghost Death Hu(Clone)", "Control", PatchFsm_ElderHuCorpseControl),
        }),
        new PatchedFsm("GG_Nailmasters", new FsmPatch[]
        {
            new FsmPatch("Oro", "nailmaster", PatchFsm_OroAndMatoControl),
            new FsmPatch("Mato", "nailmaster", PatchFsm_OroAndMatoControl),
            new FsmPatch("Brothers", "Combo Control", PatchFsm_OroAndMatoComboControl),
        }),
        new PatchedFsm("GG_Painter", new FsmPatch[]
        {
            new FsmPatch("Sheo Boss", "nailmaster_sheo", PatchFsm_SheoControl),
            new FsmPatch("Corpse Sheo(Clone)", "Control", PatchFsm_SheoCorpseControl),
        }),
        new PatchedFsm("GG_Sly", new FsmPatch[]
        {
            new FsmPatch("Sly Boss", "Control", PatchFsm_SlyControl),
        }),
        new PatchedFsm("GG_Hollow_Knight", new FsmPatch[]
        {
            new FsmPatch("HK Prime", "Control", PatchFsm_PureVesselControl),
            new FsmPatch("Corpse HK Prime(Clone)", "corpse", PatchFsm_PureVesselCorpseControl),
        }),
        new PatchedFsm("GG_Grimm", new FsmPatch[]
        {
            new FsmPatch("Grimm Boss", "Control", PatchFsm_GrimmBossControl),
        }),
        new PatchedFsm("GG_Grimm_Nightmare", new FsmPatch[]
        {
            new FsmPatch("Nightmare Grimm Boss", "Control", PatchFsm_GrimmNightmareBossControl),
            new FsmPatch("Grimm Control", "Control", PatchFsm_GrimmNightmareGrimmControl),
        }),
        new PatchedFsm("GG_Radiance", new FsmPatch[]
        {
            new FsmPatch("Boss Control", "Control", PatchFsm_AbsRadianceBossControl),
        }),
    };

    public static void SetTransitionToState(FsmState state, FsmState to, int transitionIndex)
    {
        state.Transitions[transitionIndex].ToState = to.Name;
        state.Transitions[transitionIndex].ToFsmState = to;
    }
    public static T[] InsertInArray<T>(T[] array, T elem, int index)
    {
        var list = array.ToList();
        list.Insert(index, elem);
        return list.ToArray();
    }
    public static T[] RemoveFromArray<T>(T[] array, int index)
    {
        var list = array.ToList();
        list.RemoveAt(index);
        return list.ToArray();
    }
    public static GameObject CreateTrigger(string sceneName)
    {
        var customTrigger = new GameObject("CustomTrigger");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(customTrigger, UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));
        customTrigger.layer = (int)PhysLayers.HERO_DETECTOR;

        var customCollider = customTrigger.AddComponent<BoxCollider2D>();

        customCollider.isTrigger = true;
        customCollider.size = new Vector2(25, 18);
        return customTrigger;
    }

    public static bool PatchFsm_TransitionsControl(Fsm fsm)
    {
		var Out = fsm.GetState("Out");
		var In = fsm.GetState("In");

		var origOutWait = ((Wait)Out.Actions[6]).time.Value;

		if(!PantheonQoL.skipTransitionAnimation) return true;

		((Wait)Out.Actions[6]).time = 0.2f;

		var battleEnd = fsm.GameObject.transform.Find("battle_end");
		if(battleEnd != null){
			var battleEndAnimator = battleEnd.GetComponent<Animator>();
			battleEndAnimator.speed = origOutWait / ((Wait)Out.Actions[6]).time.Value;
		}

        return true;
    }

    public static bool PatchFsm_EngineTransitionsControl(Fsm fsm)
    {
		var Out = fsm.GetState("Out");
		var In = fsm.GetState("In");

		if(!PantheonQoL.skipTransitionAnimation) return true;

		var battle_enter = fsm.GameObject.transform.Find("battle_enter");
		if(battle_enter != null){
			var paleGlower = battle_enter.Find("pale_glower (2)");
			var vignette_large = battle_enter.Find("vignette_large_v01 (1)");
			var white_solid = battle_enter.Find("white_solid (1)");
			if(paleGlower != null) paleGlower.gameObject.SetActive(false);
			if(vignette_large != null) vignette_large.gameObject.SetActive(false);
			if(white_solid != null) white_solid.gameObject.SetActive(false);
		}

        return true;
    }

    public static bool PatchFsm_DoorAnimControl(Fsm fsm)
    {
		var openUI= fsm.GetState("Open UI");
		var wait = fsm.GetState("Wait");
		var doorAnim = fsm.GetState("Door Anim");
		var hideShade = fsm.GetState("Hide Shade");
		var dreamBoxDown = fsm.GetState("Dream Box Down");
		var changeScene = fsm.GetState("Change Scene");
		var impact = fsm.GetState("Impact");
		
		((Wait)wait.Actions[0]).time = 0f;
		((Wait)doorAnim.Actions[2]).time = 0f;
		((Wait)hideShade.Actions[1]).time = 0f;
		((Wait)dreamBoxDown.Actions[3]).time = 0f;
//		((SetFsmFloat)dreamBoxDown.Actions[5]).setValue = 0f;

		var hudIn = new CustomLogicFsm(fsm);
		hudIn.action += (fsm) =>
		{
			GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");
		};

		var setupBossSequenceMenu = new CustomLogicFsm(fsm);
		setupBossSequenceMenu.action += (fsm) =>
		{
			var menu = GameObject.Find("GG_Challenge_Door_Canvas(Clone)");
			PantheonQoL.Log.LogInfo(menu);
			
			var beginButton = menu.transform.Find("Panel/BeginButton").gameObject;
			PantheonQoL.Log.LogInfo(beginButton);
			EventSystem eventSystem = EventSystem.current;
			PantheonQoL.Log.LogInfo(eventSystem);
			InputHandler inputHandler = InputHandler.Instance;

			eventSystem.SetSelectedGameObject(beginButton);
		};
		
		doorAnim.Actions = InsertInArray(doorAnim.Actions, hudIn, 0);
//		openUI.Actions = InsertInArray(openUI.Actions, setupBossSequenceMenu, openUI.Actions.Length);

        return true;
    }

    public static bool PatchFsm_HeroDeathAnimControl(Fsm fsm)
    {
		var animStart = fsm.GetState("Anim Start");
		var bursting = fsm.GetState("Bursting");
		var dreamReturn = fsm.GetState("Dream Return");

		animStart.Actions[5].Enabled = false;
		bursting.Actions[5].Enabled = false;
		bursting.Actions[4].Enabled = false;
		dreamReturn.Actions[4].Enabled = false;
		((BeginSceneTransition)dreamReturn.Actions[6]).visualization = GameManager.SceneLoadVisualizations.Custom;
		
        return true;
    }

    public static bool PatchFsm_CustomFadeControl(Fsm fsm)
    {
		var fadeOut = fsm.GetState("Fade Out");
		((SetFsmFloat)fadeOut.Actions[0]).setValue = 0.1f;

//		fadeOut.Actions[0].Enabled = false;
//		fadeOut.Actions[1].Enabled = false;
		
        return true;
    }

    public static bool PatchFsm_DreamReturnControl(Fsm fsm)
    {
		var pause = fsm.GetState("Pause");
		var doorEntry = fsm.GetState("Door Entry");
		var doorOpen = fsm.GetState("Door Open");
		var takeControl = fsm.GetState("Take Control");
		var waitForDreamReturn = fsm.GetState("Wait for dream return");
		var wait = fsm.GetState("Wait");
		var dreamReturn = fsm.GetState("Dream Return");
		var TPIn = fsm.GetState("TP In");
		var TPIn2 = fsm.GetState("TP In 2");
		var returnControl = fsm.GetState("Return Control");
		var fadeIn = fsm.GetState("Fade In");
		var save = fsm.GetState("Save");

		var resetHero = new CustomLogicFsm(fsm);
		resetHero.action += (fsm) => {
			var resetMotion = HeroController.instance.GetType().GetMethod("ResetMotion", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			resetMotion.Invoke(HeroController.instance, new object[]{});
			HeroController.instance.ResetState();
		};

		var transitionPoint = fsm.GameObject.GetComponent<TransitionPoint>();
		transitionPoint.dontWalkOutOfDoor = true;

//		((ActivateGameObject)pause.Actions[0]).activate = true;
		waitForDreamReturn.Actions[0].Enabled = false;
//		takeControl.Actions[0].Enabled = false;
//		takeControl.Actions[1].Enabled = false;
		wait.Actions[0].Enabled = false;
		TPIn.Actions[1].Enabled = false;
		TPIn2.Actions[0].Enabled = false;
//		fadeIn.Actions[0].Enabled = false;
//		fadeIn.Actions[1].Enabled = false;
		doorOpen.Actions[0].Enabled = false;
//		save.Actions[1].Enabled = false;

		SetTransitionToState(doorEntry, doorOpen, 2);

//		takeControl.Actions = InsertInArray(takeControl.Actions, resetHero, 0);

        return true;
    }

    public static bool PatchFsm_KnightDreamReturnControl(Fsm fsm)
    {
		var isLong = fsm.GetState("Long?");
		var GCPause = fsm.GetState("GC Pause");
		var GGReturn = fsm.GetState("GG Return");
		var isLongerGrimmReturn = fsm.GetState("Longer Grimm Return?");
		var prostrate = fsm.GetState("Prostrate");
		var getUp = fsm.GetState("Get Up");
		var save = fsm.GetState("Save");

		isLong.Actions[0].Enabled = false;
		getUp.Actions[1].Enabled = false;
		getUp.Actions[0].Enabled = false;
		GGReturn.Actions[2].Enabled = false;
		GCPause.Actions[0].Enabled = false;
		prostrate.Actions[11].Enabled = false;
		
		SetTransitionToState(prostrate, getUp, 0);

        return true;
    }

    public static bool PatchFsm_NPCControl(Fsm fsm)
    {
		var init = fsm.GetState("Init");
		var idle = fsm.GetState("Idle");
		var takeControl = fsm.GetState("Take Control");
		var checkProximity = fsm.GetState("Check Proximity");
		var turnHeroRight = fsm.GetState("Turn Hero Right");
		var turnHeroLeft = fsm.GetState("Turn Hero Left");

		((Wait)init.Actions[8]).Enabled = false;

		SetTransitionToState(takeControl, checkProximity, 0);

		SetTransitionToState(checkProximity, turnHeroRight, 3);
		SetTransitionToState(checkProximity, turnHeroLeft, 0);

        return true;
    }

    public static bool PatchFsm_WorkshopDreamReturnControl(Fsm fsm)
    {
		var wait = fsm.GetState("Wait");
		var transitionWait = fsm.GetState("Transition Wait");
		var transitionIn = fsm.GetState("Transition In");
		var getUpWait = fsm.GetState("Get Up Wait");
		var gottenUp = fsm.GetState("Gotten Up");

		var finishState = new CustomLogicFsm(fsm);
		finishState.action += (fsm) => {
			fsm.FsmComponent.SendEvent("DREAM WAKE");
		};

		wait.Actions[3].Enabled = false;
//		transitionIn.Actions[0].Enabled = false;
		transitionWait.Actions[0].Enabled = false;
		transitionWait.Actions[1].Enabled = false;
		transitionWait.Actions[2].Enabled = false;
		transitionWait.Actions[3].Enabled = false;
//		transitionIn.Transitions[0].FsmEvent = new FsmEvent("FINISHED");
		getUpWait.Actions[0].Enabled = false;

		transitionIn.Actions = InsertInArray(transitionIn.Actions, finishState, 1);

        return true;
    }

    public static bool PatchFsm_StatueInspectControl(Fsm fsm)
    {
		var challenge = fsm.GetState("Challenge");
		var challengeAudio = fsm.GetState("Challenge Audio");
		var transition = fsm.GetState("Transition");
		var dreamBoxDown = fsm.GetState("Dream Box Down");
		var resetPlayer = fsm.GetState("Reset Player");

		challenge.Actions[1].Enabled = false;
		challengeAudio.Actions[1].Enabled = false;

		var finishState = new CustomLogicFsm(fsm);
		finishState.action += (fsm) => {
			fsm.FsmComponent.SendEvent("FINISHED");
		};

		challenge.Actions = InsertInArray(challenge.Actions, finishState, 1);
		challengeAudio.Actions = InsertInArray(challengeAudio.Actions, finishState, 1);

		SetTransitionToState(dreamBoxDown, resetPlayer, 0);

        return true;
    }

    public static bool PatchFsm_BattleTransitions(Fsm fsm)
    {

        var _out = fsm.GetState("Out");

        _out.Actions[6].Enabled = false;

        return true;
    }
    public static bool PatchFsm_GruzzMotherCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var ready = fsm.GetState("Ready");
        var steam = fsm.GetState("Steam");
        var blow = fsm.GetState("Blow");

//		blow.Actions[10].Enabled = false;

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action = (fsm) => {
			if(fsm.ActiveStateName == "Init") fsm.FsmComponent.SendEvent("TEST");
//			else if(fsm.ActiveStateName == "Init") fsm.FsmComponent.SendEvent("TEST");
			else fsm.FsmComponent.SendEvent("FINISHED");
		};

//		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GruzzMother_V_CorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var ready = fsm.GetState("Ready");
        var steam = fsm.GetState("Steam");
        var music = fsm.GetState("Music");
        var blow = fsm.GetState("Blow");

//		blow.Actions[10].Enabled = false;

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool doSkipStates = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitWhenOnTheGround = new CustomLogicFsm(fsm);
		waitWhenOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				doSkipStates = true;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;
				
				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitBlowAnimation = new CustomLogicFsm(fsm);
		waitBlowAnimation.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action = (fsm) => {
			IEnumerator enumerator(){
				while(!doSkipStates) yield return null;

				if(fsm.ActiveStateName == "Init") fsm.FsmComponent.SendEvent("TEST");
				else fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitWhenOnTheGround, 0);
		init.Actions = InsertInArray(init.Actions, waitBlowAnimation, 0);

//		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GruzzMotherCorpseBursterControl(Fsm fsm)
    {
        var stopEmit = fsm.GetState("Stop Emit");
        var landed = fsm.GetState("Landed");

        landed.Actions[2].Enabled = false;
        stopEmit.Actions[1].Enabled = false;

        return true;
    }
    public static bool PatchFsm_DreamEntryControl(Fsm fsm)
    {
		var doorEntry = fsm.GetState("Door Entry");
		var takeControl = fsm.GetState("Take Control");
        var startKneeling = fsm.GetState("Start Kneeling");
        var pauseKneeling = fsm.GetState("Pause Kneeling");
        var waitForTransition = fsm.GetState("Wait for Transition");
		var hidePlayer = fsm.GetState("Hide Player");
        var startFade = fsm.GetState("Start Fade");
        var returnControl = fsm.GetState("Return Control");

		var resetHero = new CustomLogicFsm(fsm);
		resetHero.action += (fsm) => {
			var resetMotion = HeroController.instance.GetType().GetMethod("ResetMotion", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			resetMotion.Invoke(HeroController.instance, new object[]{});
			HeroController.instance.ResetState();
		};

		((SetFsmFloat)startFade.Actions[0]).setValue = 0.1f;
//		((SendEventByName)startFade.Actions[1]).sendEvent = new FsmString("FADE OUT INSTANT");
		startFade.Actions[2].Enabled = false;

		SetTransitionToState(doorEntry, waitForTransition, 0);
		SetTransitionToState(startFade, returnControl, 0);

		doorEntry.Actions = InsertInArray(doorEntry.Actions, resetHero, 0);

        return true;
    }
    public static bool PatchFsm_VengeflyKingDreamEntryControl(Fsm fsm)
    {
		var takeControl = fsm.GetState("Take Control");
		var hidePlayer = fsm.GetState("Hide Player");
        var startFade = fsm.GetState("Start Fade");

		if(PantheonQoL.doubleVengeflyKingsFastEntrance)
			((Wait)startFade.Actions[2]).Enabled = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var takeControlUntil = new CustomLogicFsm(fsm);
		takeControlUntil.action = (fsm) => {
			var resetInput = typeof(HeroController).GetMethod("ResetInput", BindingFlags.Instance | BindingFlags.NonPublic);
			var resetMotion = typeof(HeroController).GetMethod("ResetMotion", BindingFlags.Instance | BindingFlags.NonPublic);
			var resetLook = typeof(HeroController).GetMethod("ResetLook", BindingFlags.Instance | BindingFlags.NonPublic);
			var resetAttacks = typeof(HeroController).GetMethod("ResetAttacks", BindingFlags.Instance | BindingFlags.NonPublic);
			IEnumerator enumerator(){
				while(fsm.ActiveStateName != "Return Control"){
					resetInput.Invoke(HeroController.instance, new object[]{});
					resetMotion.Invoke(HeroController.instance, new object[]{});
					resetLook.Invoke(HeroController.instance, new object[]{});
					resetAttacks.Invoke(HeroController.instance, new object[]{});
					HeroController.instance.touchingWallL = false;
					HeroController.instance.touchingWallR = false;
					HeroController.instance.StopAnimationControl();
					yield return null;
				}
//				HeroController.instance.RegainControl();
//				HeroController.instance.StartAnimationControl();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		takeControl.Actions = InsertInArray(takeControl.Actions, takeControlUntil, 0);

        return true;
    }
    public static bool PatchFsm_VengeflyKingControl(Fsm fsm)
    {
		var setGG = fsm.GetState("Set GG");
		var introRoarAntic = fsm.GetState("Intro Roar Antic");
		var colMusic = fsm.GetState("Col Music");


		var arrivalAnimator = BossSceneController.Instance.transform.Find("Dream Entry/Knight Dream Arrival").gameObject.GetComponent<tk2dSpriteAnimator>();
		var warpInClip = arrivalAnimator.GetClipByName("Warp In");
		if(PantheonQoL.origVengeflyKingsKnightWarpInFps == -2f) PantheonQoL.origVengeflyKingsKnightWarpInFps = warpInClip.fps;
		else warpInClip.fps = PantheonQoL.origVengeflyKingsKnightWarpInFps;

		var animator = fsm.GameObject.GetComponent<tk2dSpriteAnimator>();
		var roarClip = animator.GetClipByName("Roar");
		if(PantheonQoL.origVengeflyKingsRoarFps == -2f) PantheonQoL.origVengeflyKingsRoarFps = roarClip.fps;
		else roarClip.fps = PantheonQoL.origVengeflyKingsRoarFps;

		float origWarpInDuration = warpInClip.Duration;

		if(PantheonQoL.doubleVengeflyKingsFastEntrance){
			warpInClip.fps *= PantheonQoL.doubleVengeflyKingsEntranceSpeed;
			float skippedSeconds = origWarpInDuration - warpInClip.Duration;

			((Wait)setGG.Actions[3]).time.Value -= (0.75f + skippedSeconds);
		}
//
//		if(skippedSeconds >= 2){
//			((Wait)setGG.Actions[3]).Enabled = false;
//		}
//		else{
//			((Wait)setGG.Actions[3]).time.Value -= skippedSeconds;
//		}

//		((Wait)introRoarAntic.Actions[5]).time.Value /= PantheonQoL.doubleVengeflyKingsEntranceSpeed;

//		roarClip.fps *= PantheonQoL.doubleVengeflyKingsEntranceSpeed;
//
//		var resetRoarClipFps = new CustomLogicFsm(fsm);
//		resetRoarClipFps.action = (fsm) => {
//			roarClip.fps = PantheonQoL.origVengeflyKingsRoarFps;
//		};
//
//		colMusic.Actions = InsertInArray(colMusic.Actions, resetRoarClipFps, 0);

        return true;
    }
    public static bool PatchFsm_VengeflyKingCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var musicOut2 = fsm.GetState("Music Out 2");
        var ready = fsm.GetState("Ready");
        var steam = fsm.GetState("Steam");
        var blow = fsm.GetState("Blow");
		
		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool doSkipStates = false;

		var playMakerComp = fsm.FsmComponent;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitWhenOnTheGround = new CustomLogicFsm(fsm);
		waitWhenOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				doSkipStates = true;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;
				
				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitBlowAnimation = new CustomLogicFsm(fsm);
		waitBlowAnimation.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

        init.Actions = InsertInArray(init.Actions, waitWhenOnTheGround, 0);
        init.Actions = InsertInArray(init.Actions, waitBlowAnimation, 0);

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action = (fsm) => {
			IEnumerator enumerator(){
				while(!doSkipStates) yield return null;

				if(fsm.ActiveStateName == "Init") fsm.FsmComponent.SendEvent("TEST");
				else fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_VengeflyKingDoubleCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var musicOut2 = fsm.GetState("Music Out 2");
        var ready = fsm.GetState("Ready");
        var steam = fsm.GetState("Steam");
        var blow = fsm.GetState("Blow");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool doSkipStates = false;

		var playMakerComp = fsm.FsmComponent;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitWhenOnTheGround = new CustomLogicFsm(fsm);
		waitWhenOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				int bossesLeft = (int)BossSceneController.Instance.GetType().GetField("bossesLeft", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(BossSceneController.Instance);
				if(bossesLeft != 0){
					yield break;
				}

				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				doSkipStates = true;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitBlowAnimation = new CustomLogicFsm(fsm);
		waitBlowAnimation.action += (fsm) => {
			IEnumerator enumerator(){
				int bossesLeft = (int)BossSceneController.Instance.GetType().GetField("bossesLeft", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(BossSceneController.Instance);
				if(bossesLeft != 0){
					yield break;
				}
				
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action = (fsm) => {
			IEnumerator enumerator(){
				while(!doSkipStates) yield return null;

				if(fsm.ActiveStateName == "Init") fsm.FsmComponent.SendEvent("TEST");
				else fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

        init.Actions = InsertInArray(init.Actions, waitWhenOnTheGround, 0);
        init.Actions = InsertInArray(init.Actions, waitBlowAnimation, 0);

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_BroodingMawlekControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var wakeRoar = fsm.GetState("Wake Roar");
        var roarEnd = fsm.GetState("Roar End");

        ((Wait)wakeRoar.Actions[7]).time = 0.25f;

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.Variables.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_BroodingMawlekCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var sting = fsm.GetState("Sting");
        var blow = fsm.GetState("Blow");

//		steam.Actions[2].Enabled = false;

		var playMakerComp = fsm.FsmComponent;

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitWhenOnTheGround = new CustomLogicFsm(fsm);
		waitWhenOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mawlek"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					var collider = obj.GetComponent<BoxCollider2D>();
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && collider != null && collider.enabled){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					var collider = obj.GetComponent<BoxCollider2D>();
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && collider != null && collider.enabled){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;
				
				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitBlowAnimation = new CustomLogicFsm(fsm);
		waitBlowAnimation.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action = (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(fsm.ActiveStateName == "Init") fsm.FsmComponent.SendEvent("TEST");
				if(fsm.ActiveStateName == "Ready") fsm.FsmComponent.SendEvent("FINISHED");
				if(fsm.ActiveStateName == "Steam") fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitWhenOnTheGround, 0);
		init.Actions = InsertInArray(init.Actions, waitBlowAnimation, 0);

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_BroodingMawlek_V_ActivateBoss(Fsm fsm)
    {
        var isInPantheon = fsm.GetState("In Pantheon?");
        var callMawlek = fsm.GetState("Call Mawlek");

		SetTransitionToState(isInPantheon, callMawlek, 0);

        return true;
    }
    public static bool PatchFsm_FalseKnightControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var checkIfGG = fsm.GetState("Check If GG");
        var pauseShort = fsm.GetState("Pause Short");
        var pauseLong = fsm.GetState("Pause Long");
        var stunLand = fsm.GetState("Stun Land");
        var stunStart = fsm.GetState("Stun Start");
        var deathAnimStart = fsm.GetState("Death Anim Start");
        var openMapShopAndJournal = fsm.GetState("Open Map Shop and Journal");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var betaEndEvent = fsm.GetState("Beta End Event");
        var blow = fsm.GetState("Blow");
        var deathHeadLandChinese = fsm.GetState("Death Head Land Chinese");
        var deathHeadLand = fsm.GetState("Death Head Land");
        var cough = fsm.GetState("Cough");

//        deathAnimStart.Actions[13].Enabled = false;
//        ((Wait)ready.Actions[4]).time = 0f;
//        ((Wait)steam.Actions[12]).time = 1f;
//        ((Wait)deathHeadLandChinese.Actions[3]).time = 0f;
//        ((Wait)deathHeadLand.Actions[2]).time = 0f;
        // ((SetVelocity2d)stunStart.Actions[10]).y = 1f;
//        ((Wait)stunLand.Actions[6]).time = 0f;
        ((Wait)pauseShort.Actions[0]).time = 0f;
        ((Wait)pauseLong.Actions[0]).time = 0f;
		steam.Actions[2].Enabled = false;
		((FloatCompare)stunLand.Actions[5]).float2 = 100f;
//		((Wait)stunLand.Actions[6]).time = 0.1f;

		var tk2dSpriteAnimator = fsm.GameObject.GetComponent<tk2dSpriteAnimator>();
		tk2dSpriteAnimator.GetClipByName("Stun Open").fps = 64;

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Falling Barrel") && obj.activeInHierarchy && obj.GetComponent<BoxCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Falling Barrel") && obj.activeInHierarchy && obj.GetComponent<BoxCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitBlowAnimation = new CustomLogicFsm(fsm);
		waitBlowAnimation.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(fsm.ActiveStateName == "Death Anim Start") playMakerComp.SendEvent("WAIT");
				if(fsm.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(fsm.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		deathAnimStart.Actions = InsertInArray(deathAnimStart.Actions, waitNoThreats, 0);
		cough.Actions = InsertInArray(cough.Actions, waitBlowAnimation, cough.Actions.Length);
		deathAnimStart.Actions = InsertInArray(deathAnimStart.Actions, doSkipStates, deathAnimStart.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, doSkipStates, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, doSkipStates, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_FalseKnightDreamControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var pauseShort = fsm.GetState("Pause Short");
        var pauseLong = fsm.GetState("Pause Long");
        var stunLand = fsm.GetState("Stun Land");
        var stunStart = fsm.GetState("Stun Start");
        var deathAnimStart = fsm.GetState("Death Anim Start");
        var steam = fsm.GetState("Steam");
        var deathHeadLand = fsm.GetState("Death Head Land");
        var blow = fsm.GetState("Blow");
        var ready = fsm.GetState("Ready");
        var dreamReturn = fsm.GetState("Dream Return");
        var cough = fsm.GetState("Cough");

//        ((Wait)deathAnimStart.Actions[10]).time = 0f;
//        ((Wait)deathHeadLand.Actions[2]).time = 0f;
        ((Wait)pauseShort.Actions[0]).time = 0f;
        ((Wait)pauseLong.Actions[0]).time = 0f;
		steam.Actions[2].Enabled = false;
//		((FloatCompare)stunLand.Actions[4]).float1 = 0f;

		var tk2dSpriteAnimator = fsm.GameObject.GetComponent<tk2dSpriteAnimator>();
		tk2dSpriteAnimator.GetClipByName("Stun Open").fps = 64;

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		bool hasSceneThreats = true;

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Falling Barrel") && obj.activeInHierarchy && obj.GetComponent<BoxCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Falling Barrel") && obj.activeInHierarchy && obj.GetComponent<BoxCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitBlowAnimation = new CustomLogicFsm(fsm);
		waitBlowAnimation.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(fsm.ActiveStateName == "Death Anim Start") playMakerComp.SendEvent("WAIT");
				if(fsm.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(fsm.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		deathAnimStart.Actions = InsertInArray(deathAnimStart.Actions, waitNoThreats, 0);
		cough.Actions = InsertInArray(cough.Actions, waitBlowAnimation, cough.Actions.Length);
		deathAnimStart.Actions = InsertInArray(deathAnimStart.Actions, doSkipStates, deathAnimStart.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, doSkipStates, ready.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, doSkipStates, steam.Actions.Length);

        return true;
    }
    public static bool PatchFsm_HornetCorpseControl(Fsm fsm)
    {
		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

        return true;
    }
    public static bool PatchFsm_MegaMossChargerControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var roar = fsm.GetState("Roar");
        var shake = fsm.GetState("Shake");
        var roarEnd = fsm.GetState("Roar End");

        ((Wait)shake.Actions[6]).time.Value = 2f;
        ((Wait)roar.Actions[11]).time = 0.25f;

        // roar.Actions = RemoveFromArray(roar.Actions, 6); //remove roar wave object

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.Variables.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_MegaMossChargerCorpseControl(Fsm fsm)
    {
        var doSaveDeath = fsm.GetState("Save Death?");
		var inAir = fsm.GetState("In Air");
		var save = fsm.GetState("Save");
		var steam = fsm.GetState("Steam");
		var ready = fsm.GetState("Ready");
        var land = fsm.GetState("Land");
        var blow = fsm.GetState("Blow");

//		SetTransitionToState(doSaveDeath, steam, 1);
//		SetTransitionToState(save, steam, 0);
//
		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var action = new CustomLogicFsm(fsm);
		action.action += (fsm) => {
			if(fsm.ActiveStateName == "In Air") fsm.FsmComponent.SendEvent("LAND");
			else fsm.FsmComponent.SendEvent("FINISHED");
		};

		inAir.Actions = InsertInArray(inAir.Actions, action, inAir.Actions.Length);
		land.Actions = InsertInArray(land.Actions, action, land.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, action, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, action, ready.Actions.Length);

        return true;
    }
    public static bool PatchFsm_FlukemarmCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var blow = fsm.GetState("Blow");
        var after = fsm.GetState("After");
        var dribblerOn = fsm.GetState("Dribbler On");
        var dribblerLow = fsm.GetState("Dribbler Low");
        var dribblerLow2 = fsm.GetState("Dribbler Low 2");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

        ((Wait)after.Actions[7]).time = 0f;
        ((Wait)dribblerOn.Actions[1]).time = 0f;
        ((Wait)dribblerLow.Actions[1]).time = 0f;
        ((Wait)dribblerLow2.Actions[1]).time = 0f;

        SetTransitionToState(init, blow, 0);

        return true;
    }
    public static bool PatchFsm_MantisLordsBattleFSMControl(Fsm fsm)
    {
        var state1 = fsm.GetState("State 1");
        var state2 = fsm.GetState("State 2");

        ((SendEventByName)state2.Actions[0]).delay = 0f;

        return true;
    }
    public static bool PatchFsm_MantisLordsBattleControl(Fsm fsm)
    {
        var return2 = fsm.GetState("Return 2");
		var bow = fsm.GetState("Bow");
        var gg = fsm.GetState("GG");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

//		((Wait)return2.Actions[1]).time = 6f;
//		gg.Actions[0].Enabled = false;

		var playMakerComp = fsm.FsmComponent;

		bool hasSceneThreats = true;
		bool isBossSceneEnded = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnTheGround = new CustomLogicFsm(fsm);
		waitOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f && !hasSceneThreats) break;
					yield return null;
				}

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitStandart = new CustomLogicFsm(fsm);
		waitStandart.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Return 2") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		return2.Actions = InsertInArray(return2.Actions, waitForPotentialThreats, 0);
		return2.Actions = InsertInArray(return2.Actions, waitOnTheGround, 0);
		return2.Actions = InsertInArray(return2.Actions, sendEvent, return2.Actions.Length);
		gg.Actions = InsertInArray(gg.Actions, waitStandart, gg.Actions.Length);

//        return2.Actions = InsertInArray(return2.Actions, return2.Actions[1], return2.Actions.Length);

//        return2.Actions[1].Enabled = false;

        return true;
    }
    public static bool PatchFsm_MantisLordsBattleSub(Fsm fsm)
    {
        var initPause = fsm.GetState("Init Pause");

//        ((Wait)initPause.Actions[0]).time = 1f;

		var playMakerComp = fsm.FsmComponent;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init Pause") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		initPause.Actions = InsertInArray(initPause.Actions, waitForPotentialThreats, 0);
		initPause.Actions = InsertInArray(initPause.Actions, sendEvent, initPause.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MantisLordsFirstMantisCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var leavePause = fsm.GetState("Leave Pause");

//        ((Wait)leavePause.Actions[1]).time = 0f;
//
//        init.Actions[2].Enabled = false;
//
		var playMakerComp = fsm.FsmComponent;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Leave Pause") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitForPotentialThreats, 0);
		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		leavePause.Actions = InsertInArray(leavePause.Actions, sendEvent, leavePause.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MantisLordsMantisLordThrone2(Fsm fsm)
    {
        var pause = fsm.GetState("Pause");
        var defeated = fsm.GetState("Defeated");

		bool hasSceneThreats = true;

		var playMakerComp = fsm.FsmComponent;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}
				
				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};


        var customActionSendEvent = new CustomLogicFsm(fsm);
        customActionSendEvent.action += (Fsm fsm) =>
        {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;
				
				if(playMakerComp.ActiveStateName == "Pause") fsm.FsmComponent.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Defeated") fsm.FsmComponent.SendEvent("WAIT");
			}
			coroutineHandler.StartCoroutine(enumerator());
        };

        pause.Actions = InsertInArray(pause.Actions, waitForPotentialThreats, 0);
        pause.Actions = InsertInArray(pause.Actions, customActionSendEvent, pause.Actions.Length);
        defeated.Actions = InsertInArray(defeated.Actions, customActionSendEvent, defeated.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MantisLordsTripleBattleSub(Fsm fsm)
    {
        var initPause = fsm.GetState("Init Pause");

//        ((Wait)initPause.Actions[0]).time = 1f;

		var playMakerComp = fsm.FsmComponent;

		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = true;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init Pause") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		initPause.Actions = InsertInArray(initPause.Actions, waitForPotentialThreats, 0);
		initPause.Actions = InsertInArray(initPause.Actions, sendEvent, initPause.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MantisLordsTripleBattleControl(Fsm fsm)
    {
        var return3 = fsm.GetState("Return 3");
		var bow = fsm.GetState("Bow");
        var gg = fsm.GetState("GG");

//		((Wait)return3.Actions[1]).time = 6f;
		var playMakerComp = fsm.FsmComponent;

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool hasSceneThreats = true;
		bool isBossSceneEnded = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnTheGround = new CustomLogicFsm(fsm);
		waitOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f && !hasSceneThreats) break;
					yield return null;
				}

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitStandart = new CustomLogicFsm(fsm);
		waitStandart.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp != null && playMakerComp.ActiveStateName == "Return 3") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp != null && playMakerComp.ActiveStateName == "Bow") playMakerComp.SendEvent("GG BOSS");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		return3.Actions = InsertInArray(return3.Actions, waitForPotentialThreats, return3.Actions.Length);
		return3.Actions = InsertInArray(return3.Actions, waitOnTheGround, return3.Actions.Length);
		gg.Actions = InsertInArray(gg.Actions, waitStandart, gg.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MantisLordsTripleRoarEnd(Fsm fsm)//deprecated
    {
        var end = fsm.GetState("End");

        end.Actions[3].Enabled = false;

        return true;
    }
    public static bool PatchFsm_MantisLordsTripleMantisLordThrone2(Fsm fsm)
    {
        var pause = fsm.GetState("Pause");
        var defeated = fsm.GetState("Defeated");
        var subsStand = fsm.GetState("Subs Stand");
        var IStand = fsm.GetState("I Stand");
        var roar1 = fsm.GetState("Roar 1");
        var roar2 = fsm.GetState("Roar 2");
        var returnPause = fsm.GetState("Return Pause");

//        pause.Actions[0].Enabled = false;

		bool hasSceneThreats = true;

		var playMakerComp = fsm.FsmComponent;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitForPotentialThreats = new CustomLogicFsm(fsm);
		waitForPotentialThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Shot Mantis Lord"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.x > 14.2f && obj.transform.position.x < 46.2f){
							yield return null;
							goto foreach1;
						}
					}
				}
				
				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

        pause.Actions = InsertInArray(pause.Actions, waitForPotentialThreats, 0);

        var customActionSendEvent = new CustomLogicFsm(fsm);
        customActionSendEvent.action += (Fsm fsm) =>
        {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;
				
				if(playMakerComp.ActiveStateName == "Pause") fsm.FsmComponent.SendEvent("FINISHED");
				else fsm.FsmComponent.SendEvent("WAIT");
			}
			coroutineHandler.StartCoroutine(enumerator());
        };

        defeated.Actions = InsertInArray(defeated.Actions, customActionSendEvent, defeated.Actions.Length);
        subsStand.Actions = InsertInArray(subsStand.Actions, customActionSendEvent, subsStand.Actions.Length);
        IStand.Actions = InsertInArray(IStand.Actions, customActionSendEvent, IStand.Actions.Length);
        roar1.Actions = InsertInArray(roar1.Actions, customActionSendEvent, roar1.Actions.Length);
        roar2.Actions = InsertInArray(roar2.Actions, customActionSendEvent, roar2.Actions.Length);

        return true;
    }
    public static bool PatchFsm_HiveKnightControl(Fsm fsm)
    {
        var intro = fsm.GetState("Intro");
        var introEnd = fsm.GetState("Intro End");

        ((Wait)intro.Actions[3]).time = 0.25f;

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        introEnd.Actions = InsertInArray(introEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_HiveKnightCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");
        var pause = fsm.GetState("Pause");
        var end = fsm.GetState("End");

		init.Actions[3].Enabled = false;

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var trackPotentialThreat = new CustomLogicFsm(fsm);
		trackPotentialThreat.action += (fsm) => {
			IEnumerator enumerator(){
				var scene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in scene.GetRootGameObjects()){
					if(obj.name.StartsWith("Hive Knight Glob")){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitStandart = new CustomLogicFsm(fsm);
		waitStandart.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Pause") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

        init.Actions = InsertInArray(init.Actions, waitStandart, 0);
        init.Actions = InsertInArray(init.Actions, trackPotentialThreat, 0);
        init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
        steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
        ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
        pause.Actions = InsertInArray(pause.Actions, sendEvent, pause.Actions.Length);

        return true;
    }
    public static bool PatchFsm_BrokenVesselIKControl(Fsm fsm)
    {
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

        var wait = new Wait
        {
            time = 0.25f,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions[5].Enabled = false;

        roar.Actions = InsertInArray(roar.Actions, wait, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_BrokenVesselCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var pause = fsm.GetState("Pause");
        var ready = fsm.GetState("Ready");
        var steam = fsm.GetState("Steam");
        var blow = fsm.GetState("Blow");
        var chineseBlow = fsm.GetState("Chinese Blow");
        var BGOpen = fsm.GetState("BG Open");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"IK Projectile"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.y < 45f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy && obj.transform.position.y < 45f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitStandart = new CustomLogicFsm(fsm);
		waitStandart.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

//				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("LAND");
				if(playMakerComp.ActiveStateName == "Pause") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);
		BGOpen.Actions = InsertInArray(BGOpen.Actions, waitStandart, BGOpen.Actions.Length);

        init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
        steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
        ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
        pause.Actions = InsertInArray(pause.Actions, sendEvent, pause.Actions.Length);

        return true;
    }
    public static bool PatchFsm_LostKinIKControl(Fsm fsm)
    {
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

        var wait = new Wait
        {
            time = 0.25f,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions[5].Enabled = false;

        roar.Actions = InsertInArray(roar.Actions, wait, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_LostKinCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var pause = fsm.GetState("Pause");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Parasite Balloon Spawner"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith("Parasite Balloon Spawner") && obj.activeInHierarchy){
							var control = obj.LocateMyFSM("Control");
							if(control != null && control.ActiveStateName != "Death" && control.ActiveStateName != "Death 2" && control.ActiveStateName != ""){
								yield return null;
								goto foreach1;
							}
						}
						if(obj.name.StartsWith("IK Projectile") && obj.activeInHierarchy && obj.transform.position.y < 45f){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith("Parasite Balloon Spawner") && obj.activeInHierarchy){
							var control = obj.LocateMyFSM("Control");
							if(control != null && control.ActiveStateName != "Death" && control.ActiveStateName != "Death 2" && control.ActiveStateName != ""){
								yield return null;
								goto foreach1;
							}
						}
						if(obj.name.StartsWith("IK Projectile") && obj.activeInHierarchy && obj.transform.position.y < 45f){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;
				
				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitStandart = new CustomLogicFsm(fsm);
		waitStandart.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("LAND");
				if(playMakerComp.ActiveStateName == "Pause") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);
		init.Actions = InsertInArray(init.Actions, waitStandart, 0);

        init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
        steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
        ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);
        pause.Actions = InsertInArray(pause.Actions, sendEvent, pause.Actions.Length);

        return true;
    }
    public static bool PatchFsm_ObblobleFSM(Fsm fsm)
    {
        var wait = fsm.GetState("Wait");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool areBossesDead = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var bossesLeft = BossSceneController.Instance.GetType().GetField("bossesLeft", BindingFlags.Instance | BindingFlags.NonPublic);
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Oblobble Shot"};

				while(true){
					if((int)bossesLeft.GetValue(BossSceneController.Instance) == 0) break;
					yield return null;
				}

				areBossesDead = true;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				while(true){
					if(areBossesDead) break;
					yield return null;
				}
				
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		wait.Actions = InsertInArray(wait.Actions, waitNoThreats, 0);
		wait.Actions = InsertInArray(wait.Actions, waitTimer, 0);

        return true;
    }
    public static bool PatchFsm_NoskControl(Fsm fsm)
    {
        var roarLoop = fsm.GetState("Roar Loop");
        var roarFinish = fsm.GetState("Roar Finish");

        var wait = new Wait
        {
            time = 0.25f,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roarLoop.Actions[1].Enabled = false;

        roarLoop.Actions = InsertInArray(roarLoop.Actions, wait, roarLoop.Actions.Length);
        roarFinish.Actions = InsertInArray(roarFinish.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_NoskCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Vomit Glob Nosk"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);
		init.Actions = InsertInArray(init.Actions, waitTimer, 0);

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }
    public static bool PatchFsm_WingedNoskBattleScene(Fsm fsm)
    {
        var transform1 = fsm.GetState("Transform 1");
        var transform2 = fsm.GetState("Transform 2");
        var transform3 = fsm.GetState("Transform 3");
        var transform4 = fsm.GetState("Transform 4");

        ((Wait)transform1.Actions[5]).time = 0.25f;
        ((Wait)transform2.Actions[2]).time = 0f;
        ((Wait)transform3.Actions[2]).time = 0f;

        var customActionSendEvent = new CustomLogicFsm(fsm);
        customActionSendEvent.action += (Fsm fsm) =>
        {
            fsm.FsmComponent.SendEvent("FINISHED");
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

//        transform1.Actions = InsertInArray(transform1.Actions, customActionSendEvent, transform1.Actions.Length);
        transform4.Actions = InsertInArray(transform4.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_WingedNoskCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");
 
		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;
		bool doSkipStates = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Vomit Glob Nosk") && obj.activeInHierarchy && obj.transform.position.y > 8f){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("Parasite Balloon Spawner") && obj.activeInHierarchy){
						string activeState = obj.LocateMyFSM("Control").ActiveStateName;
						if(activeState != "Death" && activeState != "Death 2" && activeState != ""){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Vomit Glob Nosk") && obj.activeInHierarchy && obj.transform.position.y > 7f){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("Parasite Balloon Spawner") && obj.activeInHierarchy){
						string activeState = obj.LocateMyFSM("Control").ActiveStateName;
						if(activeState != "Death" && activeState != "Death 2" && activeState != ""){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};
		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f && !hasSceneThreats) break;
					yield return null;
				}

				doSkipStates = true;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(!doSkipStates) yield return null;

				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);
		init.Actions = InsertInArray(init.Actions, waitOnGround, 0);
		init.Actions = InsertInArray(init.Actions, waitTimer, 0);

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }

    public static bool PatchFsm_GodTamerLobsterCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Vomit Glob") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Vomit Glob") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
				}

				foreach2:
				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Lancer") && obj.activeInHierarchy){
						string activeState = obj.LocateMyFSM("Control").ActiveStateName;
						if(activeState != "Defeat" && activeState != "Done"){
							yield return null;
							goto foreach2;
						}
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);
		blow.Actions = InsertInArray(blow.Actions, waitTimer, blow.Actions.Length);

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }

    public static bool PatchFsm_CrystalGuardianCorpseControl(Fsm fsm)
    {
        var playerData = fsm.GetState("PlayerData");
        var pause = fsm.GetState("Pause");
        var inAir = fsm.GetState("In Air");
        var deathLand = fsm.GetState("Death Land");
        var escapeAntic = fsm.GetState("Escape Antic");
        var escaped = fsm.GetState("Escaped");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;
		bool doSkipStates = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Laser Turret Mega") && obj.activeInHierarchy){
						string activeState = obj.LocateMyFSM("Laser Bug Mega").ActiveStateName;
						if(activeState != "Idle"){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnTheGround = new CustomLogicFsm(fsm);
		waitOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f && !hasSceneThreats) break;
					yield return null;
				}

				doSkipStates = true;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(!doSkipStates) yield return null;

//				if(playMakerComp.ActiveStateName == "In Air") playMakerComp.SendEvent("LAND");
				if(playMakerComp.ActiveStateName == "Death Land") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Escape Antic") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		playerData.Actions = InsertInArray(playerData.Actions, waitTimer, 0);
		playerData.Actions = InsertInArray(playerData.Actions, waitNoThreats, 0);
		playerData.Actions = InsertInArray(playerData.Actions, waitOnTheGround, 0);

		inAir.Actions = InsertInArray(inAir.Actions, sendEvent, inAir.Actions.Length);
		deathLand.Actions = InsertInArray(deathLand.Actions, sendEvent, deathLand.Actions.Length);
		escapeAntic.Actions = InsertInArray(escapeAntic.Actions, sendEvent, escapeAntic.Actions.Length);

        return true;
    }

    public static bool PatchFsm_CrystalGuardian2CorpseControl(Fsm fsm)
    {
        var stopEmit = fsm.GetState("Stop Emit");
        var initiate = fsm.GetState("Initiate");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Laser Turret Mega") && obj.activeInHierarchy){
						string activeState = obj.LocateMyFSM("Laser Bug Mega").ActiveStateName;
						if(activeState != "Idle"){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnTheGround = new CustomLogicFsm(fsm);
		waitOnTheGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f && !hasSceneThreats) break;
					yield return null;
				}

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		initiate.Actions = InsertInArray(initiate.Actions, waitTimer, 0);
		initiate.Actions = InsertInArray(initiate.Actions, waitNoThreats, 0);
		initiate.Actions = InsertInArray(initiate.Actions, waitOnTheGround, 0);

        return true;
    }

    public static bool PatchFsm_UumuuCorpseControl(Fsm fsm)
    {
		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		IEnumerator enumerator(){
			yield return null;
			BossSceneController.Instance.bossesDeadWaitTime = 3.5f;
		}
//		coroutineHandler.StartCoroutine(enumerator());

        return true;
    }

    public static bool PatchFsm_CollectorControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var roar = fsm.GetState("Roar");
		var music = fsm.GetState("Music");

        roar.Actions[2].Enabled = false;
		((Wait)roar.Actions[12]).time = 0.25f;

		var disableRoarEmitter = new CustomLogicFsm(fsm);
		disableRoarEmitter.action = (fsm) => {
			fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
		};

        music.Actions = InsertInArray(music.Actions, disableRoarEmitter, 0);

        return true;
    }

    public static bool PatchFsm_CollectorDeathControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var blow = fsm.GetState("Blow");
        var ready = fsm.GetState("Ready");
		var end = fsm.GetState("End");
		var deathAntic = fsm.GetState("Death Antic");
		var setData = fsm.GetState("Set Data");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		setData.Actions[7].Enabled = false;
		setData.Actions[8].Enabled = false;
//		blow.Actions[1].Enabled = false;

		bool hasSceneThreats = true;

		var playMakerComp = fsm.FsmComponent;

		var coroutineObj = new GameObject("CustomCoroutine");
		var coroutineHandler = coroutineObj.AddComponent<CoroutineHandler>();

        var customActionStartCheckNoEnemiesAliveForTransition = new CustomLogicFsm(fsm);
        customActionStartCheckNoEnemiesAliveForTransition.action = (Fsm fsm) =>
        {
            IEnumerator enumerator()
            {
				string[] objectsName = {"Roller", "Buzzer", "Spitter"};

				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
            };
            coroutineHandler.StartCoroutine(enumerator());
        };

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Death Antic") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Blow") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

        deathAntic.Actions = InsertInArray(deathAntic.Actions, customActionStartCheckNoEnemiesAliveForTransition, 0);

        deathAntic.Actions = InsertInArray(deathAntic.Actions, sendEvent, deathAntic.Actions.Length);
        blow.Actions = InsertInArray(blow.Actions, sendEvent, blow.Actions.Length);
        ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }

    public static bool PatchFsm_Collector_V_DeathControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var blow = fsm.GetState("Blow");
        var ready = fsm.GetState("Ready");
		var end = fsm.GetState("End");
		var deathAntic = fsm.GetState("Death Antic");
		var setData = fsm.GetState("Set Data");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

//		blow.Actions[1].Enabled = false;

		var playMakerComp = fsm.FsmComponent;

		bool hasSceneThreats = true;

		setData.Actions[7].Enabled = false;
		setData.Actions[8].Enabled = false;

		var coroutineObj = new GameObject("CustomCoroutine");
		var coroutineHandler = coroutineObj.AddComponent<CoroutineHandler>();

        var customActionStartCheckNoEnemiesAliveForTransition = new CustomLogicFsm(fsm);
        customActionStartCheckNoEnemiesAliveForTransition.action = (Fsm fsm) =>
        {
            IEnumerator enumerator()
            {
				string[] objectsName = {"Spitter Shot", "Super Spitter", "Colosseum_Armoured"};
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
				}

				hasSceneThreats = false;
            };
            coroutineHandler.StartCoroutine(enumerator());
        };

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Death Antic") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Blow") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

        deathAntic.Actions = InsertInArray(deathAntic.Actions, customActionStartCheckNoEnemiesAliveForTransition, 0);

        deathAntic.Actions = InsertInArray(deathAntic.Actions, sendEvent, deathAntic.Actions.Length);
        blow.Actions = InsertInArray(blow.Actions, sendEvent, blow.Actions.Length);
        ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GodTamerEntryObject(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

        var wait = new Wait
        {
            time = bossRoarWait,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions[3].Enabled = false;

        roar.Actions = InsertInArray(roar.Actions, wait, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_TraitorLordControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

        var wait = new Wait
        {
            time = 0.25f,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions[5].Enabled = false;

        roar.Actions = InsertInArray(roar.Actions, wait, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_TraitorLordBattleControl(Fsm fsm)
    {
        var endPause = fsm.GetState("End Pause");

//        endPause.Actions[0].Enabled = false;

        return true;
    }
    public static bool PatchFsm_TraitorLordCorpseControl(Fsm fsm)
    {
		var mainScene = fsm.FsmComponent.gameObject.scene;
		foreach(var obj in mainScene.GetRootGameObjects()){
			if(obj.name == "mega_mantis_tall_slash") obj.name = "1mega_mantis_tall_slash";
		}

        var init = fsm.GetState("Init");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Shot Traitor Lord") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("mega_mantis_tall_slash") && obj.activeInHierarchy && obj.transform.position.x < 62.5f && obj.transform.position.x > 18f){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Shot Traitor Lord") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("mega_mantis_tall_slash") && obj.activeInHierarchy && obj.transform.position.x < 62.5f && obj.transform.position.x > 18f){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Init") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Ready") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

//		init.Actions = InsertInArray(init.Actions, waitTimer, 0);
		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);

        return true;
    }
    public static bool PatchFsm_GreyPrinceZoteCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var stun = fsm.GetState("Stun");
        var spurt = fsm.GetState("Spurt");
        var blow = fsm.GetState("Blow");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Gas Explosion") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("Shockwave Spurt") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Gas Explosion") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("Shockwave Spurt") && obj.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Stun") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Spurt") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

//        stun.Actions = InsertInArray(stun.Actions, stun.Actions[2], stun.Actions.Length);
//
//        stun.Actions[2].Enabled = false;

		init.Actions = InsertInArray(init.Actions, waitNoThreats, 0);
		init.Actions = InsertInArray(init.Actions, waitTimer, 0);
		stun.Actions = InsertInArray(stun.Actions, sendEvent, stun.Actions.Length);
		spurt.Actions = InsertInArray(spurt.Actions, sendEvent, spurt.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GreyPrinceZoteControl(Fsm fsm)
    {
        var GGPause = fsm.GetState("Stun");
        var enterShort = fsm.GetState("Enter Short");
        var enter2 = fsm.GetState("Enter 2");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("FINISHED");

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

		enter2.Transitions = new FsmTransition[]{enter2.Transitions[0]};

		roar.Actions[3].Enabled = false;
//		enterShort.Actions[4].Enabled = false;
//		enter2.Actions[5].Enabled = false;

//		enter2.Actions = InsertInArray(enter2.Actions, sendEvent, enter2.Actions.Length);
//		enterShort.Actions = InsertInArray(enterShort.Actions, sendEvent, enterShort.Actions.Length);
		roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, roarEnd.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GreyPrinceZoteTitleControl(Fsm fsm)
    {
		var getLevel = fsm.GetState("Get Level");
		var mainTitlePause = fsm.GetState("Main Title Pause");
		var mainTitle = fsm.GetState("Main Title");

		getLevel.Actions[2].Enabled = false;
		mainTitlePause.Actions[0].Enabled = false;
		mainTitle.Actions[1].Enabled = false;

		for(int i = 1; i < 14; i++){
			var state = fsm.GetState($"Extra {i}");
			if(state != null) ((Wait)state.Actions[2]).time = 0.04f;
		}

        return true;
    }

    public static bool PatchFsm_SoulWarriorControl(Fsm fsm)
    {
		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var playMakerComp = fsm.FsmComponent;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();
		IEnumerator enumerator(){
			var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
			var mainScene = fsm.FsmComponent.gameObject.scene;

			foreach1:
			foreach(var obj in dontDestroyScene.GetRootGameObjects()){
				if(obj.name.StartsWith("Mage Orb") && obj.activeInHierarchy){
					string activeState = obj.LocateMyFSM("Orb Control").ActiveStateName;
					if(activeState != "Impact"){
						yield return null;
						goto foreach1;
					}
				}
			}
			foreach(var obj in mainScene.GetRootGameObjects()){
				if(obj.name.StartsWith("Mage Orb") && obj.activeInHierarchy){
					string activeState = obj.LocateMyFSM("Orb Control").ActiveStateName;
					if(activeState != "Impact"){
						yield return null;
						goto foreach1;
					}
				}
			}

			PantheonQoL.ggBossSceneMutex = false;
			BossSceneController.Instance.EndBossScene();
		}

		var hpManager = fsm.GameObject.GetComponent<HealthManager>();
		hpManager.OnDeath += () => coroutineHandler.StartCoroutine(enumerator());

        return true;
    }
    public static bool PatchFsm_SoulMasterControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

        var wait = new Wait
        {
            time = 0.25f,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions[9].Enabled = false;

        roar.Actions = InsertInArray(roar.Actions, wait, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_SoulMasterPhase2Control(Fsm fsm)
    {
        var arrivePause = fsm.GetState("Arrive Pause");

        ((Wait)arrivePause.Actions[0]).time = 0.5f;

        return true;
    }
    public static bool PatchFsm_SoulMasterCorpse1Control(Fsm fsm)
    {
        var pause = fsm.GetState("Pause");
        var end = fsm.GetState("End");

//        ((Wait)pause.Actions[0]).time = 0.2f;
		fsm.GetFsmFloat("Wait Time").Value = 0.2f;

        return true;
    }
    public static bool PatchFsm_SoulMasterCorpse2Control(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var pause = fsm.GetState("Pause");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");
        var land = fsm.GetState("Land");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		steam.Actions[2].Enabled = false;

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("FINISHED");

		var endBossScene = new CustomLogicFsm(fsm);
		endBossScene.action += (fsm) => BossSceneController.Instance.EndBossScene();

		init.Actions = InsertInArray(init.Actions, endBossScene, init.Actions.Length);
		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		pause.Actions = InsertInArray(pause.Actions, sendEvent, pause.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }
    public static bool PatchFsm_SoulTyrantControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

        var wait = new Wait
        {
            time = 0.25f,
            finishEvent = FsmEvent.GetFsmEvent("FINISHED")
        };

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions[9].Enabled = false;

        roar.Actions = InsertInArray(roar.Actions, wait, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_SoulTyrantPhase2Control(Fsm fsm)
    {
        var wait = fsm.GetState("Wait");

        ((Wait)wait.Actions[0]).time = 1.0f;

        return true;
    }
    public static bool PatchFsm_SoulTyrantCorpse1Control(Fsm fsm)
    {
        var GGDeath = fsm.GetState("GG Death");
        var end = fsm.GetState("End");

//        ((Wait)GGDeath.Actions[0]).time = 0f;
//        ((Wait)end.Actions[1]).time = 0f;
//
//        GGDeath.Actions = InsertInArray(GGDeath.Actions, GGDeath.Actions[0], GGDeath.Actions.Length);
//
//        GGDeath.Actions[0].Enabled = false;

        return true;
    }
    public static bool PatchFsm_SoulTyrantCorpse2Control(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var pause = fsm.GetState("Pause");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");
        var land = fsm.GetState("Land");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		steam.Actions[2].Enabled = false;

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("FINISHED");

		var endBossScene = new CustomLogicFsm(fsm);
		endBossScene.action += (fsm) => BossSceneController.Instance.EndBossScene();

		init.Actions = InsertInArray(init.Actions, endBossScene, init.Actions.Length);
		init.Actions = InsertInArray(init.Actions, sendEvent, init.Actions.Length);
		pause.Actions = InsertInArray(pause.Actions, sendEvent, pause.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
		ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

        return true;
    }
    public static bool PatchFsm_DungDefenderControl(Fsm fsm)
    {
        var wake = fsm.GetState("Wake");
        var roar = fsm.GetState("Roar?");
        var roarEnd = fsm.GetState("Roar End");
        var rageRoar = fsm.GetState("Rage Roar");
        var setRage = fsm.GetState("Set Rage");

		float multiplier = 3f;

        ((Wait)roar.Actions[16]).time = 0.25f;
        ((Wait)rageRoar.Actions[9]).time = 0.5f;
		((SetFloatValue)wake.Actions[5]).floatValue.Value /= multiplier;
		fsm.GetFsmFloat("Tunnel Speed").Value *= multiplier;


        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        var customActionResetStunCounter = new CustomLogicFsm(fsm);
        customActionResetStunCounter.action += (Fsm fsm) =>
        {
            var stunFsm = fsm.GameObject.LocateMyFSM("Stun");
			stunFsm.Fsm.GetFsmInt("Combo Counter").Value = 0;
        };

        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionResetStunCounter, 0);
        setRage.Actions = InsertInArray(setRage.Actions, customActionDisableRoarEmitter, 0);
        setRage.Actions = InsertInArray(setRage.Actions, customActionResetStunCounter, 0);

        return true;
    }
    public static bool PatchFsm_DungDefenderCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var land = fsm.GetState("Land");
        var steam = fsm.GetState("Steam");
        var ready = fsm.GetState("Ready");
        var blow = fsm.GetState("Blow");
        var flyOff = fsm.GetState("Fly Off");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("FINISHED");

		var endBossScene = new CustomLogicFsm(fsm);
		endBossScene.action += (fsm) => BossSceneController.Instance.EndBossScene();

		init.Actions = InsertInArray(init.Actions, endBossScene, 0);
		init.Actions = InsertInArray(init.Actions, endBossScene, 0);

        land.Actions = InsertInArray(land.Actions, sendEvent, land.Actions.Length);
        flyOff.Actions = InsertInArray(flyOff.Actions, sendEvent, flyOff.Actions.Length);
        steam.Actions = InsertInArray(steam.Actions, sendEvent, steam.Actions.Length);
        ready.Actions = InsertInArray(ready.Actions, sendEvent, ready.Actions.Length);

//        land.Actions = InsertInArray(land.Actions, land.Actions[2], land.Actions.Length);
//        flyOff.Actions = InsertInArray(flyOff.Actions, land.Actions[3], flyOff.Actions.Length);
//
//        land.Actions[2].Enabled = false;
//        flyOff.Actions[3].Enabled = false;

        return true;
    }
    public static bool PatchFsm_WhiteDefenderControl(Fsm fsm)
    {
        var introRoar = fsm.GetState("Intro Roar");
        var music = fsm.GetState("Music");
        var roar = fsm.GetState("Roar?");
        var roarEnd = fsm.GetState("Roar End");
        var rageRoar = fsm.GetState("Rage Roar");
        var setRage = fsm.GetState("Set Rage");

        ((Wait)introRoar.Actions[19]).time = 0.25f;
        ((Wait)roar.Actions[17]).time = 0.25f;
        ((Wait)rageRoar.Actions[9]).time = 0.5f;

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        music.Actions = InsertInArray(music.Actions, customActionDisableRoarEmitter, 0);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);
        setRage.Actions = InsertInArray(setRage.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_WhiteDefenderCorpseControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var land3 = fsm.GetState("Land 3");
        var steam2 = fsm.GetState("Steam 2");
        var ready2 = fsm.GetState("Ready 2");
        var blow2 = fsm.GetState("Blow 2");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

//        ((Wait)land3.Actions[2]).time = 0f;

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("FINISHED");

        land3.Actions = InsertInArray(land3.Actions, sendEvent, land3.Actions.Length);
        steam2.Actions = InsertInArray(steam2.Actions, sendEvent, steam2.Actions.Length);
        ready2.Actions = InsertInArray(ready2.Actions, sendEvent, ready2.Actions.Length);

        return true;
    }
    public static bool PatchFsm_WatcherKnight1Control(Fsm fsm)
    {
        var bugsIn = fsm.GetState("Bugs In");
        var cloudStop = fsm.GetState("Cloud Stop");
        var bugsInEnd = fsm.GetState("Bugs In End");
        var roar = fsm.GetState("Roar");
        var music = fsm.GetState("Music?");

        float speed = 2.5f;

        ((Wait)bugsIn.Actions[3]).time.Value /= speed;
        ((Wait)cloudStop.Actions[0]).time.Value /= speed;
        ((Wait)bugsInEnd.Actions[2]).time.Value /= speed;
        ((Wait)roar.Actions[8]).time = 0.25f;

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        music.Actions = InsertInArray(music.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }
    public static bool PatchFsm_WatcherKnightBattleControl(Fsm fsm)
    {
        var pause5 = fsm.GetState("Pause 5");
        var musicEnd = fsm.GetState("Music End");
		var knight1 = fsm.GetState("Knight 1");

		float speed = 2.5f;

        ((Wait)knight1.Actions[4]).time.Value -= ( (1f - 1f / speed) * 3 + (1f - 0.25f));

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

        pause5.Actions[1].Enabled = false;
        musicEnd.Actions[0].Enabled = false;

        return true;
    }
    public static bool PatchFsm_WatcherKnight1CorpseControl(Fsm fsm)
    {
        var battleControl = GameObject.Find("Battle Control");
        var battleFsm = battleControl.GetComponent<PlayMakerFSM>();
        if(battleFsm.Fsm.GetFsmInt("Battle Enemies").Value > 0) return false;

        var roar = fsm.GetState("Roar");
        var bugsOut = fsm.GetState("Bugs Out");

        ((Wait)roar.Actions[9]).time = 0f;
        ((Wait)bugsOut.Actions[2]).time = 0f;

        roar.Actions = InsertInArray(roar.Actions, roar.Actions[9], roar.Actions.Length);
        bugsOut.Actions = InsertInArray(bugsOut.Actions, bugsOut.Actions[2], bugsOut.Actions.Length);

        roar.Actions[9].Enabled = false;
        bugsOut.Actions[2].Enabled = false;

        return true;
    }
    public static bool PatchFsm_NoEyesCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool canDoTransition = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				canDoTransition = true;
				
				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		music.Actions = InsertInArray(music.Actions, waitTimer, 0);
		music.Actions = InsertInArray(music.Actions, waitOnGround, 0);
		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MarmuCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool canDoTransition = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_Marmu_V_CorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool canDoTransition = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				canDoTransition = true;
				
				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		music.Actions = InsertInArray(music.Actions, waitTimer, 0);
		music.Actions = InsertInArray(music.Actions, waitOnGround, 0);
		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_XeroCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool canDoTransition = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				canDoTransition = true;
				
				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		music.Actions = InsertInArray(music.Actions, waitTimer, 0);
		music.Actions = InsertInArray(music.Actions, waitOnGround, 0);
		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_MarkothCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool canDoTransition = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_Markoth_V_CorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool canDoTransition = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				canDoTransition = true;
				
				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		music.Actions = InsertInArray(music.Actions, waitTimer, 0);
		music.Actions = InsertInArray(music.Actions, waitOnGround, 0);
		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GalienCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool canDoTransition = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_GorbCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		bool isBossSceneEnded = false;
		bool canDoTransition = false;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var sendEventOnCondition = new CustomLogicFsm(fsm);
		sendEventOnCondition.action += (fsm) => {
			IEnumerator enumerator(){
				while(!canDoTransition) yield return null;
				fsm.FsmComponent.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f) break;
					yield return null;
				}

				canDoTransition = true;
				
				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		music.Actions = InsertInArray(music.Actions, waitTimer, 0);
		music.Actions = InsertInArray(music.Actions, waitOnGround, 0);
		pause.Actions = InsertInArray(pause.Actions, sendEventOnCondition, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, sendEventOnCondition, blow.Actions.Length);

        return true;
    }
    public static bool PatchFsm_ElderHuCorpseControl(Fsm fsm)
    {
        var music = fsm.GetState("Music");
        var pause = fsm.GetState("Pause");
        var blow = fsm.GetState("Blow");
        var end = fsm.GetState("End");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;
		
		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				Transform ringHolder = null;

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Ring Holder") && obj.activeInHierarchy){
						ringHolder = obj.transform;
					}
				}

				if(ringHolder == null) yield break;

				foreach1:
				foreach(Transform child in ringHolder){
					if(child.gameObject.activeInHierarchy){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		pause.Actions = InsertInArray(pause.Actions, doSkipStates, pause.Actions.Length);
		blow.Actions = InsertInArray(blow.Actions, doSkipStates, blow.Actions.Length);
		music.Actions = InsertInArray(music.Actions, waitTimer, 0);
		music.Actions = InsertInArray(music.Actions, waitNoThreats, 0);

        return true;
    }
    public static bool PatchFsm_OroAndMatoControl(Fsm fsm)
    {
        var rest = fsm.GetState("Rest");
        var wake = fsm.GetState("Wake");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");
        var entryRoar = fsm.GetState("Entry Roar");
        var roarEnd2 = fsm.GetState("Roar End 2");
        var deathLand = fsm.GetState("Death Land");
        var deathStart = fsm.GetState("Death Start");
        var defeated = fsm.GetState("Defeated");
        var bow = fsm.GetState("Bow");

        ((Wait)rest.Actions[1]).time = 1.5f;
        ((Wait)wake.Actions[1]).time = 0.5f;
//        wake.Actions[2].Enabled = false;
        ((Wait)roar.Actions[13]).time = 0.25f;
        ((Wait)entryRoar.Actions[14]).time = 0.5f;

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        var customActionDisableIfDoubleBrothers = new CustomLogicFsm(fsm);
        customActionDisableIfDoubleBrothers.action += (Fsm fsm) =>
        {
            if(fsm.GetFsmBool("Phase 2").Value) ((DecelerateXY)deathLand.Actions[1]).decelerationX = 0f;
        };

        wake.Actions = InsertInArray(wake.Actions, customActionDisableRoarEmitter, wake.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);
        roarEnd2.Actions = InsertInArray(roarEnd2.Actions, customActionDisableRoarEmitter, 0);

		if(PantheonQoL.disFastDeathForAll) return true;

        var sendEventIfPhase2 = new CustomLogicFsm(fsm);
        sendEventIfPhase2.action += (Fsm fsm) =>
        {
            if(fsm.GetFsmBool("Phase 2").Value){
				fsm.Event(((SendEventByName)defeated.Actions[0]).eventTarget, ((SendEventByName)defeated.Actions[0]).sendEvent.Value);
			}
        };

		defeated.Actions[0].Enabled = false;
        ((Wait)deathLand.Actions[4]).time = 0.1f;
        ((Wait)bow.Actions[4]).time = 0f;
        deathLand.Actions = InsertInArray(deathLand.Actions, customActionDisableIfDoubleBrothers, 0);

        deathStart.Actions = InsertInArray(deathStart.Actions, sendEventIfPhase2, 0);

        return true;
    }
    public static bool PatchFsm_OroAndMatoComboControl(Fsm fsm)
    {
        var defeated2 = fsm.GetState("Defeated 2");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		defeated2.Actions[0].Enabled = false;

        var endBossScene = new CustomLogicFsm(fsm);
        endBossScene.action += (Fsm fsm) =>
        {
			BossSceneController.Instance.EndBossScene();
        };

        defeated2.Actions = InsertInArray(defeated2.Actions, endBossScene, 0);

        return true;
    }
    public static bool PatchFsm_SheoControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var painting = fsm.GetState("Painting");
        var look = fsm.GetState("Look");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");

		((Wait)painting.Actions[0]).time = 1.5f;

		var wait1 = new Wait{
			time = 0.5f,
			finishEvent = new FsmEvent("FINISHED")
		};

		var wait2 = new Wait{
			time = 0.25f,
			finishEvent = new FsmEvent("FINISHED")
		};

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        look.Actions = InsertInArray(look.Actions, wait1, look.Actions.Length);
        roar.Actions = InsertInArray(roar.Actions, wait2, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);

        look.Actions[1].Enabled = false;
        roar.Actions[10].Enabled = false;

        return true;
    }
    public static bool PatchFsm_SheoCorpseControl(Fsm fsm)
    {
        var deathLaunch = fsm.GetState("Death Launch");
        var deathAir = fsm.GetState("Death Air");
        var deathLand = fsm.GetState("Death Land");
        var bow = fsm.GetState("Bow");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		((DecelerateXY)deathLand.Actions[0]).decelerationX = 0f;

		PantheonQoL.ggBossSceneMutex = true;
		
		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Paint Shot") && obj.activeInHierarchy && obj.GetComponent<BoxCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("Paint Shot") && obj.activeInHierarchy && obj.GetComponent<BoxCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				playMakerComp.SendEvent("END");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		deathLand.Actions = InsertInArray(deathLand.Actions, doSkipStates, deathLand.Actions.Length);
		deathLaunch.Actions = InsertInArray(deathLaunch.Actions, waitTimer, 0);
		deathLaunch.Actions = InsertInArray(deathLaunch.Actions, waitNoThreats, 0);

        return true;
    }

    public static bool PatchFsm_SlyControl(Fsm fsm)
    {
        var init = fsm.GetState("Init");
        var docile = fsm.GetState("Docile");
        var call = fsm.GetState("Call");
        var roar = fsm.GetState("Roar");
        var roarEnd = fsm.GetState("Roar End");
        var airRoar = fsm.GetState("Air Roar");
        var explosion = fsm.GetState("Explosion");
        var deathLand = fsm.GetState("Death Land");
        var bow = fsm.GetState("Bow");

		float speed = 3f;

        ((Wait)docile.Actions[0]).time = 1f;

		var wait1 = new Wait{
			time = 0.5f,
			finishEvent = new FsmEvent("FINISHED")
		};

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("END");

		var wait2 = new Wait{
			time = ((Wait)airRoar.Actions[7]).time.Value / speed,
			finishEvent = new FsmEvent("FINISHED")
		};

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

        roar.Actions = InsertInArray(roar.Actions, wait1, roar.Actions.Length);
        roarEnd.Actions = InsertInArray(roarEnd.Actions, customActionDisableRoarEmitter, 0);
        airRoar.Actions = InsertInArray(airRoar.Actions, wait2, airRoar.Actions.Length);

        roar.Actions[10].Enabled = false;
        airRoar.Actions[7].Enabled = false;

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

        var endBossScene = new CustomLogicFsm(fsm);
        endBossScene.action += (Fsm fsm) =>
        {
			BossSceneController.Instance.EndBossScene();
        };

        deathLand.Actions[4].Enabled = false;
        bow.Actions[2].Enabled = false;
        deathLand.Actions = InsertInArray(deathLand.Actions, sendEvent, deathLand.Actions.Length);

        explosion.Actions = InsertInArray(explosion.Actions, endBossScene, 0);

        return true;
    }

    public static bool PatchFsm_PureVesselControl(Fsm fsm)
    {
		var intro1 = fsm.GetState("Intro 1");
		var intro2 = fsm.GetState("Intro 2");
		var intro3 = fsm.GetState("Intro 3");
		var introRoar = fsm.GetState("Intro Roar");
		var introRoarEnd = fsm.GetState("Intro Roar End");

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		intro1.Actions[0].Enabled = false;
		intro2.Actions[3].Enabled = false;
		introRoar.Actions[7].Enabled = false;

		((Wait)intro3.Actions[9]).time = 1f;

        var customActionDisableRoarEmitter = new CustomLogicFsm(fsm);
        customActionDisableRoarEmitter.action += (Fsm fsm) =>
        {
            fsm.Variables.GetFsmGameObject("Roar Emitter").Value.SetActive(false);
        };

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("END");

		var waitRoar = new CustomLogicFsm(fsm);
		waitRoar.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(0.25f);

				fsm.FsmComponent.SendEvent("END");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		intro2.Actions = InsertInArray(intro2.Actions, sendEvent, intro2.Actions.Length);
		introRoar.Actions = InsertInArray(introRoar.Actions, waitRoar, introRoar.Actions.Length);
		introRoarEnd.Actions = InsertInArray(introRoarEnd.Actions, customActionDisableRoarEmitter, 0);

        return true;
    }

    public static bool PatchFsm_PureVesselCorpseControl(Fsm fsm)
    {
		var music = fsm.GetState("Music");
		var land = fsm.GetState("Land");

		var fizzle = fsm.GetState("Fizzle");
		var fade = fsm.GetState("Fade");
		var endPause1 = fsm.GetState("End Pause 1");
		var endPause = fsm.GetState("End Pause");
		var endScene = fsm.GetState("End Scene");

		var extraPause = fsm.GetState("Extra Pause");
		var lookUp = fsm.GetState("Look Up");
		var rumbleStart = fsm.GetState("Rumble Start");
		var beamOn = fsm.GetState("Beam On");

		if(PantheonQoL.disFastDeathForAll) return true;
//		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;

		var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
		var mainScene = fsm.FsmComponent.gameObject.scene;

		GameObject battleScene = fsm.GameObject.transform.parent.parent.gameObject;
		
		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					if(obj.name.StartsWith("HK Plume Prime") && obj.activeInHierarchy){
						var plumeFsm = obj.LocateMyFSM("FSM");
						if(plumeFsm != null){
							var name = plumeFsm.ActiveStateName;
							if(name == "End" || name == "Auto?" || name == "Deactivate" || name == "Recycle"){
								continue;
							}
							else{
								yield return null;
								goto foreach1;
							}
						}
					}
					if(obj.name.StartsWith("Shot HK Shadow") && obj.activeInHierarchy && obj.transform.position.x > 24f && obj.transform.position.x < 66f && obj.transform.position.y > 0f && obj.transform.position.y < 25f){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					if(obj.name.StartsWith("HK Plume Prime") && obj.activeInHierarchy){
						var plumeFsm = obj.LocateMyFSM("FSM");
						if(plumeFsm != null){
							var name = plumeFsm.ActiveStateName;
							if(name == "End" || name == "Auto?" || name == "Deactivate" || name == "Recycle"){
								continue;
							}
							else{
								yield return null;
								goto foreach1;
							}
						}
					}
					if(obj.name.StartsWith("Shot HK Shadow") && obj.activeInHierarchy && obj.transform.position.x > 24f && obj.transform.position.x < 66f && obj.transform.position.y > 0f && obj.transform.position.y < 25f){
						yield return null;
						goto foreach1;
					}
				}

				if(battleScene != null){
					var focusBlasts = battleScene.transform.Find("Focus Blasts");
					if(focusBlasts != null){
						foreach(Transform focusBlast in focusBlasts){
							var blastFsm = focusBlast.gameObject.LocateMyFSM("Control");
							if(blastFsm != null && blastFsm.ActiveStateName != "Init" && blastFsm.ActiveStateName != "Idle"){
								yield return null;
								goto foreach1;
							}
						}
					}
				}

				hasSceneThreats = false;

//				if(isBossSceneEnded) yield break;
//				isBossSceneEnded = true;
//
//				PantheonQoL.ggBossSceneMutex = false;
//				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats) yield return null;

				if(playMakerComp.ActiveStateName == "Land") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Fizzle") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "End Pause") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "End Pause 1") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Extra Pause") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Look Up") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Rumble Start") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Beam On") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		music.Actions = InsertInArray(music.Actions, waitNoThreats, 0);

		land.Actions = InsertInArray(land.Actions, doSkipStates, land.Actions.Length);
		fizzle.Actions = InsertInArray(fizzle.Actions, doSkipStates, fizzle.Actions.Length);
		endPause.Actions = InsertInArray(endPause.Actions, doSkipStates, endPause.Actions.Length);
		endPause1.Actions = InsertInArray(endPause1.Actions, doSkipStates, endPause1.Actions.Length);
		endScene.Actions = InsertInArray(endScene.Actions, doSkipStates, endScene.Actions.Length);
		extraPause.Actions = InsertInArray(extraPause.Actions, doSkipStates, extraPause.Actions.Length);
		lookUp.Actions = InsertInArray(lookUp.Actions, doSkipStates, lookUp.Actions.Length);
		rumbleStart.Actions = InsertInArray(rumbleStart.Actions, doSkipStates, rumbleStart.Actions.Length);
		beamOn.Actions = InsertInArray(beamOn.Actions, doSkipStates, beamOn.Actions.Length);

//		((Wait)land.Actions[1]).time = 2f;
//		fizzle.Actions[3].Enabled = false;
//		endPause1.Actions[0].Enabled = false;
//		endPause.Actions[0].Enabled = false;
		endScene.Actions[1].Enabled = false;
//
//		extraPause.Actions[0].Enabled = false;
//		lookUp.Actions[0].Enabled = false;
//		rumbleStart.Actions[3].Enabled = false;
//		((AudioPlayerOneShotSingle)beamOn.Actions[2]).Enabled = false;

        return true;
    }

    public static bool PatchFsm_GrimmBossControl(Fsm fsm)
    {
        var deathStart = fsm.GetState("Death Start");
        var steam = fsm.GetState("Steam");

		if(PantheonQoL.disFastDeathForAll) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;
		
		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Grimm UP Ball", "Flameball"};

//				PlayMakerFSM spikeHolder = null;
//				foreach(var obj in mainScene.GetRootGameObjects()){
//					if(obj.name == "Grimm Spike Holder"){
//						spikeHolder = obj.LocateMyFSM("Spike Control");
//					}
//				}

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
					if(obj.name.StartsWith("Grimm Firebat") && obj.activeInHierarchy){
						var fsmComp = obj.LocateMyFSM("Control");
						if(fsmComp != null && fsmComp.ActiveStateName == "Fire" || fsmComp.ActiveStateName == "Init"){
							yield return null;
							goto foreach1;
						}
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
					if(obj.name.StartsWith("Grimm Firebat") && obj.activeInHierarchy){
						var fsmComp = obj.LocateMyFSM("Control");
						if(fsmComp != null && fsmComp.ActiveStateName == "Fire" || fsmComp.ActiveStateName == "Init"){
							yield return null;
							goto foreach1;
						}
					}
				}

//				if(spikeHolder != null && spikeHolder.ActiveStateName != "Idle" && spikeHolder.ActiveStateName != "Init"){
//					yield return null;
//					goto foreach1;
//				}

				hasSceneThreats = false;

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats || !isBossSceneEnded) yield return null;

				if(playMakerComp.ActiveStateName == "Death Start") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		deathStart.Actions = InsertInArray(deathStart.Actions, waitTimer, 0);
		deathStart.Actions = InsertInArray(deathStart.Actions, waitNoThreats, 0);

		deathStart.Actions = InsertInArray(deathStart.Actions, doSkipStates, deathStart.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, doSkipStates, steam.Actions.Length);

        return true;
    }

    public static bool PatchFsm_GrimmNightmareBossControl(Fsm fsm)
    {
        var deathStart = fsm.GetState("Death Start");
        var steam = fsm.GetState("Steam");

		if(PantheonQoL.disFastDeathForAll || PantheonQoL.disFastDeathForPermaThreat) return true;
		PantheonQoL.ggBossSceneMutex = true;
		float origBossDeadWait = BossSceneController.Instance.bossesDeadWaitTime;
		BossSceneController.Instance.bossesDeadWaitTime = 0f;
		
		var playMakerComp = fsm.FsmComponent;

		bool isBossSceneEnded = false;
		bool hasSceneThreats = true;

		var coroutineHandlerObj = new GameObject("CoroutineHandler1");
		var coroutineHandler = coroutineHandlerObj.AddComponent<CoroutineHandler>();

		var waitNoThreats = new CustomLogicFsm(fsm);
		waitNoThreats.action += (fsm) => {
			IEnumerator enumerator(){
				var dontDestroyScene = PantheonQoL.instance.gameObject.scene;
				var mainScene = fsm.FsmComponent.gameObject.scene;

				var objectsName = new string[]{"Nightmare UP Ball", "Flame Trail", "Grimm_flare_pillar"};

				foreach1:
				foreach(var obj in dontDestroyScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
					if(obj.name.StartsWith("Nightmare Firebat") && obj.activeInHierarchy && obj.transform.position.x < 125f && obj.transform.position.x > 35f){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("Flameball") && obj.activeInHierarchy && obj.transform.position.x < 125f && obj.transform.position.x > 35f && obj.GetComponent<CircleCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				foreach(var obj in mainScene.GetRootGameObjects()){
					foreach(string str in objectsName){
						if(obj.name.StartsWith(str) && obj.activeInHierarchy){
							yield return null;
							goto foreach1;
						}
					}
					if(obj.name.StartsWith("Nightmare Firebat") && obj.activeInHierarchy && obj.transform.position.x < 125f && obj.transform.position.x > 35f){
						yield return null;
						goto foreach1;
					}
					if(obj.name.StartsWith("Flameball") && obj.activeInHierarchy && obj.transform.position.x < 125f && obj.transform.position.x > 35f && obj.GetComponent<CircleCollider2D>().enabled){
						yield return null;
						goto foreach1;
					}
				}

				hasSceneThreats = false;
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitTimer = new CustomLogicFsm(fsm);
		waitTimer.action += (fsm) => {
			IEnumerator enumerator(){
				yield return new WaitForSeconds(origBossDeadWait);

				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var waitOnGround = new CustomLogicFsm(fsm);
		waitOnGround.action += (fsm) => {
			IEnumerator enumerator(){
				float timer = 0;
				while(true){
					if(HeroController.instance.cState.onGround) timer += Time.deltaTime;
					else timer = 0;
					if(timer > 0.5f && !hasSceneThreats) break;
					yield return null;
				}
				if(isBossSceneEnded) yield break;
				isBossSceneEnded = true;

				PantheonQoL.ggBossSceneMutex = false;
				BossSceneController.Instance.EndBossScene();
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

		var doSkipStates = new CustomLogicFsm(fsm);
		doSkipStates.action += (fsm) => {
			IEnumerator enumerator(){
				while(hasSceneThreats || !isBossSceneEnded) yield return null;

				if(playMakerComp.ActiveStateName == "Death Start") playMakerComp.SendEvent("FINISHED");
				if(playMakerComp.ActiveStateName == "Steam") playMakerComp.SendEvent("FINISHED");
			}
			coroutineHandler.StartCoroutine(enumerator());
		};

//		deathStart.Actions = InsertInArray(deathStart.Actions, waitTimer, 0);
		deathStart.Actions = InsertInArray(deathStart.Actions, waitNoThreats, 0);
		deathStart.Actions = InsertInArray(deathStart.Actions, waitOnGround, 0);

		deathStart.Actions = InsertInArray(deathStart.Actions, doSkipStates, deathStart.Actions.Length);
		steam.Actions = InsertInArray(steam.Actions, doSkipStates, steam.Actions.Length);

        return true;
    }

    public static bool PatchFsm_GrimmNightmareGrimmControl(Fsm fsm)
    {
		var panOver2 = fsm.GetState("Pan Over 2");
		var eye3 = fsm.GetState("Eye 3");
		var eye4 = fsm.GetState("Eye 4");
		var burst = fsm.GetState("Burst");

		panOver2.Actions[4].Enabled = false;
		eye3.Actions[1].Enabled = false;
		eye4.Actions[4].Enabled = false;
		burst.Actions[0].Enabled = false;

		var sendEvent = new CustomLogicFsm(fsm);
		sendEvent.action += (fsm) => fsm.FsmComponent.SendEvent("FINISHED");

		eye3.Actions = InsertInArray(eye3.Actions, sendEvent, eye3.Actions.Length);
		eye4.Actions = InsertInArray(eye4.Actions, sendEvent, eye4.Actions.Length);
		burst.Actions = InsertInArray(burst.Actions, sendEvent, burst.Actions.Length);

        return true;
    }

    public static bool PatchFsm_AbsRadianceBossControl(Fsm fsm)
    {
		var setup = fsm.GetState("Setup");
		var titleUp = fsm.GetState("Title Up");
		var flashDown = fsm.GetState("Flash Down");

		setup.Actions[6].Enabled = false;

		fsm.GetFsmGameObject("Feather Particles").Value.SetActive(false);
		fsm.GetFsmGameObject("Sun").Value.SetActive(false);

		SetTransitionToState(setup, flashDown, 0);

//		flashDown.Actions = InsertInArray(flashDown.Actions, titleUp.Actions[3], 0);
//		flashDown.Actions = InsertInArray(flashDown.Actions, titleUp.Actions[2], 0);
//		flashDown.Actions = InsertInArray(flashDown.Actions, titleUp.Actions[0], 0);

        return true;
    }
}

using EA4S.Audio;
using EA4S.Core;
using EA4S.UI;
using UnityEngine;
using System.Collections.Generic;
using EA4S.MinigamesCommon;

namespace EA4S.AnturaSpace
{
    /// <summary>
    /// Manages the AnturaSpace scene.
    /// </summary>
    // refactor: group this class with other scene managers
    public class AnturaSpaceManager : MonoBehaviour
    {
        const int MaxBonesInScene = 5;

        public AnturaLocomotion Antura;
        public AnturaSpaceUI UI;
        public AnturaSpaceTutorial Tutorial;

        public Transform SceneCenter;
        public Pedestal RotatingBase;
        public Transform AttentionPosition;
        public Transform BoneSpawnPosition;
        public GameObject BonePrefab;
        public GameObject PoofPrefab;

        public Transform DraggingBone { get; private set; }
        public Transform NextBoneToCatch
        {
            get
            {
                if (bones.Count == 0)
                    return null;
                return bones[0].transform;
            }
        }
        List<GameObject> bones = new List<GameObject>();

        public readonly AnturaIdleState Idle;
        public readonly AnturaCustomizationState Customization;
        public readonly AnturaDrawingAttentionState DrawingAttention;
        public readonly AnturaAnimateState Animate;
        public readonly AnturaSleepingState Sleeping;
        public readonly AnturaWaitingThrowState WaitingThrow;
        public readonly AnturaCatchingState Catching;

        StateManager stateManager = new StateManager();
        public AnturaState CurrentState
        {
            get
            {
                return (AnturaState)stateManager.CurrentState;
            }
            set
            {
                stateManager.CurrentState = value;
            }
        }

        public bool HasPlayerBones
        {
            get
            {
                var totalBones = AppManager.I.Player.GetTotalNumberOfBones();

                return totalBones > 0;
            }
        }

        public float AnturaHappiness { get; private set; }

        public void ThrowBone()
        {
            if (DraggingBone != null)
                return;

            if (bones.Count < MaxBonesInScene && AppManager.I.Player.TotalNumberOfBones > 0)
            {
                var bone = Instantiate(BonePrefab);
                bone.SetActive(true);
                bone.transform.position = BoneSpawnPosition.position;
                bone.GetComponent<BoneBehaviour>().SimpleThrow();
                bones.Add(bone);
                --AppManager.I.Player.TotalNumberOfBones;
            }
        }

        public bool InCustomizationMode { get; private set; }
        public float LastTimeCatching { get; set; }

        /// <summary>
        /// Drag a bone around.
        /// </summary>
        public void DragBone()
        {
            if (DraggingBone != null)
                return;

            if (bones.Count < MaxBonesInScene && AppManager.I.Player.TotalNumberOfBones > 0)
            {
                var bone = Instantiate(BonePrefab);
                bone.SetActive(true);
                bone.transform.position = BoneSpawnPosition.position;
                DraggingBone = bone.transform;
                bones.Add(bone);
                --AppManager.I.Player.TotalNumberOfBones;
                bone.GetComponent<BoneBehaviour>().Drag();
            }
        }

        public void EatBone(GameObject bone)
        {
            if (bones.Remove(bone))
            {
                AudioManager.I.PlaySound(Sfx.EggMove);
                var poof = Instantiate(PoofPrefab).transform;
                poof.position = bone.transform.position;

                foreach (var ps in poof.GetComponentsInChildren<ParticleSystem>())
                {
                    var main = ps.main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }

                poof.localScale = poof.localScale * 0.5f;
                poof.gameObject.AddComponent<AutoDestroy>().duration = 2;
                AudioManager.I.PlaySound(Sfx.Poof);
                AnturaHappiness += 0.2f;
                if (AnturaHappiness > 1)
                    AnturaHappiness = 1;

                Destroy(bone);
            }
        }

        public AnturaSpaceManager()
        {
            Idle = new AnturaIdleState(this);
            Customization = new AnturaCustomizationState(this);
            DrawingAttention = new AnturaDrawingAttentionState(this);
            Sleeping = new AnturaSleepingState(this);
            WaitingThrow = new AnturaWaitingThrowState(this);
            Catching = new AnturaCatchingState(this);
            Animate = new AnturaAnimateState(this);
        }

        void Awake()
        {
            UI.onEnterCustomization += OnEnterCustomization;
            UI.onExitCustomization += OnExitCustomization;

            Antura.onTouched += () => { if (CurrentState != null) CurrentState.OnTouched(); };

            LastTimeCatching = Time.realtimeSinceStartup;
        }

        public void Update()
        {
            AnturaHappiness -= Time.deltaTime/40.0f;
            if (AnturaHappiness < 0)
                AnturaHappiness = 0;

            stateManager.Update(Time.deltaTime);

            if (!Tutorial.IsRunning)
                UI.ShowBonesButton(bones.Count < MaxBonesInScene);

            UI.BonesCount = AppManager.I.Player.GetTotalNumberOfBones();

            if (DraggingBone != null && !Input.GetMouseButton(0))
            {
                DraggingBone.GetComponent<BoneBehaviour>().LetGo();
                DraggingBone = null;
            }
        }

        public void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }

        public Music backgroundMusic;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);

            if (!AppManager.I.Player.IsFirstContact())
            {
                ShowBackButton();
            }

            AudioManager.I.PlayMusic(backgroundMusic);
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "enter");

            CurrentState = Idle;
        }

        public void ShowBackButton()
        {
            GlobalUI.ShowBackButton(true, OnExit);
        }

        void OnExit()
        {
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "exit");
            AppManager.I.NavigationManager.GoBack();
        }

        void OnEnterCustomization()
        {
            InCustomizationMode = true;
            CurrentState = Customization;
        }

        void OnExitCustomization()
        {
            InCustomizationMode = false;
            CurrentState = Idle;
        }
    }
}

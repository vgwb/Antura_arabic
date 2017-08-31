using Antura.AnturaSpace.UI;
using Antura.Audio;
using Antura.Core;
using Antura.Minigames;
using Antura.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.AnturaSpace
{
    /// <summary>
    /// Manages the AnturaSpace scene.
    /// </summary>
    public class AnturaSpaceScene : SceneBase
    {
        private const int MaxBonesInScene = 5;

        [Header("References")]
        public AnturaLocomotion Antura;

        public AnturaSpaceUI UI;
        public AnturaSpaceTutorial Tutorial;

        public Transform SceneCenter;
        public Pedestal RotatingBase;
        public Transform AttentionPosition;
        public Transform BoneSpawnPosition;
        public GameObject BonePrefab;
        public GameObject PoofPrefab;

        public bool MustShowBonesButton { get; set; }
        public Transform DraggingBone { get; private set; }

        public Transform NextBoneToCatch {
            get {
                if (bones.Count == 0) {
                    return null;
                }
                return bones[0].transform;
            }
        }

        private List<GameObject> bones = new List<GameObject>();

        public AnturaIdleState Idle;
        public AnturaCustomizationState Customization;
        public AnturaDrawingAttentionState DrawingAttention;
        public AnturaAnimateState Animate;
        public AnturaSleepingState Sleeping;
        public AnturaWaitingThrowState WaitingThrow;
        public AnturaCatchingState Catching;

        private StateManager stateManager = new StateManager();

        public AnturaState CurrentState {
            get { return (AnturaState)stateManager.CurrentState; }
            set { stateManager.CurrentState = value; }
        }

        public bool HasPlayerBones {
            get {
                var totalBones = AppManager.I.Player.GetTotalNumberOfBones();

                return totalBones > 0;
            }
        }

        public float AnturaHappiness { get; private set; }
        public bool InCustomizationMode { get; private set; }
        public float LastTimeCatching { get; set; }

        protected override void Init()
        {
            InitStates();

            UI.onEnterCustomization += OnEnterCustomization;
            UI.onExitCustomization += OnExitCustomization;

            Antura.onTouched += () => {
                if (CurrentState != null) {
                    CurrentState.OnTouched();
                }

                if (CurrentState == Customization) {
                    UI.ToggleModsPanel();
                }
            };

            LastTimeCatching = Time.realtimeSinceStartup;
        }

        public void InitStates()
        {
            Idle = new AnturaIdleState(this);
            Customization = new AnturaCustomizationState(this);
            DrawingAttention = new AnturaDrawingAttentionState(this);
            Sleeping = new AnturaSleepingState(this);
            WaitingThrow = new AnturaWaitingThrowState(this);
            Catching = new AnturaCatchingState(this);
            Animate = new AnturaAnimateState(this);
        }

        protected override void Start()
        {
            base.Start();

            GlobalUI.ShowPauseMenu(false);

            if (!AppManager.I.Player.IsFirstContact()) {
                ShowBackButton();
            }

            CurrentState = Idle;
        }

        public void Update()
        {
            AnturaHappiness -= Time.deltaTime / 40.0f;
            if (AnturaHappiness < 0) {
                AnturaHappiness = 0;
            }

            stateManager.Update(Time.deltaTime);

            if (!Tutorial.IsRunning) {
                UI.ShowBonesButton(MustShowBonesButton && (bones.Count < MaxBonesInScene));
            }

            UI.BonesCount = AppManager.I.Player.GetTotalNumberOfBones();

            if (DraggingBone != null && !Input.GetMouseButton(0)) {
                AudioManager.I.PlaySound(Sfx.ThrowObj);
                DraggingBone.GetComponent<BoneBehaviour>().LetGo();
                DraggingBone = null;
            }
        }

        public void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }

        public void ShowBackButton()
        {
            GlobalUI.ShowBackButton(true, OnExit);
        }

        void OnExit()
        {
            AppManager.I.NavigationManager.GoBack();
        }

        void OnEnterCustomization()
        {
            GlobalUI.ShowBackButton(false);
            ShowBackButton();
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            InCustomizationMode = true;
            CurrentState = Customization;
        }

        void OnExitCustomization()
        {
            GlobalUI.ShowBackButton(false);
            ShowBackButton();
            AudioManager.I.PlaySound(Sfx.UIButtonClick);
            InCustomizationMode = false;
            CurrentState = Idle;
        }

        #region bones actions

        public void ThrowBone()
        {
            if (DraggingBone != null) {
                return;
            }

            if (bones.Count < MaxBonesInScene && AppManager.I.Player.TotalNumberOfBones > 0) {
                AudioManager.I.PlaySound(Sfx.ThrowObj);
                var bone = Instantiate(BonePrefab);
                bone.SetActive(true);
                bone.transform.position = BoneSpawnPosition.position;
                bone.GetComponent<BoneBehaviour>().SimpleThrow();
                bones.Add(bone);
                --AppManager.I.Player.TotalNumberOfBones;
            } else {
                AudioManager.I.PlaySound(Sfx.KO);
            }
        }


        /// <summary>
        /// Drag a bone around.
        /// </summary>
        public void DragBone()
        {
            if (DraggingBone != null)
                return;

            if (bones.Count <= MaxBonesInScene && AppManager.I.Player.TotalNumberOfBones > 0) {
                var bone = Instantiate(BonePrefab);
                bone.SetActive(true);
                bone.transform.position = BoneSpawnPosition.position;
                DraggingBone = bone.transform;
                bones.Add(bone);
                --AppManager.I.Player.TotalNumberOfBones;
                bone.GetComponent<BoneBehaviour>().Drag();
            } else {
                AudioManager.I.PlaySound(Sfx.KO);
            }
        }

        public void EatBone(GameObject bone)
        {
            if (bones.Remove(bone)) {
                AudioManager.I.PlaySound(Sfx.EggMove);
                var poof = Instantiate(PoofPrefab).transform;
                poof.position = bone.transform.position;

                foreach (var ps in poof.GetComponentsInChildren<ParticleSystem>()) {
                    var main = ps.main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }

                poof.localScale = poof.localScale * 0.5f;
                poof.gameObject.AddComponent<AutoDestroy>().duration = 2;
                AudioManager.I.PlaySound(Sfx.Poof);
                AnturaHappiness += 0.2f;
                if (AnturaHappiness > 1) {
                    AnturaHappiness = 1;
                }

                Destroy(bone);
            }
        }

        #endregion
    }
}
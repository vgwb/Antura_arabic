using Antura.AnturaSpace.UI;
using Antura.Audio;
using Antura.Core;
using Antura.FSM;
using Antura.Minigames;
using Antura.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Antura.AnturaSpace
{
    /// <summary>
    /// Manages the AnturaSpace scene.
    /// </summary>
    public class AnturaSpaceScene : SceneBase
    {
        private const int MaxSpawnedObjectsInScene = 5;

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

        public ThrowableObject NextObjectToCatch {
            get
            {
                var nextObject = spawnedObjects.FirstOrDefault(x => x.Catchable);
                if (nextObject == null) return null;
                return nextObject;
            }
        }

        public AnturaIdleState Idle;
        public AnturaCustomizationState Customization;
        public AnturaDrawingAttentionState DrawingAttention;
        public AnturaAnimateState Animate;
        public AnturaSleepingState Sleeping;
        public AnturaWaitingThrowState WaitingThrow;
        public AnturaCatchingState Catching;

        private StateMachineManager stateManager = new StateMachineManager();

        public AnturaState CurrentState {
            get { return (AnturaState)stateManager.CurrentState; }
            set { stateManager.CurrentState = value; }
        }

        public bool HasPlayerBones {
            get {
                return AppManager.I.Player.GetTotalNumberOfBones() > 0;
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

            // TODO: the tutorial needs to change to accommodate the shop!
            if (!Tutorial.IsRunning) {
                UI.ShowBonesButton(MustShowBonesButton && (spawnedObjects.Count < MaxSpawnedObjectsInScene));
            }

            UI.BonesCount = AppManager.I.Player.GetTotalNumberOfBones();

            if (DraggingBone != null && !Input.GetMouseButton(0)) {
                AudioManager.I.PlaySound(Sfx.ThrowObj);
                DraggingBone.GetComponent<ThrowableObject>().LetGo();
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

        
        #region Throwable actions

        public Transform DraggedTransform { get; private set; }
        public Transform ObjectSpawnPivotTr;
        private List<ThrowableObject> spawnedObjects = new List<ThrowableObject>();

        public void ThrowObject(ThrowableObject ObjectPrefab)
        {
            if (DraggedTransform != null)
                return;

            if (AppManager.I.Player.GetTotalNumberOfBones() > 0 && spawnedObjects.Count < MaxSpawnedObjectsInScene)
            {
                AppManager.I.Player.RemoveBones(1);

                AudioManager.I.PlaySound(Sfx.ThrowObj);
                GameObject newObjectGo = Instantiate(ObjectPrefab.gameObject);
                newObjectGo.SetActive(true);
                newObjectGo.transform.position = ObjectSpawnPivotTr.position;
                var throwableObject = newObjectGo.GetComponent<ThrowableObject>();
                spawnedObjects.Add(throwableObject);

                throwableObject.SimpleThrow();
            }
            else {
                // TODO: set this as the sound for when a button is NOT clickable
                AudioManager.I.PlaySound(Sfx.KO);
            }
        }

        public void DragObject(ThrowableObject ObjectPrefab)
        {
            if (DraggedTransform != null)
                return;

            if (AppManager.I.Player.GetTotalNumberOfBones() > 0 && spawnedObjects.Count < MaxSpawnedObjectsInScene)
            {
                var newObjectGo = Instantiate(ObjectPrefab.gameObject);
                newObjectGo.SetActive(true);
                newObjectGo.transform.position = BoneSpawnPosition.position;
                var throwableObject = newObjectGo.GetComponent<ThrowableObject>();
                spawnedObjects.Add(throwableObject);

                DraggedTransform = newObjectGo.transform;
                throwableObject.Drag();
            }
            else {
                AudioManager.I.PlaySound(Sfx.KO);
            }
        }

        public void EatObject(ThrowableObject throwableObject)
        {
            if (spawnedObjects.Remove(throwableObject))
            {
                AudioManager.I.PlaySound(Sfx.EggMove);
                var poof = Instantiate(PoofPrefab).transform;
                poof.position = throwableObject.transform.position;

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
                {
                    AnturaHappiness = 1;
                }

                Destroy(throwableObject.gameObject);
            }
        }

        #endregion
    }
}
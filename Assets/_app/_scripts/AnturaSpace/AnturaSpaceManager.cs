using EA4S.Audio;
using EA4S.Core;
using EA4S.UI;
using EA4S.Utilities;
using UnityEngine;
using TMPro;
using EA4S.Antura;
using System.Collections.Generic;

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

        public Transform SceneCenter;
        public Transform AttentionPosition;
        public Transform BoneSpawnPosition;
        public GameObject BonePrefab;
        public GameObject PoofPrefab;

        public Transform DraggingBone { get; private set; }
        public Transform LaunchedBone
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
        public readonly AnturaDrawingAttentionState DrawingAttention;
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

        public void ThrowBone()
        {
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

        public void EatBone(GameObject bone)
        {
            if (bones.Remove(bone))
            {
                Instantiate(PoofPrefab).transform.position = bone.transform.position;
                Destroy(bone);
            }
        }

        public AnturaSpaceManager()
        {
            Idle = new AnturaIdleState(this);
            DrawingAttention = new AnturaDrawingAttentionState(this);
            Sleeping = new AnturaSleepingState(this);
            WaitingThrow = new AnturaWaitingThrowState(this);
            Catching = new AnturaCatchingState(this);
        }

        void Awake()
        {
            CurrentState = Idle;
        }

        public void Update()
        {
            stateManager.Update(Time.deltaTime);

            UI.ShowBonesButton(bones.Count < MaxBonesInScene);
            UI.BonesCount = AppManager.I.Player.GetTotalNumberOfBones();
        }

        public void FixedUpdate()
        {
            stateManager.UpdatePhysics(Time.fixedDeltaTime);
        }

        public Music backgroundMusic;

        void Start()
        {
            GlobalUI.ShowPauseMenu(false);
            GlobalUI.ShowBackButton(true, OnExit);
            AudioManager.I.PlayMusic(backgroundMusic);
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "enter");

        }

        void OnExit()
        {
            LogManager.I.LogInfo(InfoEvent.AnturaSpace, "exit");
            AppManager.I.NavigationManager.GoToNextScene();
        }
    }
}

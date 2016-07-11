using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace EA4S {

    public class DropContainer : MonoBehaviour {

        public List<DropSingleArea> Aree;
        int actualAreaIndex = 0;

        enum DropAreaPositions { ActivePos, NextPos, NextsPos, CompletedPos }
        
        /// <summary>
        /// Setup done. Set first as active.
        /// </summary>
        public void SetupDone() {
            actualAreaIndex = 0;
            dropAreaSetPosition();
        }

        public void NextArea() {
            if (actualAreaIndex < Aree.Count -1)
                actualAreaIndex++;
            else {
                actualAreaIndex++;
                if (OnObjectiveBlockCompleted != null)
                    OnObjectiveBlockCompleted();
            }
            dropAreaSetPosition();
        }

        /// <summary>
        /// Set area positions with animation.
        /// </summary>
        void dropAreaSetPosition() {
            for (int i = 0; i < Aree.Count; i++) {
                if (actualAreaIndex == i) { 
                    positionigAreaDropElement(Aree[i], DropAreaPositions.ActivePos, delegate () { });
                    Aree[i].SetEnabled();
                } else if (actualAreaIndex > i) {
                    positionigAreaDropElement(Aree[i], DropAreaPositions.CompletedPos, delegate () { });
                    Aree[i].SetDisbled();
                } else if (actualAreaIndex +1 == i) {
                    positionigAreaDropElement(Aree[i], DropAreaPositions.NextPos, delegate () { });
                    Aree[i].SetDisbled();
                } else {
                    positionigAreaDropElement(Aree[i], DropAreaPositions.NextsPos, delegate () { });
                    Aree[i].SetDisbled();
                }
            }

        }

        #region events
        public delegate void ObjectiveEvent();

        /// <summary>
        /// Happens when a peace of objective completed. Ex: Letter match (peace of block objective -> word).
        /// </summary>
        public static event ObjectiveEvent OnObjectivePeaceCompleted;

        /// <summary>
        /// Happens when a peace of objective completed. Ex: Word match completed (word is a block objective).
        /// </summary>
        public static event ObjectiveEvent OnObjectiveBlockCompleted;
        #endregion

        #region event subscription
        void OnEnable() {
            Droppable.OnRightMatch += Droppable_OnRightMatch;
            Droppable.OnWrongMatch += Droppable_OnWrongMatch;
        }

        void OnDisable() {
            Droppable.OnRightMatch -= Droppable_OnRightMatch;
            Droppable.OnWrongMatch -= Droppable_OnWrongMatch;
        }
        #endregion

        #region results events delegates
        private void Droppable_OnWrongMatch(LetterObjectView _letterView) {

        }

        /// <summary>
        /// Risen on letter or world match.
        /// </summary>
        private void Droppable_OnRightMatch(LetterObjectView _letterView) {
            NextArea();
        }
        #endregion

        #region Tween Animations
        void positionigAreaDropElement(DropSingleArea _dropArea, DropAreaPositions _position, TweenCallback _callback) {
            float durantion = 0.4f;
            Sequence _sequence = DOTween.Sequence();
            bool needFade = false;

            if (_position == DropAreaPositions.CompletedPos)
                needFade = true;

            // - Actual elimination
            _sequence.Append(_dropArea.transform.DOLocalMove(getPosition(_position), durantion)).OnComplete(delegate () {
                if (needFade) {
                    _sequence.Append(_dropArea.GetComponent<MeshRenderer>().materials[0].DOFade(0, durantion));
                    _sequence.Append(_dropArea.LetterLable.transform.DOLocalMove(new Vector3(getPosition(_position).x, -2, getPosition(_position).z), durantion));
                    // pro only
                    // sequence.Append(Aree[actualAreaIndex].LetterLable.DOFade(0, 0.4f));
                }
                _callback();
            }).OnStart(delegate () {
                _sequence.Append(_dropArea.transform.DOLocalRotate(getRotation(_position), durantion));
            });
        }

        /// <summary>
        /// Return right position.
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        Vector3 getPosition(DropAreaPositions _position) {
            switch (_position) {
                case DropAreaPositions.ActivePos:
                    return new Vector3(0, 0.1f, 0);
                case DropAreaPositions.NextPos:
                    return new Vector3(-6, 0.1f, -1.5f);
                case DropAreaPositions.NextsPos:
                    return new Vector3(-12, 0.1f, -6);
                case DropAreaPositions.CompletedPos:
                    return new Vector3(6, 0.1f, 0);
                default:
                    Debug.LogError("Position not found");
                    break;
            }
            return new Vector3(0, -10, 0); // underground...
        }

        /// <summary>
        /// Return right position.
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        Vector3 getRotation(DropAreaPositions _position) {
            switch (_position) {
                case DropAreaPositions.ActivePos:
                    return new Vector3(90,0,0);
                case DropAreaPositions.NextPos:
                    return new Vector3(90, -30, 0);
                case DropAreaPositions.NextsPos:
                    return new Vector3(90, -30, 30);
                case DropAreaPositions.CompletedPos:
                    return new Vector3(90, 0, 0);
                default:
                    Debug.LogError("Position not found");
                    break;
            }
            return new Vector3(0, -10, 0); // underground...
        }
        
        #endregion

    }
}
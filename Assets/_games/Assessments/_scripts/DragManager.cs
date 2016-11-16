using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public enum DragType
    {
        Anywhere
    }

    // This class has to filter grab/dropping. Should not grab/drop during
    // pause and should not grab/drop if something else is grabbed/dropped
    public class DragManager
    {
        private IDraggable draggable;
        private ScoreCounter score;

        public DragManager( ScoreCounter score)
        {
            Debug.Log("Setted ScoreCounter:" + score);
            this.score = score;
        }

        IAudioManager audioManager;
        public void SetAudioManager( IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        DragType type;
        public void SetDragType( DragType type)
        {
            this.type = type;
        }

        public IDraggable DecorateLivingLetter( GameObject go, int groupID)
        {
            var comp = go.GetComponent< LetterObjectView>();
            if (comp != null)
                return DecorateLivingLetter( comp, groupID);

            throw new ArgumentException("No LetterObjectView in GameObject trying to be decorated");
        }

        public IDraggable DecorateLivingLetter( LetterObjectView letter, int groupID)
        {
            var collider = letter.gameObject.AddComponent< SphereCollider>();
            collider.radius = 2;
            collider.center = new Vector3( 0, 0, 0);
            collider.isTrigger = true;

            //TODO: capsule already removed but not yet merged into Dev
            var capsule = letter.GetComponent< CapsuleCollider>();
            if (capsule != null)
                UnityEngine.Object.Destroy( capsule);

            IDraggable draggable = null;
            switch (type)
            {
                case DragType.Anywhere: 
                    draggable = letter.gameObject.AddComponent< Draggable>(); break;
                default: break;
            }

            draggable.SetGroupID(groupID);
            draggable.SetDragManager(this);

            return draggable;
        }

        public void StartDragging( IDraggable draggable)
        {
            if (this.draggable != null)
                return;

            audioManager.PlaySound(Sfx.ThrowObj);
            this.draggable = draggable;
            draggable.OnBecomeDragged();
        }

        public void StopDragging( IDraggable draggable)
        {
            if (this.draggable != draggable)
                throw new InvalidOperationException( "Can drop only previously dragged objet");

            audioManager.PlaySound(Sfx.ThrowObj);
            switch (type)
            {
                case DragType.Anywhere:
                    CheckCollidedWithPlaceHolder(); break;
                default: break;
            }
            
            this.draggable.OnBecomeDropped();
            this.draggable = null;
        }

        //Iterates: TODO: need to inject placeholders
        private void CheckCollidedWithPlaceHolder()
        {
            var zones = GameObject.FindObjectsOfType< DropZone>();
            bool placedNearZone = false;
            foreach( var zone in zones)
            {
                if (IsNearDropZone( zone))
                {
                    placedNearZone = true;
                    if (zone.GetGroup() == draggable.GetGroupID())
                    {
                        Match(zone);
                        return;
                    }                        
                }
            }
            if(placedNearZone)
                Mismatch();
        }

        private void Mismatch()
        {
            Debug.Log("MisMatch");
            //TODO CONT SCORE --

            audioManager.PlaySound( Sfx.Splat);
            draggable.ReturnToOrigin();
            score.WrongAnswer(draggable.GetGroupID());
        }

        private void Match( DropZone zone)
        {
            Debug.Log("Match");

            audioManager.PlaySound( Sfx.StampOK);
            //TODO CONT SCORE ++
            draggable.PlacedOnCorrectPlace(zone.transform.localPosition);
            zone.PlacedOnCorrectPlace();
            score.CorrectAnswer(draggable.GetGroupID());
        }

        bool IsNearDropZone( DropZone zone)
        {
            var p1 = zone.transform.position;
            var p2 = draggable.GetPosition();
            p1.z = p2.z = 0;
            return p1.DistanceIsLessThan( p2, 3f);
        }

        public void Update( float delta)
        {
            if (draggable != null)
            {
                draggable.SetPosition( Input.mousePosition);
            }
        }

        internal void DecorateDropZone( DropZone zone, int groupID)
        {
            zone.SetGroup( groupID);
        }
    }
}

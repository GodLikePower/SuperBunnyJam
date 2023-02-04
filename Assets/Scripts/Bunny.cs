using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using BNG;

namespace SuperBunnyJam
{
    public class Bunny : GrabbableEvents, IRootBreaker
    {
        [SerializeField]
        MMF_Player _bunnySpawnFeedback;
        [SerializeField]
        Rigidbody _bunnyRB;
        [SerializeField]
        BoxCollider _bunnyCollider;
        [SerializeField]
        float _bunnyForceMoltiplir;

        public float collisionForceMultiplier => 1f;

        [field: SerializeField]
        public int color { get; set; }
        
        public bool isActiveBreaker => true;

        public bool isObliterator => false;

        public bool penaltyOnColorMismatch => true;

        private void Start()
        {
            _bunnyRB.isKinematic = true;
            _bunnySpawnFeedback?.PlayFeedbacks();
        }

        public override void OnGrab(Grabber grabber)
        {
            base.OnGrab(grabber);
            _bunnyRB.isKinematic = false;
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }

        public void OnBreak(RootSegment segment)
        {
            /*throw new System.NotImplementedException();*/
        }

        public void OnMismatch(RootSegment segment)
        {
           /* throw new System.NotImplementedException();*/
        }

        public void OnTooWeak(RootSegment segment)
        {
           /* throw new System.NotImplementedException();*/
        }
    }

}

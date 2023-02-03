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

        public float collisionForceMultiplier => throw new System.NotImplementedException();

        public int color => throw new System.NotImplementedException();

        public bool isActiveBreaker => throw new System.NotImplementedException();

        public bool isObliterator => throw new System.NotImplementedException();

        public bool penaltyOnColorMismatch => throw new System.NotImplementedException();

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
            throw new System.NotImplementedException();
        }

        public void OnMismatch(RootSegment segment)
        {
            throw new System.NotImplementedException();
        }

        public void OnTooWeak(RootSegment segment)
        {
            throw new System.NotImplementedException();
        }
    }

}

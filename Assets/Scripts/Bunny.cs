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
        [SerializeField]
        MMF_Player _bunnyPullFeedback;
        [SerializeField]
        MMF_Player _bunnyHitFeedback;

        public float collisionForceMultiplier => 1f;

        [field: SerializeField]
        public int color { get; set; }
        
        public bool isActiveBreaker => true;

        public bool isObliterator => false;

        public bool penaltyOnColorMismatch => true;

        [SerializeField]
        FMODUnity.EventReference soundForGrab;
        [SerializeField]
        FMODUnity.EventReference soundForRelease;
        [SerializeField]
        FMODUnity.EventReference soundForBreakRoot;
        [SerializeField]
        FMODUnity.EventReference soundForTooWeak;
        [SerializeField]
        FMODUnity.EventReference soundForWrongColor;

        private void Start()
        {
            _bunnyRB.isKinematic = true;
            _bunnySpawnFeedback?.PlayFeedbacks();
        }

        public override void OnGrab(Grabber grabber)
        {
            base.OnGrab(grabber);
            _bunnyRB.isKinematic = false;
            _bunnyPullFeedback?.PlayFeedbacks();

            this.TryPlaySound(soundForGrab);
        }

        public override void OnRelease()
        {
            base.OnRelease();
            Destroy(gameObject, 5);

            this.TryPlaySound(soundForRelease);
        }

        public void OnBreak(RootSegment segment)
        {
            _bunnyHitFeedback?.PlayFeedbacks();
            this.TryPlaySound(soundForBreakRoot);
        }

        public void OnMismatch(RootSegment segment)
        {
            this.TryPlaySound(soundForWrongColor);
        }

        public void OnTooWeak(RootSegment segment)
        {
            this.TryPlaySound(soundForTooWeak);
        }
    }

}

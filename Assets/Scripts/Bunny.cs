using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using BNG;

namespace SuperBunnyJam
{
    public class Bunny : GrabbableEvents
    {
        [SerializeField]
        MMF_Player _bunnySpawnFeedback;
        [SerializeField]
        Rigidbody _bunnyRB;
        [SerializeField]
        BoxCollider _bunnyCollider;
        [SerializeField]
        float _bunnyForceMoltiplir;

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

    }

}

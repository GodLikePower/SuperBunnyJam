using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using BNG;

namespace SuperBunnyJam
{
    public class Bunny : MonoBehaviour
    {
        [SerializeField]
        MMF_Player _bunnySpawnFeedback;
        [SerializeField]
        Rigidbody _bunnyRB;
        [SerializeField]
        BoxCollider _bunnyCollider;

        private void Start()
        {
            _bunnyCollider.enabled = false;
            _bunnyRB.useGravity = false;
            _bunnyRB.isKinematic = true;
            _bunnySpawnFeedback?.PlayFeedbacks();
        }
    }

}

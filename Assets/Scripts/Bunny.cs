using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace SuperBunnyJam
{
    public class Bunny : MonoBehaviour
    {
        [SerializeField]
        MMF_Player _bunnySpawnFeedback;

        private void Start()
        {
            _bunnySpawnFeedback?.PlayFeedbacks();
        }
    }
}

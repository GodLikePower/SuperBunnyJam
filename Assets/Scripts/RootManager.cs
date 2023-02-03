using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class RootManager : SingletonBehavior<RootManager> {

        /// <summary>GameObject towards which roots tend to grow (presumably the player's platform or a point somewhere below it)</summary>
        public GameObject attractor;

        [field: SerializeField]
        public float baseBranchProbability { get; private set; }

        [field: SerializeField]
        public float baseCollisionForceRequiredToBreak { get; private set; }

        /// <remarks>In units/s</remarks>
        [field: SerializeField]
        public float baseGrowthRate { get; private set; }

        [field: SerializeField]
        public float baseScatter { get; private set; }

        public Material[] rootColors;

        [SerializeField]
        RootSegment rootPrefab;

        public Roll segmentLength;

        public RootSegment Spawn(Vector3 position, Quaternion rotation, int color) {
            var result = Instantiate(rootPrefab, position, rotation);

            result.color = color;

            return result;
        }
    }
}
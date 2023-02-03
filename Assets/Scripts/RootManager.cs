using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class RootManager : SingletonBehavior<RootManager> {

        /// <summary>GameObject towards which dry roots tend to grow (presumably somewhere under the player's platform)</summary>
        public GameObject dryAttractor;

        /// <summary>GameObject towards which wet roots tend to grow (presumably around the HMD's positions)</summary>
        /// <remarks>Defaults to CenterEyeAnchor</remarks>
        public GameObject wetAttractor;

        [field: SerializeField]
        public float baseBranchProbability { get; private set; }

        [field: SerializeField]
        public float baseCollisionForceRequiredToBreak { get; private set; }

        /// <remarks>In units/s</remarks>
        [field: SerializeField]
        public float baseGrowthRate { get; private set; }

        [field: SerializeField]
        public float baseScatter { get; private set; }

        /// <summary>If true, wet roots will seek their attractor more accurately the closer they get. Scatter increases linearly with distance,
        /// from 0 at 0 to baseScatter at rangeFormaxScatter</summary>
        public bool decreaseScatterWithProximityToWetAttractor;

        public float rangeForMaxScatter = 3f;

        public Material[] rootColors;

        public GameObject rootContainer;

        [SerializeField]
        RootSegment rootPrefab;

        /// <summary>Determines width and height of roots</summary>
        public Roll rootTransverseSize;

        public Roll segmentLength;

        public Material[] wetRootColors;

        public float wetSpeedMultipier;

        private void Start() {
            if (wetAttractor == null)
                wetAttractor = Main.instance.centerEyeAnchor;
        }

        public float Scatter(RootSegment segment) {            
            var scatter = baseScatter * ScatterModifier(segment);

            return Random.value * (Random.value > 0.5f ? scatter : -scatter);
        }

        public float ScatterModifier(RootSegment segment) {
            // Do segments become more accurate when wet and close to target? Is this one wet etc?
            if (decreaseScatterWithProximityToWetAttractor && segment.isWet) {
                var distanceFromWetAttractor = (segment.endpoint - wetAttractor.transform.position).magnitude;

                if (distanceFromWetAttractor < rangeForMaxScatter)
                    return distanceFromWetAttractor / rangeForMaxScatter;
            }

            // Scatter unchanged
            return 1f;
        }

        public RootSegment Spawn(Vector3 position, Quaternion rotation, int color, Vector2 size) {
            var result = Instantiate(rootPrefab, position, rotation);

            result.Init();

            result.color = color;
            result.transverseSize = size;

            return result;
        }
    }
}
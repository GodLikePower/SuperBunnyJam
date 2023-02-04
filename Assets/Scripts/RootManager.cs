using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SuperBunnyJam {
    public class RootManager : SingletonBehavior<RootManager> {

        /// <summary>GameObject towards which dry roots tend to grow (presumably somewhere under the player's platform)</summary>
        public GameObject dryAttractor;

        /// <summary>GameObject towards which wet roots tend to grow (presumably around the HMD's positions)</summary>
        /// <remarks>Defaults to CenterEyeAnchor</remarks>
        public GameObject wetAttractor;

        /// <summary>A little bit extra that we take off the tip when severing branches, to avoid weirdness</summary>
        public float addedDestructionRadius = 0.2f;

        [field: SerializeField]
        public float baseBranchProbability { get; private set; }

        [field: SerializeField]
        public float baseCollisionForceRequiredToBreak { get; private set; }

        /// <remarks>In units/s</remarks>
        [field: SerializeField]
        public float baseGrowthRate { get; private set; }

        [field: SerializeField]
        public float baseScatter { get; private set; }

        [SerializeField]
        Roll baseSpawnInterval;

        /// <summary>If true, wet roots will seek their attractor more accurately the closer they get. Scatter increases linearly with distance,
        /// from 0 at 0 to baseScatter at rangeFormaxScatter</summary>
        public bool decreaseScatterWithProximityToWetAttractor;

        [SerializeField]
        int maxRoots = 10;

        float nextSpawnTime;

        [SerializeField]
        Nubbin[] nubbinPrefabs;

        public float rangeForMaxScatter = 3f;

        public Material[] rootColors;
        public Material[] rootCorpseColors;

        public GameObject rootContainer;

        public MeshRenderer rootCorpsePrefab;

        [SerializeField]
        RootSegment rootPrefab;

        /// <remarks>A rootRoot is the root of a root</remarks>
        Dictionary<int, HashSet<RootSegment>> rootRootsByColor;        

        /// <summary>Determines width and height of roots</summary>
        public Roll rootTransverseSize;
        
        public float shrinkTimeOnHit = 0.4f;
        /// <remarks>m/s</remarks>
        public float shrinkRate = 1.5f;

        public float stunTimeAfterShrink = 1f;

        public Roll segmentLength;

        [SerializeField]
        GameObject spawnPoints;
        Dictionary<RootSegment, Transform> spawnPointsByRoot;
        HashSet<Transform> availableSpawnPoints;

        public Material[] wetRootColors;

        public float wetSpeedMultipier;

        private void Start() {
            if (wetAttractor == null)
                wetAttractor = Main.instance.centerEyeAnchor;

            rootRootsByColor = new Dictionary<int, HashSet<RootSegment>>();
            spawnPointsByRoot = new Dictionary<RootSegment, Transform>();

            for (var i = 0; i < rootColors.Length; ++i)
                rootRootsByColor[i] = new HashSet<RootSegment>();

            availableSpawnPoints = spawnPoints.GetComponentsInChildren<Transform>().ToHashSet();
        }        

        public void OnDie(RootSegment segment) {
            // Is this a root root?
            var point = spawnPointsByRoot.Get(segment);

            if (point == null)
                // Nope
                return;

            // Yup. Update pertinent state
            availableSpawnPoints.Add(point);
            rootRootsByColor[segment.color].Remove(segment);
            spawnPointsByRoot.Remove(segment);
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

        public RootSegment Spawn(Vector3 position, Quaternion rotation, int color, Vector2 size, RootSegment predecessor = null) {
            var result = Instantiate(rootPrefab, position, rotation);

            result.Init();

            result.color = color;
            result.transverseSize = size;

            if (predecessor != null) {
                result.predecessor = predecessor;

                if (nubbinPrefabs.Length > 0 && predecessor.nubbin == null) {
                    // Nub
                    var nubbin = Instantiate(nubbinPrefabs.Choice());

                    nubbin.transform.position = (result.transform.position + predecessor.endpoint) * 0.5f;
                    nubbin.targetScale = Mathf.Max(result.transverseSize.x, result.transverseSize.y, predecessor.transverseSize.x, predecessor.transverseSize.y);

                    predecessor.nubbin = nubbin;

                    nubbin.transform.parent = rootContainer.transform;
                }
            }

            result.transform.parent = rootContainer.transform;

            return result;
        }

        RootSegment TrySpawnRootRoot() {
            if (availableSpawnPoints.Count == 0)
                // No available points
                return null;
            // Lazy kludge, note that we don't do collision checking, just check for points without associated roots

            // Count extant roots
            if (rootRootsByColor.Values.Sum(v => v.Count) >= maxRoots)
                // Too many
                return null;

            // Choose one of the least-represented colors (in terms of root roots, we don't care about total vine area)
            var color = -1;
            {
                var min = rootRootsByColor.Values.Min(v => v.Count);

                color = rootRootsByColor.Where(p => p.Value.Count == min).Choice().Key;
            }

            // Choose spawn point
            var point = availableSpawnPoints.Choice();

            // Spawn
            var result = Spawn(point.position, point.rotation, color, new Vector2(rootTransverseSize.Value(), rootTransverseSize.Value()));

            // Remember
            spawnPointsByRoot[result] = point;
            availableSpawnPoints.Remove(point);
            rootRootsByColor[color].Add(result);

            return result;
        }

        private void Update() {
            // Try to spawn a new root?
            if (nextSpawnTime <= Time.time) {
                Debug.Assert(baseSpawnInterval.max >= 0f);

                // Remove dead roots
                foreach (var l in rootRootsByColor.Values)
                    l.RemoveWhere(r => r == null);

                TrySpawnRootRoot();

                // Whether succeeded or failed, go on cooldown
                nextSpawnTime = Time.time + baseSpawnInterval.Value();                    
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public interface IRootBreaker {

        /// <summary>Multiplies the actual force of our collision to determine w</summary>
        float collisionForceMultiplier { get; }

        /// <remarks>If negative, will break roots irrespective of color</remarks>
        int color { get; }

        /// <summary>If false, breaker will be disregarded on collision</summary>
        bool isActiveBreaker { get; }

        /// <summary>If true, will completely destroy any segments it contacts, else will break them at the point of contact</summary>
        /// <remarks>Non-obliterating breakers should probably self-destruct or set isActiveBreaker to false on collision</remarks>
        bool isObliterator { get; }

        /// <summary>If true, trying to break a wrong-colored root with this breaker will horribly backfire somehow</summary>
        bool penaltyOnColorMismatch { get; }

        /// <summary>Called just before the segment in question is broken</summary>
        void OnBreak(RootSegment segment);

        /// <summary>Called upon hitting a wrong-colored segment</summary>
        void OnMismatch(RootSegment segment);

        /// <summary>Called when a breakable segment is hit with insufficient force</summary>
        void OnTooWeak(RootSegment segment);
    }

    [RequireComponent(typeof(BoxCollider))]
    //[RequireComponent(typeof(LineRenderer))]
    public class RootSegment : MonoBehaviour {

        const float branchCollisionCheckLength = gapBetweenSegments * 2f;

        /// <summary>Interval at which we retry growing new segments (if previous attempt was blocked by something)</summary>
        const float branchRetryInterval = 1f;
        const int branchTriesOnFork = 20;
        const int branchTriesPerCheck = 4;        

        const float gapBetweenSegments = 0.3f;

        const float startingLength = 0.1f;

        float branchRetryTime;

        new BoxCollider collider;

        /// <see cref="RootManager.rootColors"/>
        public int color;

        //LineRenderer renderer;        

        /// <summary>_Immediate_ successors</summary>
        public RootSegment[] successors { get; private set; }

        public float targetLength { get; private set; }

        MeshRenderer visualization;

        private void Start() {
            // Straightforward stuff
            {
                collider = GetComponent<BoxCollider>();

                //renderer = GetComponent<LineRenderer>();
                //renderer.useWorldSpace = false;
                //renderer.SetPosition(0, Vector3.zero);
                //renderer.SetPosition(1, Vector3.zero);

                visualization = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();

                successors = new RootSegment[2];
            }            

            // Length and target length
            length = startingLength;
            targetLength = RootManager.instance.segmentLength.Value();

            RefreshVisualization();
        }

        bool CheckRootCollision(Vector3 direction, float checkLength, float epsilon = 0.1f) {
            UnityEngine.Profiling.Profiler.BeginSample("GrowthCollisionCheck");

            var checkExtents = collider.size * 0.5f;
            checkExtents.z = 0.5f * checkLength;

            foreach (var contact in Physics.OverlapBox(endpoint + direction * (epsilon + checkExtents.z), checkExtents, Quaternion.LookRotation(direction))) {
                if (contact.GetComponent<RootSegment>() != null) {
                    // Found another root
                    UnityEngine.Profiling.Profiler.EndSample();

                    return true;
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();

            return false;
        }

        void DestroySuccessors() {
            foreach (var s in successors)
                if (s != null)
                    s.Die();
        }

        void Die() {
            Destroy(gameObject);

            DestroySuccessors();
        }

        public Vector3 endpoint => transform.position + transform.forward * length;

        public float growthRate => isGrowing ? RootManager.instance.baseGrowthRate : 0f;

        /// <summary>True if segment itself (rather than its successors) is still growing</summary>
        public bool isGrowing => length < targetLength;

        public float length {
            get => collider.size.z;

            private set {
                var size = collider.size;
                var center = collider.center;

                size.z = value;
                collider.size = size;                

                center.z = value * 0.5f;
                collider.center = center;

                RefreshVisualization();
            }
        }

        private void OnCollisionEnter(Collision collision) {
            // Hit by a root breaker?
            var breaker = collision.gameObject.GetComponent<IRootBreaker>();
            if (breaker != null && breaker.isActiveBreaker)
                TryBreak(breaker.color, breaker.penaltyOnColorMismatch, collision.GetContact(0).point,
                    (collision.impulse / Time.fixedDeltaTime).magnitude * breaker.collisionForceMultiplier);
        }

        void RefreshVisualization() {
            //renderer.SetPosition(1, Vector3.forward * length);
            visualization.transform.localScale = collider.size;
            visualization.transform.localPosition = collider.center;

            visualization.sharedMaterial = RootManager.instance.rootColors[color];
        }        

        /// <summary>Tries to find a direction in which we can branch without bumping into anything</summary>
        Vector3? TryFindBranchDirection(int numTries) {            
            float component() {
                return Random.value * (Random.value > 0.5f ? RootManager.instance.baseScatter : -RootManager.instance.baseScatter);
            }

            for (var i = 0; i < numTries; ++i) {
                // Choose random direction
                var result = Quaternion.Euler(component(), component(), 0) * transform.forward;
                // TODO: bias towards attractor (or do 2 stages, first biased downwards, then upwards and towards player (after touching water)

                // Check for obstacles
                if (!CheckRootCollision(result, branchCollisionCheckLength))
                    // None 
                    return result;                
            }

            // Failed
            return null;
        }

        /// <param name="breakPosition">If null, will attempt to completely destroy the segment, else will break it at that point</param>
        public void TryBreak(int breakerColor, bool penaltyOnColorMismatch, Vector3? breakPosition, float collisionForce = 100000f) {
            if (breakerColor >= 0 && breakerColor != color) {
                // Color mismatch
                // TODO: penalty
                return;
            }

            if (collisionForce < RootManager.instance.baseCollisionForceRequiredToBreak)
                // Not enough force
                return;

            // Break root
            if (breakPosition == null) {
                // Completely
                Die();

                return;
            }

            // But only partially
            {
                DestroySuccessors();

                // Wait, haven't thought out edge cases here
                throw new System.NotImplementedException();
            }
        }

        private void Update() {
            // Are we growing?
            if (isGrowing) {
                // Yes
                var rate = growthRate * Time.deltaTime;

                // Can we continue, or are we about to bump into another root?
                if (CheckRootCollision(transform.forward, rate)) {
                    // BUMP WARNING, arrest growth
                    targetLength = length;
                    UnityEngine.Profiling.Profiler.EndSample();

                    return;
                }

                // We're good to grow
                length += rate;
                length = Mathf.Min(length, targetLength);
                
                return;
            }

            // Not growing, should we branch out into 1+ new segments?
            foreach (var s in successors)
                if (s != null)
                    // Already have a successor
                    return;

            if (branchRetryTime > Time.time)
                // Branching on cooldown
                return;

            // Yes
            {
                var direction = TryFindBranchDirection(branchTriesPerCheck);

                if (direction == null) {
                    // Couldn't find unobstructed path, abort for now
                    branchRetryTime = Time.time + branchRetryInterval;

                    return;
                }

                successors[0] = RootManager.instance.Spawn(endpoint + direction.Value * gapBetweenSegments, Quaternion.LookRotation(direction.Value), color);

                // Should we fork?
                if (Random.value < RootManager.instance.baseBranchProbability) {
                    direction = TryFindBranchDirection(branchTriesOnFork);
                
                    if (direction != null)
                        successors[1] = RootManager.instance.Spawn(endpoint + direction.Value * gapBetweenSegments, Quaternion.LookRotation(direction.Value), color);

                    // Lazy kludge, if we fail to fork, we don't retry later
                }
            }
        }
    }
}
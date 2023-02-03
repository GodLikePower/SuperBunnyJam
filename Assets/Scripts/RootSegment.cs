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
        const int branchTriesOnFork = branchTriesPerCheck * 5;
        const int branchTriesPerCheck = 8;        

        // TODO: could just eliminate this any number of ways
        const float gapBetweenSegments = 0.3f;

        const float startingLength = 0.1f;

        float branchRetryTime;

        new BoxCollider collider;

        /// <see cref="RootManager.rootColors"/>
        public int color;

        public bool isWet { get; private set; }

        //LineRenderer renderer;        

        /// <summary>_Immediate_ successors</summary>
        public RootSegment[] successors { get; private set; }

        public float targetLength { get; private set; }

        MeshRenderer visualization;
        private void Start() {
            Init();
        }

        public void Init() {
            if (successors != null)
                // Already initialized
                return;

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

        bool CheckRootCollision(Vector3 direction, float checkLength, Vector2 checkTransverseSize, float epsilon = 0.1f) {
            UnityEngine.Profiling.Profiler.BeginSample("GrowthCollisionCheck");

            var checkExtents = new Vector3(checkTransverseSize.x * 0.5f, checkTransverseSize.y * 0.5f, 0.5f * checkLength);

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
            RootManager.instance.OnDie(this);

            Destroy(gameObject);

            DestroySuccessors();
        }

        public Vector3 endpoint => transform.position + transform.forward * length;

        public float growthRate {
            get {
                if (!isGrowing)
                    return 0f;

                var result = RootManager.instance.baseGrowthRate;

                if (isWet)
                    result *= RootManager.instance.wetSpeedMultipier;

                return result;
            }
        }

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

        private void OnTriggerEnter(Collider other) {            
            if (other.tag.Equals("Water"))
                // Splish splash
                isWet = true;
        }

        void RefreshVisualization() {
            //renderer.SetPosition(1, Vector3.forward * length);
            visualization.transform.localScale = collider.size;
            visualization.transform.localPosition = collider.center;

            // Add a little tail, to hide gaps
            {
                var tailLength = gapBetweenSegments * 1.1f;
                
                var size = visualization.transform.localScale;
                var position = visualization.transform.localPosition;

                size.z += tailLength;
                position.z -= tailLength * 0.5f;

                visualization.transform.localScale = size;
                visualization.transform.localPosition = position;
            }

            visualization.sharedMaterial = isWet ? RootManager.instance.wetRootColors[color] : RootManager.instance.rootColors[color];
        }        

        /// <summary>Width and height</summary>
        public Vector2 transverseSize {
            get => collider.size;

            set {
                var size = collider.size;

                size.x = value.x;
                size.y = value.y;

                collider.size = size;
            }
        }

        /// <summary>Tries to find a direction in which we can branch without bumping into anything</summary>
        (Vector3 direction, Vector2 transverseSize)? TryPlanBranch(int numTries) {
            var attractor = isWet ? RootManager.instance.wetAttractor : RootManager.instance.dryAttractor;

            for (var i = 0; i < numTries; ++i) {
                // Base direction is partway between out current facing and the look facing for the attractor
                var direction = transform.forward;
                if (attractor != null) {                    
                    // The lower our scatter modifier, the more we're biased towards the attractor
                    var scatter = RootManager.instance.ScatterModifier(this);

                    var toAttractor = (attractor.transform.position - transform.position);
                    var maxExtraWeight = 4f;

                    var weight = 1f + (1f - scatter) * maxExtraWeight;

                    direction = ((direction * scatter + toAttractor * weight) / (1f + weight)).normalized;
                }

                var size = new Vector2(RootManager.instance.rootTransverseSize.Value(), RootManager.instance.rootTransverseSize.Value());
                                
                // Add scatter                
                direction = Quaternion.Euler(RootManager.instance.Scatter(this), RootManager.instance.Scatter(this), 0) * direction;

                // Check for obstacles
                if (!CheckRootCollision(direction, branchCollisionCheckLength, size))
                    // None 
                    return (direction, size);
            }

            // Failed
            return null;
        }

        /// <param name="breakPosition">If null, will attempt to completely destroy the segment, else will break it at that point</param>
        public void TryBreak(int breakerColor, bool penaltyOnColorMismatch, Vector3? breakPosition = null, float collisionForce = 100000f) {
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

                // Find nearest point on line
                var onLine = NearestPointOnLine(transform.position, transform.forward, breakPosition.Value);

                // Is it even on the segment?
                var distance = transform.InverseTransformPoint(onLine).z;
                if (distance < 0f || distance > length) {
                    // Nope
                    Debug.Log("Segment collision edge case, if you see too many of these something is wrong");

                    return;
                }

                length = distance;
            }
        }

        Vector3 NearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point) {
            var pointToOrigin = origin - point;
            return point + pointToOrigin - Vector3.Dot(pointToOrigin, direction) * direction;
        }

        private void Update() {
            // Are we growing?
            if (isGrowing) {
                // Yes
                var rate = growthRate * Time.deltaTime;

                // Can we continue, or are we about to bump into another root?
                if (CheckRootCollision(transform.forward, rate, transverseSize)) {
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
                var plan = TryPlanBranch(branchTriesPerCheck);

                if (plan == null) {
                    // Couldn't find unobstructed path, abort for now
                    branchRetryTime = Time.time + branchRetryInterval;

                    return;
                }

                RootSegment spawn() {
                    var spawned = RootManager.instance.Spawn(endpoint + plan.Value.direction * gapBetweenSegments, Quaternion.LookRotation(plan.Value.direction), color,
                    plan.Value.transverseSize);

                    spawned.isWet = isWet;

                    spawned.transform.parent = RootManager.instance.rootContainer.transform;

                    return spawned;
                }

                successors[0] = spawn();

                // Should we fork?
                if (Random.value < RootManager.instance.baseBranchProbability) {
                    plan = TryPlanBranch(branchTriesOnFork);

                    if (plan != null) {
                        successors[1] = spawn();
                    }

                    // Lazy kludge, if we fail to fork, we don't retry later
                }
            }
        }
    }
}
using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SuperBunnyJam {

    /// <summary>Enters glide mode, partially or wholly negating gravity (and instantaneously gaining lift and losing its downward velocity component),
    /// some time after being released (unless it hits something first)</summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Glider : GrabbableEvents {

        public float antigravityFraction;

        /// <summary>Time between release and glide mode</summary>
        public float armingTime;

        Rigidbody body;

        float dragWhenNotGliding;

        float glideStartTime;

        public bool isGliding { get; private set; }

        /// <summary>Fraction of our velocity which will be converted to lift on glide start</summary>
        public float liftRatio;

        public UnityEvent onGlideEnd;

        public UnityEvent onGlideStart;

        private void Start() {
            body = GetComponent<Rigidbody>();

            dragWhenNotGliding = body.drag;
        }

        private void FixedUpdate() {
            if (isGliding) {
                // Antigravity
                body.AddForce(-Physics.gravity * antigravityFraction, ForceMode.Acceleration);
            }
        }

        void EndGlide() {
            glideStartTime = 0f;

            body.drag = dragWhenNotGliding;

            if (isGliding) {
                isGliding = false;
                onGlideEnd?.Invoke();
            }
        }

        private void OnCollisionEnter(Collision collision) {
            EndGlide();
        }

        
        public override void OnGrab(Grabber grabber) {
            base.OnGrab(grabber);

            EndGlide();
        }

        public override void OnRelease() {
            base.OnRelease();

            glideStartTime = Time.time + armingTime;
        }

        private void Update() {
            if ((!isGliding) && glideStartTime > 0f && glideStartTime <= Time.time) {
                // Start gliding
                onGlideStart?.Invoke();

                body.drag = 0f;

                isGliding = true;

                // Convert downward velocity into forward
                var v = body.velocity;
                var antiDown = Mathf.Max(0f, -v.y);

                if (antiDown != 0f)
                    v.y = 0f;

                var direction = v.normalized;

                v += direction * antiDown;

                // Gain lift based on total velocity (after downward conversion component)
                var lift = liftRatio * v.magnitude;

                // Apply forces
                body.AddForce(Vector3.up * (lift + antiDown) + direction * (antiDown - lift), ForceMode.VelocityChange);
            }
        }
    }
}

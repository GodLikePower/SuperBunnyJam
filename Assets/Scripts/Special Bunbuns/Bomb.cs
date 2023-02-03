using BNG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SuperBunnyJam {
    public class Bomb : MonoBehaviour {

        public enum VrButton {
            None,
            A,
            B,
            X,
            Y,
            LeftTrigger,
            RightTrigger
        }

        /// <remarks>-1 is still a wildcard. If color isn't wild, mismatched roots will block the explosion</remarks>
        public int color;

        public VrButton detonatorButton;

        /// <remarks>Ignores collisions with non-root objects</remarks>
        public bool explodesOnImpact;

        public float explosionRadius;

        public UnityEvent onExplode;

        public void Explode() {
            onExplode?.Invoke();            

            // Are we an indiscriminate sphere of annihilation, or a series of feeble, blockable raycasts?
            if (color < 0) {
                // Doomsphere
                foreach (var seg in Physics.OverlapSphere(transform.position, explosionRadius).Select(c => c.GetComponent<RootSegment>()))
                    seg.TryBreak(color, false, null);                    
            } else {
                // Pathetic beam-thing
                var increment = 60;

                for (var i = 0f; i < 360f; i += increment)
                    for (var j = 0f; j < 360f; j += increment) {
                        if (Physics.Raycast(transform.position, Quaternion.Euler(i, j, 0) * transform.forward, out var hit, explosionRadius)) {
                            var seg = hit.collider.GetComponent<RootSegment>();

                            if (seg != null)
                                seg.TryBreak(color, false);
                        }
                    }
            }

            // Destroy component, but not the GameObject
            Destroy(this);
        }

        private void Update() {
            // Manually detonated?
            var detonatorInputPressed = detonatorButton switch {
                VrButton.A => InputBridge.Instance.AButtonDown,
                VrButton.B => InputBridge.Instance.BButtonDown,
                VrButton.X => InputBridge.Instance.XButtonDown,
                VrButton.Y => InputBridge.Instance.YButtonDown,
                VrButton.LeftTrigger => InputBridge.Instance.LeftTriggerDown,
                VrButton.RightTrigger => InputBridge.Instance.RightTriggerDown,
                _ => false
            };

            if (detonatorInputPressed)
                Explode();
        }        
    }
}

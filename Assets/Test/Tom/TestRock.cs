using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class TestRock : MonoBehaviour, IRootBreaker {
        public float collisionForceMultiplier => 100000f;

        [field: SerializeField]
        public int color { get; set; }
        
        public bool isActiveBreaker => true;

        public bool isObliterator => false;

        public bool penaltyOnColorMismatch => false;

        public void OnBreak(RootSegment segment) {
            
        }

        public void OnMismatch(RootSegment segment) {
            
        }

        public void OnTooWeak(RootSegment segment) {
            
        }
    }
}

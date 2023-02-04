using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class Nubbin : MonoBehaviour {
        bool isGrowing;

        public float targetScale;

        private void Start() {
            isGrowing = true;

            transform.localScale = Vector3.one * targetScale;
        }

        public void Die() {
            Destroy(gameObject);
        }

        private void Update() {
            
        }
    }
}

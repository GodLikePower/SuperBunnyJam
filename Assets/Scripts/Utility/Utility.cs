using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {

    [Serializable]
    public struct Roll {
        public float max;

        public float min;

        public Roll(float min = 0f, float max = 0f) {
            this.min = min;
            this.max = max;
        }

        public float Value() => UnityEngine.Random.value * (max - min) + min;
    }
}

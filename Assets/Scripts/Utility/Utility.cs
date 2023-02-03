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

    public static class UnityExtensions {
        public static T Demand<T>(this GameObject obj) where T : Component {
            var result = obj.GetComponent<T>();

            if (result == null)
                result = obj.AddComponent<T>();

            return result;
        }

        public static T Demand<T>(this Component component) where T : Component => component.gameObject.Demand<T>();
    }
}

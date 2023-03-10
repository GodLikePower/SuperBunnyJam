using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static T Choice<T>(this IEnumerable<T> enumerable) {

            var roll = UnityEngine.Random.value;

            var count = enumerable.Count();
            var index = (int)(roll * count);
            if (index == count) // this is possible, just not terribly likely
                --index;
            return enumerable.ElementAt(index);
        }

        public static T Demand<T>(this GameObject obj) where T : Component {
            var result = obj.GetComponent<T>();

            if (result == null)
                result = obj.AddComponent<T>();

            return result;
        }

        public static V Get<K, V>(this IDictionary<K, V> dict, K key, V defaultValue) {
            if (key != null && dict.ContainsKey(key))
                return dict[key];

            return defaultValue;
        }

        public static V Get<K, V>(this IDictionary<K, V> dict, K key) {
            return dict.Get(key, default);
        }

        public static T Demand<T>(this Component component) where T : Component => component.gameObject.Demand<T>();

        // Cutting all the corners
        public static void TryPlaySound(this Component component, FMODUnity.EventReference evt) {
            if (!evt.IsNull)
                FMODUnity.RuntimeManager.PlayOneShot(evt, component.transform.position);
        }
    }
}

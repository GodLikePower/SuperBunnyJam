using UnityEngine;

namespace SuperBunnyJam {

    public abstract class Singleton<T> where T : Singleton<T> {

        public static T _instance { get; private set; }

        public Singleton() {
            Debug.Assert(_instance == null, "Multiply-found singleton");

            _instance = this as T;
        }
    }
}
using UnityEngine;

namespace SuperBunnyJam {
    public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour {

        static T _instance;

        public static T instance {
            get {
                if (_instance == null) {
                    var instances = FindObjectsOfType<T>();

                    Debug.Assert(instances.Length > 0, "Singleton not found");
                    Debug.Assert(instances.Length < 2, "Multiply-found singleton");

                    _instance = instances[0];
                }

                return _instance;
            }
        }
    }
}
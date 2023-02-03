using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class TestSpawner : MonoBehaviour {

        [SerializeField]
        GameObject prefab;

        void Start() {
            InvokeRepeating("Spawn", 1f, 1f);
        }

        void Spawn() {
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}

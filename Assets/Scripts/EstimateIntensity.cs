using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>Figures out how fucked the player is, adjusts doof</summary>
namespace SuperBunnyJam {

    [Serializable] 
    struct IntensityBracket {
        public float distance;

        public float intensity;
    }

    public class EstimateIntensity : SingletonBehavior<EstimateIntensity> {

        public float currentIntensity { get; private set; }

        [SerializeField]
        IntensityBracket[] intensityBrackets;

        Dictionary<RootSegment, float> rootsByIntensity;

        FMODUnity.StudioEventEmitter studioEventEmitter;

        [SerializeField]
        float maxIntensity;

        private void Start() {
            studioEventEmitter = FindObjectOfType<FMODUnity.StudioEventEmitter>();

            rootsByIntensity = new Dictionary<RootSegment, float>();

            InvokeRepeating("RefreshIntensity", 1f, 1f);
        }

        public void OnRootDoneGrowing(RootSegment root) {
            // Lazy kludge, just use endpoints
            var distance = Mathf.Min((root.transform.position - Main.instance.centerEyeAnchor.transform.position).magnitude,
                (root.endpoint - Main.instance.centerEyeAnchor.transform.position).magnitude);

            var i = 0;
            for (; i < intensityBrackets.Length; ++i)
                if (distance <= intensityBrackets[i].distance) {
                    // Bracket found
                    rootsByIntensity[root] = intensityBrackets[i].intensity;

                    break;
                }                        
        }

        public void OnRootDead(RootSegment root) {
            rootsByIntensity.Remove(root);            
        }

        public void RefreshIntensity() {
            Profiler.BeginSample("RefreshIntensity");

            currentIntensity = Mathf.Min(maxIntensity, rootsByIntensity.Values.Sum()) / maxIntensity;

            studioEventEmitter.SetParameter("Intensity", currentIntensity * 100f);

            //Debug.Log($"Threats: {string.Join(", ", rootsByIntensity.GroupBy(p => p.Value).Select(g => g.Count()))}, intensity {intensity * 100f}");

            Profiler.EndSample();
        }
    }
}

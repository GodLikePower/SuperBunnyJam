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

        [SerializeField]
        IntensityBracket[] intensityBrackets;

        Dictionary<RootSegment, float> rootsByIntensity;

        [SerializeField]
        float maxIntensity;

        private void Start() {
            InvokeRepeating("RefreshIntensity", 1f, 1f);
        }

        public void OnRootDoneGrowing(RootSegment root) {
            // Lazy kludge, just use endpoints
            var distance = Mathf.Min((root.transform.position - Main.instance.centerEyeAnchor.transform.position).magnitude,
                (root.endpoint - Main.instance.centerEyeAnchor.transform.position).magnitude);

            var i = 0;
            for (; i < intensityBrackets.Length; ++i)
                if (distance <= intensityBrackets[i].distance)
                    // Bracket found
                    break;

            rootsByIntensity[root] = intensityBrackets[i].intensity;
        }


        public void OnRootDead(RootSegment root) {
            rootsByIntensity.Remove(root);            
        }

        public void RefreshIntensity() {
            Profiler.BeginSample("RefreshIntensity");

            var intensity = Mathf.Min(maxIntensity, rootsByIntensity.Values.Sum());         
            
            // TODO: plug into our musical whatsit. Multiply by 100 first

            Profiler.EndSample();
        }
    }
}

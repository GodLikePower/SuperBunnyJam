using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class RootCorpse : MonoBehaviour {

        [SerializeField]
        int materialIndex;

        [SerializeField]
        new MeshRenderer renderer;
        
        public void SetMaterial(Material material) {
            var materials = renderer.sharedMaterials;
            materials[materialIndex] = material;

            renderer.sharedMaterials = materials;
        }
    }
}

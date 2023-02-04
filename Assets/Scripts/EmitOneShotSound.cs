using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace SuperBunnyJam
{
   
    public class EmitOneShotSound : MonoBehaviour
    {
        [SerializeField] 
        EventReference eventReference;
        


        public void playSFX()
        {
            RuntimeManager.PlayOneShot(eventReference, gameObject.transform.position);
        }
    }
}

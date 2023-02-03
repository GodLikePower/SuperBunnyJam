using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace SuperBunnyJam
{
    public class AudioManager : SingletonBehavior<AudioManager>
    {
        [SerializeField]
        float amount;

        EventReference Reference;

        /// <summary>
        /// Intensity needs to get a value between zero to 100 overall 
        /// </summary>
        /// <param name="IncreaseAmount"></param>
        /// <returns></returns>
        public void ChangeIntensity() 
        {
            if ( amount >=0 && amount <= 100)
            {
                
            }

        }
    }
}

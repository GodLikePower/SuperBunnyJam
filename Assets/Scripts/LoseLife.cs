using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <remarks>Does not discriminate by type or tag, must be on a layer which only interacts with dangerous things</remarks>
namespace SuperBunnyJam
{
    public class LoseLife : MonoBehaviour {

        private void OnTriggerStay(Collider other) {            
            Main.instance.isBeingDamaged = true;
        }
    }
}

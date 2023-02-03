using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class Main : SingletonBehavior<Main> {
        GameObject _centerEyeAnchor;
        
        public GameObject centerEyeAnchor {
            get {
                if (_centerEyeAnchor == null)
                    _centerEyeAnchor = GameObject.Find("CenterEyeAnchor");

                return _centerEyeAnchor;
            }
        }
    }
}

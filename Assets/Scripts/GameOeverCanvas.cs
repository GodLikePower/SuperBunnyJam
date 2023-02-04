using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam
{
    public class GameOeverCanvas : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvas;
        private void Update()
        {
            canvas.alpha = 1f - Main.instance.life;
        }
    }
}

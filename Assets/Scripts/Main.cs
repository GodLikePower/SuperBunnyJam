using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperBunnyJam {
    public class Main : SingletonBehavior<Main> {
        GameObject _centerEyeAnchor;

        [HideInInspector]
        public bool isBeingDamaged;

        bool gameOver;

        /// <remarks>Normalized. One life to live.</remarks>
        [HideInInspector]
        public float life;

        [SerializeField]
        float lifeLossRate;

        [SerializeField]
        float lifeRegenerationRate;

        public GameObject centerEyeAnchor {
            get {
                if (_centerEyeAnchor == null)
                    _centerEyeAnchor = GameObject.Find("CenterEyeAnchor");

                return _centerEyeAnchor;
            }
        }

        private void Start() {
            life = 1f;
        }

        IEnumerator LoseGame() {
            FindObjectOfType<BNG.ScreenFader>().DoFadeIn();
            yield return new WaitForSeconds(1f);

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        private void Update() {
            if (gameOver)
                return;

            if (isBeingDamaged)
                life -= Time.deltaTime * lifeLossRate;
            else
                life += Time.deltaTime * lifeRegenerationRate;

            life = Mathf.Min(1f, life);

            if (life <= 0f)
                StartCoroutine(LoseGame());
        }        
    }
}

using UnityEngine;

namespace SuperBunnyJam
{
    public class BunnyHole : MonoBehaviour
    {
        //1. Spawn a bunny
        //2. Make it pickable
        //3. If bunny is picked up, spawn a new bunny
        [Header("Bunny Components")]
        [SerializeField]
        private GameObject _bunnyPrefab;
        private GameObject _bunnyClone;

        [Header("Spawn Prameeters")]
        [SerializeField]
        private float _heightOffset = 1.3f;
        [SerializeField]
        private float _spawnDelay = 1.0f;
        private float _tempDealy = 0.0f;

        private void Start()
        {
            _tempDealy = _spawnDelay;
            SpawnBunny();
        }

        private void Update()
        {
            SpawnBunny();
        }

        private void SpawnBunny()
        {
            if (_bunnyClone == null)
            {
                _tempDealy -= Time.deltaTime;

                if (_tempDealy <= 0)
                {
                    _bunnyClone = Instantiate(_bunnyPrefab, transform.position + new Vector3(0, _heightOffset, 0), Quaternion.identity);
                    _tempDealy = _spawnDelay;
                }
            }
        }

        private void GrabbableOnGrabBegin()
        {
            _bunnyClone = null;
        }
    }
}

using BNG;
using UnityEngine;

namespace SuperBunnyJam
{
    public class BunnyHole : MonoBehaviour
    {
        [Header("Bunny Components")]
        [SerializeField]
        private GameObject _bunnyPrefab;
        private GameObject _bunnyClone;

        [Header("Spawn Prame")]
        [SerializeField]
        private float _heightOffset = 1.3f;
        [SerializeField]
        private float _spawnDelay = 1.0f;
        private float _tempDealy = 0.0f;

        private bool _isSpawnable = true;

        private void Start()
        {
            _tempDealy = _spawnDelay;
            SpawnBunny();
        }

        private void Update()
        {
            SpawnBunny();
        }

        private void OnTriggerExit(Collider other)
        {
            _isSpawnable = other.CompareTag("Bunny");
            
        }

        private void SpawnBunny()
        {
            if (_isSpawnable)
            {
                _tempDealy -= Time.deltaTime;

                if (_tempDealy <= 0)
                {
                    _bunnyClone = Instantiate(_bunnyPrefab, transform.position + new Vector3(0, _heightOffset, 0), Quaternion.identity);
                    _tempDealy = _spawnDelay;
                    _isSpawnable = false;
                }
            }
        }
    }
}

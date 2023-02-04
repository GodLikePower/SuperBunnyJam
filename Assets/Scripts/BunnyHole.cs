using UnityEngine;
using MoreMountains.Feedbacks;

namespace SuperBunnyJam
{
    public class BunnyHole : MonoBehaviour
    {
        [Header("Bunny Components")]
        [SerializeField]
        private GameObject _bunnyPrefab;
        [SerializeField]
        MMF_Player _pullBunnyFeedback;

        [Header("Spawn Prame")]
        [SerializeField]
        private float _heightOffset = 1.3f;
        [SerializeField]
        private float _spawnDelay = 1.0f;
        private float _tempDealy = 0.0f;
        private bool _isSpawnable = true;
        [SerializeField]
        LayerMask _bunnyLayer;

        private void Start()
        {
            _tempDealy = _spawnDelay;
            SpawnBunny();
        }

        private void Update()
        {
            SpawnBunny();
        }

        private void FixedUpdate()
        {
            if(Physics.Raycast(transform.position + Vector3.down * 2 , transform.up, 5, _bunnyLayer))
            {
                _isSpawnable = false;
            }
            else
            {  
                _isSpawnable = true;
            }
        }

        private void SpawnBunny()
        {
            if (_isSpawnable)
            {
                _tempDealy -= Time.deltaTime;

                if (_tempDealy <= 0)
                {
                    Instantiate(_bunnyPrefab, transform.position + new Vector3(0, _heightOffset, 0), Quaternion.identity).transform.parent = transform;
                    _pullBunnyFeedback.PlayFeedbacks();
                    _tempDealy = _spawnDelay;
                    _isSpawnable = false;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * 5);
        }
    }
}

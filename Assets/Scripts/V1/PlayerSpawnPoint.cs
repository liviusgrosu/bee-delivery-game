using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        public static PlayerSpawnPoint Instance { get; private set; }
        public GameObject PlayerPrefab;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SpawnPlayer()
        {
            Instantiate(PlayerPrefab,  transform.position, transform.rotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && GameManager.Instance.IsDoneLevel)
            {
                UIManager.Instance.TriggerGameWinScreen();
            }
        }
    }
}
using UnityEngine;

namespace Entity {

    public class DestroyParentAfterTime : MonoBehaviour
    {
        public float delay = 1f; // Čas zpoždění v sekundách

        private void Start()
        {
            Invoke("DestroyParent", delay); // Spustí funkci DestroyParent() po zadaném zpoždění
        }

        private void DestroyParent()
        {
            if (transform.parent != null) // Pokud má objekt rodiče
            {
                Destroy(transform.parent.gameObject); // Zničí rodiče
            }
        }
    }

}
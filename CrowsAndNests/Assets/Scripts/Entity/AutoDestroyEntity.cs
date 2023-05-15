using UnityEngine;

namespace Enity {
    
    /// <summary>
    /// Zajistuje automaticke odstaneni objektu ze sceny po uplynutem case
    /// </summary>
    public class AutoDestroyEntity : MonoBehaviour
    {
        public float destroyTime = 2f; /** doba po které se objekt má odstranit */

        void Start()
        {
            // spustíme počítání času pro odstranění objektu
            Destroy(gameObject, destroyTime);
        }
    }

}
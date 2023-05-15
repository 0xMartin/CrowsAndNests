using UnityEngine;

namespace Enity {
    public class CollisionDetection  : MonoBehaviour
    {
        public delegate void CollisionDelegate();
        public event CollisionDelegate OnCollisionDetected;

        private void OnCollisionEnter(Collision collision)
        {
            // Vyvolání události po detekci kolize
            if (OnCollisionDetected != null) 
            {
                OnCollisionDetected();
            }
        }
    }

}
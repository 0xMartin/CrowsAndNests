using UnityEngine;

namespace Entity {

    public class FastFall : MonoBehaviour
    {
        public float fallSpeed = 10f; 

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            rb.velocity = new Vector3(rb.velocity.x, -fallSpeed, rb.velocity.z);
        }
    }

}
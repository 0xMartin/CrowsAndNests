using UnityEngine;

namespace Enity {

    public class LightIntensity : MonoBehaviour
    {
        public float frequency = 3f; 
        public float minIntensity = 10f;
        public float maxIntensity = 25f;

        private Light pointLight;
        private float time; 

        private void Start()
        {
            this.pointLight = GetComponent<Light>();
        }

        private void Update()
        {
            time += Time.deltaTime;
            pointLight.intensity = Mathf.Sin(time * 2 * Mathf.PI * frequency) * (maxIntensity - minIntensity) + minIntensity;
        }
    }

}
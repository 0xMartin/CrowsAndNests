using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableEgg : MonoBehaviour
{

    public float minForce;
    public float maxForce;
    public float radius;
    public Transform playerTransform;

    [Header("References")]
    public ParticleSystem dustParticle;
 
    private bool isBroken;

    private void Start() {
        isBroken = false;

        // daktivuje gravitaci pro casti vejce
        foreach(Transform t in transform) {
            Rigidbody rg = t.GetComponent<Rigidbody>();
            if(rg == null) continue;
            rg.isKinematic = true;
        }
    }

    private void Update()
    {
        if(!isBroken && Input.GetMouseButtonDown(0)) {
            // vzdalenost od hrace
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            // uhel (je 0 pokud se hrac diva primo celem na objekt)
            Vector3 direction = (transform.position - playerTransform.position).normalized;
            float angle = Vector3.Angle(playerTransform.forward, direction);

            // kontrola vzdalenosti + kontrola uhlu 
            if(distance < 3.5 && angle < 35f) {
                Invoke(nameof(breaEgg1), 0.15f);
            }
        }
    }
    
    private void breaEgg1()
    {
        isBroken = true;

        //dust fx
        if(dustParticle != null) {
            dustParticle.Play();
        }

        Invoke(nameof(breakEgg2), 0.1f);
    }

    private void breakEgg2() {
        // aktivuje gravitaci pro casti vejce a odhodi je do okoli
        foreach(Transform t in transform) {
            Rigidbody rg = t.GetComponent<Rigidbody>();
            if(rg == null) continue;
            rg.isKinematic = false;
            rg.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
        }
    }

    public bool isEggBroken() {
        return this.isBroken;
    }

}

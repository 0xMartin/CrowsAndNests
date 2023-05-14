using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zajistuje rozbiti vejce. Je mozne pouzit pro libovolny typ objetu, jehoz model se sklad z vice mensich casti.
/// </summary>
public class BreakableEgg : MonoBehaviour
{

    public float minForce; /** minimalni sila */
    public float maxForce; /** maxilani sila */
    public float radius; /** radius posobeni sily rozbity vejce */

    [Header("References")]
    public Transform playerTransform; /** pozice hrace */
    public ParticleSystem dustParticle; /** efekt rozbiti vejce */
    public GameObject breakSoundObj; /** objekt se zvukem rozbitim vejce */
 
    private bool isBroken; /** stav zda je vejce uz rozbite*/

    private AudioSource breakSound; /** zvuk s rozbitim vejce */

    // delegat pro rozbiti vejce
    public delegate void EggBreakAction(BreakableEgg egg); 
    public EggBreakAction EggBreakCallback { get; set; }

    private void Start() {
        isBroken = false;

        this.breakSound = this.breakSoundObj.GetComponent<AudioSource>();

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
    
    /// <summary>
    /// Prvni faze rozbiti vejce. Jen aktivuje efekt a po chvili vyvola jeho roztristeni na skorapky.
    /// </summary>
    private void breaEgg1()
    {
        isBroken = true;

        // zvuk
        this.breakSound.Play();

        //dust fx
        if(dustParticle != null) {
            dustParticle.Play();
        }

        // invoke callback
        this.EggBreakCallback.Invoke(this);

        Invoke(nameof(breakEgg2), 0.1f);
    }

    /// <summary>
    /// Druha vaze rozbiti vejce. Roztristeni na mensi casti. Model vejce je jiz setaven z nekolika 
    /// mensich casti, kterym se jen aktivuje fyzika a aplikuje se na ne sila exploze.
    /// </summary>
    private void breakEgg2() {
        // aktivuje gravitaci pro casti vejce a odhodi je do okoli
        foreach(Transform t in transform) {
            Rigidbody rg = t.GetComponent<Rigidbody>();
            if(rg == null) continue;
            rg.isKinematic = false;
            rg.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
        }
    }

    /// <summary>
    /// Je vejce rozbite?
    /// </summary>
    /// <returns>True -> vejce je rozbite</returns>
    public bool isEggBroken() {
        return this.isBroken;
    }

}

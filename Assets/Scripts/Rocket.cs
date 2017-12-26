using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public float speed;
    public Transform target;
    public float damage;
    public ObjectPool pool;

    public ParticleSystem ps;
    public SpriteRenderer sr;
    private bool detonated;

    

    private void Start()
    {
        //ps = GetComponent<ParticleSystem>();
        //sr = GetComponent<SpriteRenderer>();
    }

    public void StartFlying(Transform goal, float dmg)
    {
        target = goal;
        damage = dmg;
        detonated = false;
        sr.enabled = true;
        StartCoroutine(Fly());

    }

    IEnumerator Fly()
    {
        while (true)
        {
            CheckTarget();

            Vector3 destination = target.position;
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            Utilities.RotateSprite(transform, target.position, -90);

            yield return new WaitForFixedUpdate();
        }
    }

    private void CheckTarget()
    {
        if (!target.gameObject.activeSelf)
        {
            if(WaveControl.Instance.activeEnemies.Count == 0)
            {
                target = null;
                detonated = true;
                StopAllCoroutines();
                StartCoroutine(Explode());
            }
            else
                target = WaveControl.Instance.activeEnemies[0].transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !detonated)
            if(other.transform == target)
            {
                detonated = true;
                StopAllCoroutines();
                StartCoroutine(Explode());
            }                
    }

    IEnumerator Explode()
    {
        if(target != null)
            target.GetComponent<Enemy>().TakeDamage(damage);
        ps.Emit(50);
        sr.enabled = false;
        yield return new WaitForSeconds(1.0f);
        GameLogic.Instance.rocketPool.ReturnObject(gameObject);
    }

}
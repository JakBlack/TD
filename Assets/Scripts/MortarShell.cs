using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarShell : MonoBehaviour
{
    public List<Enemy> enemies;
    private ParticleSystem pS;

    void Start()
    {
        enemies = new List<Enemy>();
        pS = GetComponent<ParticleSystem>();
    }

    public void Boom(float damage, float radius)
    {
        transform.localScale = new Vector3(radius, radius, 1);
        StartCoroutine(Explode(damage));
    }

    IEnumerator Explode(float damage)
    {
        yield return new WaitForFixedUpdate();
        foreach(Enemy e in enemies)
        {
            e.TakeDamage(damage);
        }
        pS.Emit(100);
        yield return new WaitForSeconds(0.5f);
        enemies.Clear();
        GameLogic.Instance.shellPool.ReturnObject(gameObject);
    }

    IEnumerator WaiForStart()
    {
        while (enemies == null)
            yield return new WaitForFixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy")
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }
}

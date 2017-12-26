using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public NavMeshAgent navComponent;
    public ObjectPool enemyPool;
    public ParticleSystem fireSystem;
    public float hp;
    private short points;
    public bool airborne;
    public List<BasicTile> onTiles;
    public List<Tower> towers;

    public List<float> multipliers;
    public float currentMultiplier;
    private float defaultSpeed;

    public BasicTile goal;

    public Color slowColor;

    private Coroutine activeMovement;
    private short activeFires;

    private float fireCounter;
    
    private void Start()
    {
        multipliers = new List<float>();
        onTiles = new List<BasicTile>();
        towers = new List<Tower>();
    }

    public void Teleport()
    {
        if (goal != null)
            transform.position = goal.transform.position;
    }

    public void GoTo(Transform goal)
    {
        navComponent.SetDestination(goal.position);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            if (damage < 1000)
                GameLogic.Instance.ChangePoints(points);

            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForEndOfFrame();

        foreach (BasicTile tile in onTiles)
            tile.CheckCollider(GetComponent<Collider>());

        foreach (Tower tower in towers)
            tower.RemoveFromTargets(this);

        onTiles.Clear();
        towers.Clear();

        WaveControl.Instance.activeEnemies.Remove(this);
        fireSystem.Stop();
        StopAllCoroutines();

        GetComponent<PooledObject>().pool.ReturnObject(gameObject);
    }

    public void Slow(float multiplier)
    {
        multipliers.Add(multiplier);
        if(multiplier < currentMultiplier)
        {
            currentMultiplier = multiplier;
            SetSpeed();
            spriteRenderer.color = slowColor;
        }
    }

    public void UnSlow(float multiplier)
    {
        multipliers.Remove(multiplier);
        if(multipliers.Count > 0)
        {
            if(currentMultiplier == multiplier)
                currentMultiplier = multipliers.Min();
        }
        else
        {
            currentMultiplier = 1;
            spriteRenderer.color = Color.white;
        }

        SetSpeed();
    }

    public void StartMoving(Vector3 goal)
    {
        activeMovement = StartCoroutine(Move(goal));
    }

    public void StopMoving()
    {
        UnityEngine.Debug.Log("STOP, THOT!");
        StopCoroutine(activeMovement);
    }

    IEnumerator Move(Vector3 startingPosition)
    {
        UnityEngine.Debug.Log("Started moving!");
        Vector3 destination;
        if (startingPosition == Vector3.zero)
        {
            destination = onTiles[0].next;
            if (onTiles[0].nextTile != null)
                onTiles[0].nextTile.headingHere.Add(this);
        }
        else
        {
            destination = startingPosition;
        }
        while (true)
        {
            transform.LookAt(destination);
            while ((transform.position - destination).magnitude > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, defaultSpeed * currentMultiplier * Time.deltaTime);
                yield return new WaitForFixedUpdate();
            }
            UnityEngine.Debug.Log("Got to destination!");
            destination = onTiles[0].next;
            onTiles[0].headingHere.Remove(this);
            if(onTiles[0].nextTile != null)
                onTiles[0].nextTile.headingHere.Add(this);
        }
    }

    public void SetOnFire(float duration, float delay, float dmg)
    {
        activeFires++;
        StartCoroutine(OnFire(duration, delay, dmg));
        UnityEngine.Debug.Log("ON FIRE!");
        fireSystem.Play();
    }

    IEnumerator OnFire(float duration, float delay, float dmg)
    {
        while(duration > 0)
        {
            TakeDamage(dmg);
            yield return new WaitForSeconds(delay);
            duration -= Time.deltaTime;
        }
        PutOutFire();
    }

    private void PutOutFire()
    {
        activeFires--;

        if (activeFires == 0)
        {
            UnityEngine.Debug.Log("No longer on fire!");
            spriteRenderer.color = Color.white;
            fireSystem.Stop();
        }            
    }

    private void SetSpeed()
    {
        navComponent.speed = defaultSpeed * currentMultiplier;
    }

    public void Setup(EnemyItem config)
    {
        spriteRenderer.sprite = config.sprite;
        navComponent.speed = config.speed;
        hp = config.hp;
        points = config.points;
        airborne = config.airborne;

        activeFires = 0;
        defaultSpeed = config.speed;
        multipliers.Clear();
        currentMultiplier = 1;
        SetSpeed();
        spriteRenderer.color = Color.white;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Tower")
        {
            towers.Add(other.GetComponent<Tower>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Tower")
        {
            towers.Remove(other.GetComponent<Tower>());
        }

        if(other.tag == "Tile")
        {
            BasicTile tile = other.GetComponent<BasicTile>();
            if (onTiles.Contains(tile))
            {
                tile.CheckCollider(GetComponent<Collider>());
                onTiles.Remove(tile);
            }
        }
    }
}
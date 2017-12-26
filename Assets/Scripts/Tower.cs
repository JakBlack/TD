using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public Transform radiusObect;
    public SpriteRenderer gun;
    public Enemy currentTarget;
    public TowerItem currentItem;
    public List<Enemy> targets = new List<Enemy>();
    public ParticleSystem partSys;
    
    private bool fireIsRunning;
    private bool facesEnemy;

    private void Awake()
    {
        facesEnemy = false;
        fireIsRunning = false;
        currentTarget = null;
    }

    // And divide into two coroutines for default tower and slow tower
    IEnumerator Fire()
    {
        // Bool to make sure that there's only one coroutine of this type running at any time
        fireIsRunning = true;
        // Coroutine will stop if there's no targets in range
        while(targets.Count > 0)
        {
            // Pick new target if the last one was destroyed in tower's radius
            if (!currentTarget.gameObject.activeSelf)
            {
                targets.Remove(currentTarget);
                if (targets.Count == 0)
                    currentTarget = null;
                else
                    currentTarget = targets[0];
            }

            if(currentTarget != null)
            {
                //Utilities.RotateSprite(gun.transform, currentTarget.transform.position, -90);
                Shoot(currentItem.type);        
            }
                
            yield return new WaitForSeconds(currentItem.fireDelay);
        }
        Debug.Log("Ran out of targets!");
        fireIsRunning = false;
    }

    private void Shoot(short towerType)
    {
        switch (towerType)
        {
            case 2:
                currentTarget.SetOnFire(currentItem.fireDuration, currentItem.fireDelay, currentItem.damage);
                break;
            case 4:
            case 3:
                GameObject rocket = GameLogic.Instance.rocketPool.GetObject(MyGrid.Instance.transform, false);

                Utilities.RotateSprite(rocket.transform, currentTarget.transform.position, -90);
                rocket.transform.position = transform.position;

                rocket.SetActive(true);
                rocket.GetComponent<Rocket>().StartFlying(currentTarget.transform, currentItem.damage);
                break;
            case 5:
                GameObject shell = GameLogic.Instance.shellPool.GetObject(MyGrid.Instance.transform, false);
                shell.transform.position = currentTarget.transform.position;
                shell.SetActive(true);
                shell.GetComponent<MortarShell>().Boom(currentItem.damage, 3f);
                break;
            default:
                currentTarget.TakeDamage(currentItem.damage * GameLogic.Instance.damageMultiplier);
                partSys.Emit(currentItem.upgradeLevel + 1);
                break;
        }
    }

    IEnumerator FaceTarget()
    {
        facesEnemy = true;
        while(currentTarget != null)
        {
            Utilities.RotateSprite(gun.transform, currentTarget.transform.position, -90);

            yield return new WaitForEndOfFrame();
        }
        facesEnemy = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " entered tower's radius!");

        if (other.tag != "Enemy")
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (!targets.Contains(enemy))
        {
            if (currentItem.type == 1 || currentItem.type == 4)
            {
                if (enemy.airborne)
                    AddEnemy(enemy);
            }
            else if (currentItem.type == 2 || currentItem.type == 5)
            {
                if (!enemy.airborne)
                    AddEnemy(enemy);
            }
            else
            {
                AddEnemy(enemy);
            }
        }
    }

    private void AddEnemy(Enemy enemy)
    {
        targets.Add(enemy);

        if(currentItem.type == 6)
        {
            enemy.Slow(currentItem.speedMultiplier);
            return;
        }

        if (currentTarget == null)
            currentTarget = enemy;

        if (!fireIsRunning)
            StartCoroutine(Fire());

        if (!facesEnemy)
            StartCoroutine(FaceTarget());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Enemy")
            return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (targets.Contains(enemy))
            RemoveFromTargets(enemy);

        if(currentItem.type == 6)
            enemy.UnSlow(currentItem.speedMultiplier);
    }

    public void RemoveFromTargets(Enemy enemy)
    {
        if (targets.Contains(enemy))
            targets.Remove(enemy);

        if (currentTarget == enemy)
        {
            if (targets.Count > 0)
                currentTarget = targets[0];
            else
                currentTarget = null;
        }
    }

    public void Setup(TowerItem config) 
    {
        if(currentItem != null)
            if (currentItem.type == 6) { }
                foreach (Enemy enemy in targets)
                    enemy.UnSlow(currentItem.speedMultiplier);


        currentItem = config;
        GetComponent<SpriteRenderer>().sprite = config.baseSprite;
        GetComponent<SphereCollider>().radius = config.range * 2;
        radiusObect.localScale = new Vector3(config.range, config.range, 1);
        gun.sprite = config.gunSprite;

        if (fireIsRunning)
        {
            StopCoroutine(Fire());
            StartCoroutine(Fire());
        }

        if (currentItem.type == 6 && targets.Count > 0)
            foreach (Enemy enemy in targets)
                enemy.Slow(currentItem.speedMultiplier);
    }

    public void Sell()
    {
        if(targets.Count > 0)
        {
            if(currentItem.type == 6)
            {
                foreach (Enemy enemy in targets)
                    enemy.UnSlow(currentItem.speedMultiplier);

                targets.Clear();
            }
            else
            {
                StopAllCoroutines();
                fireIsRunning = false;
                facesEnemy = false;
                currentTarget = null;
                targets.Clear();
                partSys.Stop();
            }
        }

        currentItem = null;
    }

    public void RadiusVisible(bool visible)
    {
        radiusObect.GetComponent<SpriteRenderer>().enabled = visible;
    }

    private void OnEnable()
    {
        gun.transform.localEulerAngles = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public ObjectPool enemyPool;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            GameLogic.Instance.LoseHealth(1);
            other.GetComponent<Enemy>().TakeDamage(9999);
        }
    }
}

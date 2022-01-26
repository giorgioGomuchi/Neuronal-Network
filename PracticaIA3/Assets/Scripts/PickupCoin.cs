using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCoin : MonoBehaviour
{
    public ObjectPooler pool;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Character")
        {
            ScoreManager.singleton.AddScore();
            pool.AddDesactiveObject(gameObject);
            gameObject.SetActive(false);
        }
    }
}

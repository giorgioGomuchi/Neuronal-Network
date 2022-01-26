using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDestroyer : MonoBehaviour
{
    public ObjectPooler pool;

    private Transform tr;

    private float size;

    private float offset = 6;

    void Start()
    {
        tr = transform;
        size = GetComponent<BoxCollider2D>().size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (tr.position.x + size + offset < Camera.main.transform.position.x)
        {
            if (pool)
            {
                pool.AddDesactiveObject(gameObject);
                if (tr.tag == "Platform")
                {
                    Gestor.singleton.RemovePlatform(this);
                }else if (tr.tag == "Coin")
                {
                    Gestor.singleton.RemoveCoin(this);
                }
                else if (tr.tag == "KillZone")
                {
                    Gestor.singleton.RemoveSpike(this);
                }
            }
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    private ObjectPooler pool;

    [SerializeField]
    private float distanceBetweenCoins;

    public float amountCoins = 0;

    void Start()
    {
        pool = GetComponent<ObjectPooler>();
    }

    public void SpawnCoins(Vector3 startPosition)
    {
        float distance = 0;

        if (amountCoins > 1)
        {
            distance -= (Mathf.Round(amountCoins / 2)-1) * distanceBetweenCoins;
        }

        for (int i = 0; i < amountCoins; i++)
        {
            GameObject coin = pool.GetPooledObject();
            if (coin)
            {
                coin.transform.position = new Vector3(startPosition.x + distance, startPosition.y, startPosition.z);
                coin.transform.localPosition = new Vector3(coin.transform.localPosition.x, coin.transform.localPosition.y, 34);
                coin.GetComponent<PickupCoin>().pool = pool;
                coin.GetComponent<PlatformDestroyer>().pool = pool;
                coin.SetActive(true);
                Gestor.singleton.AddCoin(coin.GetComponent<PlatformDestroyer>());
                distance += distanceBetweenCoins;
            }
            else
            {
                break;
            }
        }
    }
}

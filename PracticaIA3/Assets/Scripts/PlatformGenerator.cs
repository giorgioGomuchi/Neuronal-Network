using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    private ObjectPooler pool;

    [SerializeField]
    private CoinGenerator coinGenerator;
    [SerializeField]
    private SpikeGenerator spikeGenerator;

    [SerializeField]
    private float distanceBetweenMin;
    [SerializeField]
    private float distanceBetweenMax;

    private float distanceBetween;

    [SerializeField]
    private Transform trGenerationPoint;

    private Transform tr;

    private float platformWidth;

    [SerializeField]
    private Transform maxHeightPoint;
    private float minHeight;
    private float maxHeight;
    [SerializeField]
    private float maxHeightChange;
    private float heightChange;

    void Start()
    {
        tr = transform;
        pool = GetComponent<ObjectPooler>();
        minHeight = tr.position.y;
        maxHeight = maxHeightPoint.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (tr.position.x < trGenerationPoint.position.x)
        {
            GameObject obejctpool = pool.GetPooledObject();
            if (obejctpool)
            {
                obejctpool.GetComponent<PlatformDestroyer>().pool = pool;

                Gestor.singleton.AddPlatform(obejctpool.GetComponent<PlatformDestroyer>());

                distanceBetween = Random.Range(distanceBetweenMin, distanceBetweenMax);

                platformWidth = obejctpool.GetComponent<BoxCollider2D>().size.x;

                heightChange = tr.position.y + Random.Range(-maxHeightChange, maxHeightChange);

                if (heightChange > maxHeight)
                {
                    heightChange = maxHeight;
                }
                else if(heightChange < minHeight)
                {
                    heightChange = minHeight;
                }

                tr.position = new Vector3(tr.position.x + distanceBetween /*+ platformWidth*/, heightChange, tr.position.z);

                coinGenerator.amountCoins = Mathf.Round(Random.Range(0, platformWidth-1));
                
                coinGenerator.SpawnCoins(new Vector3(tr.position.x, tr.position.y + 1f, tr.position.z));

                //spikeGenerator.amountSpikes = Mathf.Round(Random.Range(0, 2));
                //Debug.Log(spikeGenerator.amountSpikes);
                //spikeGenerator.size = platformWidth;
                //spikeGenerator.SpawnSpikes(new Vector3(tr.position.x, tr.position.y + 0.5f, tr.position.z));

                obejctpool.transform.position = tr.position;
                obejctpool.SetActive(true);
            }
        }
    }
}

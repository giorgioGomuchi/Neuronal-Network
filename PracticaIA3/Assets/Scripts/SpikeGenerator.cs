using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGenerator : MonoBehaviour
{
    private ObjectPooler pool;

    [SerializeField]
    private float distanceBetweenSpikes;

    public float amountSpikes = 1;

    public float size = 0;

    void Start()
    {
        pool = GetComponent<ObjectPooler>();
    }

    public void SpawnSpikes(Vector3 startPosition)
    {
        float distance = Random.Range(Mathf.Round(-size/2)+1, Mathf.Round(size/2)-1);

        for (int i = 0; i < amountSpikes; i++)
        {
            GameObject spike = pool.GetPooledObject();
            if (spike)
            {
                spike.transform.position = new Vector3(startPosition.x + distance, startPosition.y, startPosition.z);
                spike.transform.localPosition = new Vector3(spike.transform.localPosition.x, spike.transform.localPosition.y, 34);
                spike.GetComponent<PlatformDestroyer>().pool = pool;
                spike.SetActive(true);
                Gestor.singleton.AddSpike(spike.GetComponent<PlatformDestroyer>());
                distance += distanceBetweenSpikes;
            }
            else
            {
                break;
            }
        }
    }
}

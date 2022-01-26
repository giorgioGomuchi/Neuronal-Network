using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestor : MonoBehaviour
{
    public static Gestor singleton;

    [SerializeField]
    private Transform platformGenerator;
    private Vector3 platformStartPoint;

    [SerializeField]
    private PlayerCotnroller controller;
    private Vector3 playerStartPoint;

    [SerializeField]
    private GameObject SensorDetector;
    private Vector3 SensorDetectorPos;

    private List<PlatformDestroyer> platformList;
    private List<PlatformDestroyer> coinList;
    private List<PlatformDestroyer> spikeList;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            platformList = new List<PlatformDestroyer>();
            coinList = new List<PlatformDestroyer>();
            spikeList = new List<PlatformDestroyer>();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        platformStartPoint = platformGenerator.position;
        playerStartPoint = controller.transform.position;
        SensorDetectorPos = SensorDetector.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlatform(PlatformDestroyer pd)
    {
        platformList.Add(pd);
    }

    public void RemovePlatform(PlatformDestroyer pd)
    {
        if (platformList.Contains(pd))
        {
            platformList.Remove(pd);
        }
    }

    public void AddCoin(PlatformDestroyer pd)
    {
        coinList.Add(pd);
    }

    public void RemoveCoin(PlatformDestroyer pd)
    {
        if (coinList.Contains(pd))
        {
            coinList.Remove(pd);
        }
    }
    public void AddSpike(PlatformDestroyer pd)
    {
        spikeList.Add(pd);
    }

    public void RemoveSpike(PlatformDestroyer pd)
    {
        if (spikeList.Contains(pd))
        {
            spikeList.Remove(pd);
        }
    }

    public void RestartGame()
    {
        StartCoroutine("RestartGameCo");
    }

    private IEnumerator RestartGameCo()
    {
        controller.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < platformList.Count; i++)
        {
            platformList[i].gameObject.SetActive(false);
            platformList[i].pool.AddDesactiveObject(platformList[i].gameObject);
        }
        platformList.Clear();
        for (int i = 0; i < coinList.Count; i++)
        {
            coinList[i].gameObject.SetActive(false);
            coinList[i].pool.AddDesactiveObject(coinList[i].gameObject);
        }
        coinList.Clear();
        for (int i = 0; i < spikeList.Count; i++)
        {
            spikeList[i].gameObject.SetActive(false);
            spikeList[i].pool.AddDesactiveObject(spikeList[i].gameObject);
        }
        spikeList.Clear();
        SensorDetector.transform.position = SensorDetectorPos;
        SensorDetector.GetComponent<SensorDetector>().speed = 11;
        controller.transform.position = playerStartPoint;
        platformGenerator.position = platformStartPoint;
        controller.gameObject.SetActive(true);
        ScoreManager.singleton.ResetScore();

        yield return new WaitForSeconds(0);
    }
}

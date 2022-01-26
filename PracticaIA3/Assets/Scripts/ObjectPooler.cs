using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    enum Type {Platforms, Coins, Spikes}

    [SerializeField]
    Type type = Type.Platforms;

    [SerializeField]
    private List<GameObject> pooledObjects;

    private List<GameObject> pooledInstancesDesactive;

    private Transform tr;

    // Start is called before the first frame update
    void Start()
    {
        pooledInstancesDesactive = new List<GameObject>();
        tr = transform;

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            pooledInstancesDesactive.Add(Instantiate(pooledObjects[i], tr.position, tr.rotation));
            pooledInstancesDesactive[i].transform.parent = Gestor.singleton.transform.GetChild((int)type);
            pooledInstancesDesactive[i].SetActive(false);
        }
    }

    public void AddDesactiveObject(GameObject obj)
    {
        pooledInstancesDesactive.Add(obj);
    }

    public GameObject GetPooledObject()
    {
        if (pooledInstancesDesactive.Count<=0)
        {
            return null;
        }
        int ran = Random.Range(0, pooledInstancesDesactive.Count-1);

        GameObject obj = pooledInstancesDesactive[ran];

        pooledInstancesDesactive.RemoveAt(ran);

        return obj;
    }
}

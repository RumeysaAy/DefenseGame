using System.Collections;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField][Range(0, 50)] int poolSize = 5;
    [SerializeField][Range(0.3f, 30f)] float spawnTimer = 4f; // bir düşmanın en fazla 30 saniye sonra ortaya çıkabilir

    GameObject[] pool;

    void Awake()
    {
        PopulatePool();
    }

    void Start()
    {
        // her 4 saniyede bir tane düşman oluşturulacak
        StartCoroutine(SpawnEnemy());
    }

    // performansı optimize edebilmek için havuz oluşturdum
    // bu havuzda poolSize kadar düşman nesnesi bulunur
    // düşmanlar ölürse devre dışı bırakılır, yeni oluşacak olan düşman için tekrar aktifleşir
    // yolun sonunda devre dışı bırakılan düşman, başlangıç noktasında aktifleştirilir EnemyMover OnEnable()
    // yeniden aktifleştirilen her düşmanın almış olduğu hasar sıfırlanmalı EnemyHealth OnEnable()
    void PopulatePool()
    {
        pool = new GameObject[poolSize];

        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(enemyPrefab, transform);
            pool[i].SetActive(false);
        }
    }

    void EnableObjectInPool()
    {
        // devre dışı bırakılmış düşman nesnelerini aktifleştirelim
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].activeInHierarchy == false)
            {
                pool[i].SetActive(true);
                return;
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            EnableObjectInPool(); // her spawnTimer kadar saniye sonra bir düşman aktifleştirilir
            yield return new WaitForSeconds(spawnTimer);
        }
    }
}



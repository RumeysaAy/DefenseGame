using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))] // Nesneye bu bileşeni eklediğimde mutlaka Enemy.cs bileşeni de eklenmeli.
public class EnemyMover : MonoBehaviour
{
    [SerializeField][Range(0f, 5f)] float speed = 1f; // düşmanın ne kadar hızlı hareket edeceği

    List<Node> path = new List<Node>(); // Node listesi düşmanın izleyeceği yol olacak

    Enemy enemy;
    GridManager gridManager;
    Pathfinder pathfinder;

    void OnEnable() // nesne her etkinleştirildiğinde çağrılır
    {
        ReturnToStart();
        RecalculatePath(true); // true: düşman yeniden etkinleştirildiğinde en baştadır.
    }

    void Awake()
    {
        enemy = GetComponent<Enemy>();
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    // Pathfinder.cs'den BroadcastMessage aldığımda bu metot çalıştırılacak
    // böylece mevcut düşman yeni oluşturulan yolda yürüyecek
    void RecalculatePath(bool resetPath) // yol yeniden hesaplanır
    {
        Vector2Int coordinates = new Vector2Int();

        if (resetPath) // yol sıfırlanacak mı
        {
            // düşman en başta ise
            coordinates = pathfinder.StartCoordinates;
        }
        else
        {
            // Şu anda bulunduğu düğüm başlangıç koordinatlarıdır.
            coordinates = gridManager.GetCoordinatesFromPosition(transform.position);
            // şu anda bulunduğu konumdan hedef koordinata kadar olan yolu hesaplar
        }

        // yeni bir yol bulana kadar düşmanın yolu takip etmesini durdurmak ve yeni yolu bulduktan sonra rutini yeniden başlatmaktır.
        StopAllCoroutines();

        path.Clear();
        // coordinates düşmanın ya şu andaki konumu ya da en baştaki konum
        // başlangıca göre yeni bir yol bulur
        path = pathfinder.GetNewPath(coordinates); // düşmanın gideceği yol

        StartCoroutine(FollowPath());
    }

    void ReturnToStart()
    {
        // Düşman oluşturulduğunda başlangıç noktasında konumlansın
        transform.position = gridManager.GetPositionFromCoordinates(pathfinder.StartCoordinates);
    }

    void FinishPath()
    {
        // düşman yolun sonuna ulaştığında oyuncu para kaybeder
        enemy.StealGold();

        // Düşman yolun sonuna ulaştığında devre dışı bırakılır
        gameObject.SetActive(false); // böylece havuzda yeniden kullanılabilir
    }

    IEnumerator FollowPath()
    {
        // i = 0 olduğu için mevcut yoldaki ilk düğüme geri dönüyor 
        // ama yoldaki bir sonraki düğüme gitmeli yani i=1 olmalı
        // Her saniyede, düşmanın konumu sırasıyla her Waypoint nesnesinin konumuna eşitlenecek.
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = gridManager.GetPositionFromCoordinates(path[i].coordinates);
            float travelPercent = 0f;
            /*
            Vector3.LERP(Başlangıç konumu, bitiş konumu, seyahat yüzdesi (0-1))

            Seyahat yüzdesi(travelPercent) 0 olduğunda, tamamen başlangıç konumundayız ve 1 olduğunda bitiş konumundayız. 
            Arada, örneğin 0.5 civarında ise hareketimizin ortasındayız.
            */

            // düşman hareket yönüne baksın yani her zaman yöneldiği ara noktaya doğru bakacak
            transform.LookAt(endPosition);

            while (travelPercent < 1f) // ardışık 2 noktadan birinden diğerine gidiliyor
            {
                // travelPercent değeri 1'e yaklaştıkça endPosition'e yaklaşıyorum
                travelPercent += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame(); // karenin sonunu bekleyecek
            }
        }

        // düşman yolu tamamladığında düşman devre dışı bırakılır ve oyuncu para kaybeder
        FinishPath();
    }
}

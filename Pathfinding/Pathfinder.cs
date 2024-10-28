using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    // başlangıç ve hedef koordinatları
    [SerializeField] Vector2Int startCoordinates;
    public Vector2Int StartCoordinates { get { return startCoordinates; } }

    [SerializeField] Vector2Int destinationCoordinates;
    public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

    Node startNode; // başlangıç düğümü
    Node destinationNode; // hedef düğümü
    Node currentSearchNode;

    Queue<Node> frontier = new Queue<Node>(); // komşuları ararken sıraya aldığım ancak yolumun bir parçası olup olmayacağını görmek için henüz bakmadığım tüm düğümler
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>(); // keşfedilen düğümler

    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            grid = gridManager.Grid; // sözlük
            startNode = grid[startCoordinates]; // anahtara göre değer getirir
            destinationNode = grid[destinationCoordinates];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetNewPath();
    }

    // yolu en baştan hesaplamak için
    public List<Node> GetNewPath()
    {
        return GetNewPath(startCoordinates);
    }

    // yeni yol elde etmek için başlangıç koordinatlarını aktaracağım
    public List<Node> GetNewPath(Vector2Int coordinates) // yolu yeniden hesaplamak için
    {
        gridManager.ResetNodes();
        // Bir düğüme kule yerleştirdiğimde o düğümü yürünemez yapacağım 
        // ve tekrar yol bulmak için bu fonksiyonu çağıracağım
        BreadthFirstSearch(coordinates);
        return BuildPath();
    }

    void ExploreNeighors()
    {
        // currentSearchNode'un 4 komşusunu bulacağım
        // yukarı, aşağı, sola ve sağa
        // yönleri hangi sırayla koyduğum yolumun nasıl inşa edileceğini belirleyecektir.

        List<Node> neighbors = new List<Node>();

        foreach (Vector2Int direction in directions)
        {
            // currentSearchNode koordinatlarını almak ve komşusunun koordinatlarını bulacağım
            Vector2Int neighborCoords = currentSearchNode.coordinates + direction;
            //        (2, 3)
            // (1, 2) (2, 2) (3, 2)
            //        (2, 1)
            // bu komşu koordinatların grid sözlüğünde olup olmadığını kontrol edeceğim
            // grid sözlüğündeki anahtarlar bu komşu koordinatlarını içeriyor mu?
            if (grid.ContainsKey(neighborCoords))
            {
                // grid sözlüğünde neighborCoords anahtar varsa
                // o anahtarın değeri olan Node, neighbors listesine ekledim
                neighbors.Add(grid[neighborCoords]);
            }
        }

        // 4 yöne bakılır ve komşular bulunur
        // bu komşular keşfedilecek düğümler listesine eklenir

        foreach (Node neighbor in neighbors)
        {
            // Bu komşu düğüm, ulaşılan düğümler sözlüğünde yoksa ve yürünebilirse eklenebilir
            if (!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchNode; // bağlantı ekledim

                reached.Add(neighbor.coordinates, neighbor); // keşfedildi
                frontier.Enqueue(neighbor); // keşfedilecek düğümler
            }
        }
    }

    // yol bulmak
    void BreadthFirstSearch(Vector2Int coordinates)
    {
        startNode.isWalkable = true; // yürünebilir
        destinationNode.isWalkable = true;

        // tekrar yol bulunursa temizlensin
        frontier.Clear();
        reached.Clear();

        bool isRunning = true; // hedef düğümünü bulduğumda döngüden çıkacağım

        // bfs hangi düğümden başlamak istiyorsam o düğümden başlayarak hedef düğüme giden yolu bulacak
        // grid[coordinates]
        frontier.Enqueue(grid[coordinates]); // ilettiğim koordinattaki düğümü kuyruğa ekledim.
        reached.Add(coordinates, grid[coordinates]); // ilettiğim koordinattaki düğümü keşfettim

        // komşuları arama ve sonra onları keşfetme süreci
        while (frontier.Count > 0 && isRunning) // ağaçta keşfedilecek düğümler varsa
        {
            // keşfetmek için mevcut düğüme başlangıç düğümünü eşitledim
            // ve keşfedilecek düğümlerden başlangıç düğümünü çıkarttım
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true; // keşfettim
            // mevcut düğümün komşularını keşfedilecek düğümler listesine ekleyeceğim
            ExploreNeighors();
            // eğer mevcut düğüm, hedef düğüm ise döngüden çık
            if (currentSearchNode.coordinates == destinationCoordinates)
            {
                isRunning = false;
            }
        }
    }

    List<Node> BuildPath()
    {
        // hedefden başlangıç düğümüne geri dönmek ve yolu oluşturmak (varış noktasından geriye doğru arama)
        List<Node> path = new List<Node>();
        Node currentNode = destinationNode; // şu anki düğüm hedef düğüme eşittir

        path.Add(currentNode); // düğümü path isimli listeye ekledim
        currentNode.isPath = true; // düğümü listeye eklediğimden artık düğüm yolumun üzerinde

        // Mevcut düğümü hedef olarak ayarladım, şimdi sadece ona bağlı olan tüm düğümler arasında yinelemeli olarak döngü yapacağım
        // hala keşfedilecek bağlı düğümler varsa ağaçta geri gitmeye devam et
        while (currentNode.connectedTo != null)
        {
            // currentNode.connectedTo: mevcut düğümün bağlı olduğu düğüm
            currentNode = currentNode.connectedTo; // bir adım geriye

            // bu düğümü yola ekleyeceğim 
            path.Add(currentNode); // düğümü path isimli listeye ekledim
            currentNode.isPath = true; // düğümü listeye eklediğimden artık düğüm yolumun üzerinde
        }

        // oluşturduğum yolu tersine çevireceğim
        path.Reverse();

        return path; // yol
    }

    public bool WillBlockPath(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates)) // eğer böyle bir anahtar varsa
        {
            bool previousState = grid[coordinates].isWalkable;

            grid[coordinates].isWalkable = false;
            List<Node> newPath = GetNewPath();
            grid[coordinates].isWalkable = previousState;

            if (newPath.Count <= 1)
            {
                GetNewPath();
                return true;
            }
        }
        return false;
    }

    // yayın mesajını göndermekten sorumlu
    public void NotifyReceiver()
    {
        // mesaj, düşmana yolunu yeniden hesaplamasını söylemektir.
        BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver); // RecalculatePath adlı yöntemi çağırır.
        // EnemyMover.cs > RecalculatePath
        /*
        BroadcastMessage("methodName")
        Bu oyun nesnesindeki veya onun alt öğelerinden herhangi birinde bulunan 
        her MonoBehaviour'da methodName adlı yöntemi çağırır.

        SendMessageOptions.DontRequireReceiver
        alıcı yoksa hata vermesin

        RecalculatePath(false) çünkü en baştan başlamasın bulunduğu konumdan itibaren yolu hesaplasın
        */
    }
}

using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Tower towerPrefab; // tower.cs

    [SerializeField] bool isPlaceable; // bu karoya kule inşaa edebilir miyim?
    public bool IsPlaceable { get { return isPlaceable; } }

    GridManager gridManager;
    Pathfinder pathfinder;
    Vector2Int coordinates = new Vector2Int();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    void Start()
    {
        if (gridManager != null)
        {
            // bu karonun dünyadaki konumunu GridManager’ın çalışabileceği koordinata dönüştürdüm
            coordinates = gridManager.GetCoordinatesFromPosition(transform.position);
            // bu karo üzerinde yürünebilir değilse
            if (!isPlaceable)
            {
                gridManager.BlockedNode(coordinates);
            }
        }
    }

    // yalnızca tıklandığında çağrılır: OnMouseDown()
    // fare karolardan birinin üzerine her geldiğinde: OnMouseOver()

    private void OnMouseDown() // karonun üzerine tıklandıysa
    {
        // eğer bu karoya kule inşaa edilebilirse (yürünebilirse ve kulenin yerleştirilmesi yolu kapatmıyorsa)
        if (gridManager.GetNode(coordinates).isWalkable && !pathfinder.WillBlockPath(coordinates))
        {
            // kule oluşturuldu mu? hesapta yeterli para varsa oluşturulabilir
            bool isSuccessful = towerPrefab.CreateTower(towerPrefab, transform.position); // noktaya bağlı olan kule prefabriği, noktanın konumu
            // eğer kule oluşturulduysa isSuccessful = true
            if (isSuccessful)
            {
                // eğer karoya kule eklenirse o düğüm yürünemez olacak
                gridManager.BlockedNode(coordinates);
                // kule eklendiğinde oluşan yeni yolun mevcut düşman tarafından takip edilmesini sağlar
                pathfinder.NotifyReceiver(); // yeniden yol hesaplanır
            }
        }
    }
}



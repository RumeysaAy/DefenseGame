using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize; // x ile genişliği, y ile yüksekliği belirleyeceğim

    // Dünya Izgara Boyutu - UnityEditor snap ayarlarıyla eşleşmelidir.
    [Tooltip("World Grid Size - Should match UnityEditor snap settings.")]
    [SerializeField] int unityGridSize = 10;
    public int UnityGridSize { get { return unityGridSize; } }

    // dünyamı oluşturan tüm düğümleri saklayacak olan Grid adında bir sözlük
    // anahtar veri türü, değerin veri türü
    // Anahtar için koordinatları kullanacağım
    // değer ise Node türünde olacak
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    public Dictionary<Vector2Int, Node> Grid { get { return grid; } } // bütün sözlüğü geri dönderir

    void Awake()
    {
        CreateGrid(); // grid sözlüğünü dünyadaki düğümlerle dolduracağım
    }

    // sözlüğün anahtarını girdiğimde değeri döndürecek.
    public Node GetNode(Vector2Int coordinates) // tek bir düğümü geri dönderir
    {
        if (grid.ContainsKey(coordinates)) // eğer böyle bir anahtar varsa
        {
            return grid[coordinates];
        }

        // yoksa
        return null;
    }

    // bazı karolar yürünebilir olmadığını belirtmeliyim
    public void BlockedNode(Vector2Int coordinates)
    {
        // grid sözlüğünde böyle bir anahtar varsa
        if (grid.ContainsKey(coordinates))
        {
            // yürünebilir değil
            grid[coordinates].isWalkable = false;
        }
    }

    public void ResetNodes()
    {
        foreach (KeyValuePair<Vector2Int, Node> entry in grid)
        {
            entry.Value.connectedTo = null;
            entry.Value.isExplored = false;
            entry.Value.isPath = false;
        }
    }

    public Vector2Int GetCoordinatesFromPosition(Vector3 position)
    {
        // dünya konumlarını grid koordinatlarına tekrar geri dönüştürme

        Vector2Int coordinates = new Vector2Int();
        // integer'a yani tamsayıya dönüştürdüm
        coordinates.x = Mathf.RoundToInt(position.x / unityGridSize); // grid size 10
        coordinates.y = Mathf.RoundToInt(position.z / unityGridSize);

        return coordinates;
    }

    public Vector3 GetPositionFromCoordinates(Vector2Int coordinates)
    {
        Vector3 position = new Vector3();
        position.x = coordinates.x * unityGridSize;
        position.z = coordinates.y * unityGridSize;

        return position;
    }

    void CreateGrid()
    {
        // düğümler oluşturmaya ve bunları sözlüğe yerleştirmeye başlayacağım
        // (0, 0) konumundan başlamak ve ardından ızgaradaki her bir nesne arasında 
        // döngü oluşturmak ve her nesnenin konumu için yeni bir düğüm nesnesi eklemektir.

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                grid.Add(coordinates, new Node(coordinates, true)); // varsayılan olarak yürünebilir olsun
            }
        }
    }
}

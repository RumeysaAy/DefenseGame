using UnityEngine;
using TMPro;

[ExecuteAlways] // bu kod hem düzenleme modunda hem de oynatma modunda çalışacak.
// [RequireComponent(typeof(TextMeshPro))] // Nesneye bu bileşeni eklediğimde mutlaka TextMeshPro bileşeni de eklenmeli.
public class CoordinateLabeler : MonoBehaviour
{
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color blockedColor = Color.gray; // düğüm/node yürünebilir değilse
    [SerializeField] Color exploredColor = Color.yellow; // düğüm/node keşfedildiyse
    [SerializeField] Color pathColor = new Color(1f, 0.5f, 0f); // düğüm/node bulduğum yoldaysa (turuncu)

    TextMeshPro label;
    Vector2Int coordinates = new Vector2Int();
    GridManager gridManager;

    void Awake()
    {
        // GridManager türünde nesne bul
        gridManager = FindObjectOfType<GridManager>();

        // oyun başladığında bir kez çalıştıracak
        label = GetComponent<TextMeshPro>();
        label.enabled = false; // oyun modunda koordinatlar gizlensin

        DisplayCoordinates();
    }

    void Update()
    {
        // uygulama oynatılmıyorsa
        if (!Application.isPlaying)
        {
            // yalnızca düzenleme modunda yürütülecek
            DisplayCoordinates(); // geçerli koordinatları görüntülemek için
            UpdateObjectName();
            // koordinatlar düzenleme modunda görünsün
            label.enabled = true;
        }

        // oyun modu
        SetLabelColor();
        ToggleLabels();
    }

    void ToggleLabels()
    {
        // eğer c tuşuna basılırsa koordinatlar gizlensin
        if (Input.GetKeyDown(KeyCode.C))
        {
            label.enabled = !label.IsActive();
        }
    }

    void SetLabelColor()
    {
        if (gridManager == null) { return; } // gridManager var mı?

        // GetNode fonksiyonuyla grid sözlüğünde koordinata göre düğümü bulacağım
        Node node = gridManager.GetNode(coordinates);

        if (node == null) { return; }

        if (!node.isWalkable) // yürünebilir değilse
        {
            label.color = blockedColor;
        }
        else if (node.isPath) // node/düğüm bulduğum yolda mı?
        {
            label.color = pathColor;
        }
        else if (node.isExplored) // node/düğüm keşfedildi mi?
        {
            label.color = exploredColor;
        }
        else
        {
            label.color = defaultColor;
        }
    }

    void DisplayCoordinates()
    {
        // Editör kodunu kaldırdığımdan dolayı artık oyunu oluşturduğumda bu dosyayı editor klasörüne sürüklememe gerek yok

        if (gridManager == null) { return; }

        // integer'a yani tamsayıya dönüştürdüm
        coordinates.x = Mathf.RoundToInt(transform.parent.position.x / gridManager.UnityGridSize); // grid size 10
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z / gridManager.UnityGridSize);

        label.text = coordinates.x + "," + coordinates.y;
    }

    void UpdateObjectName()
    {
        // bu nesneleri hareket ettirdikçe gerçek adlarını güncellemek
        transform.parent.name = coordinates.ToString();
    }
}

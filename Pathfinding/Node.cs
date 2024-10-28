using UnityEngine;

[System.Serializable] // denetçide görüntülenebilmesi için
public class Node
{
    public Vector2Int coordinates; // Bu düğümün, dünyada veya ızgarada işgal edeceği koordinatlar olacak
    public bool isWalkable; // bu düğümde yürünebilir mi?
    public bool isExplored; // bu düğüm önceden keşfedildi mi?
    public bool isPath; // bu düğüm yolda mı?
    public Node connectedTo; // bu düğümün hangi düğüme bağlı, bu düğümün dallandığı ana düğümü tutacaktır

    public Node(Vector2Int coordinates, bool isWalkable)
    {
        this.coordinates = coordinates;
        this.isWalkable = isWalkable;
    }
}

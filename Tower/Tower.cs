using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] int cost = 75; // kulenin maliyeti
    [SerializeField] float buildDelay = 1f; // 2*buildDelay inşaa süresi

    void Start()
    {
        StartCoroutine(Build());
    }

    // kule oluşturmak için
    public bool CreateTower(Tower tower, Vector3 position)
    {
        Bank bank = FindObjectOfType<Bank>();

        if (bank == null)
        {
            return false;
        }

        // kuleyi oluşturmak için bankada kulenin maliyetine eşit veya daha fazla para olması gerekiyor
        if (bank.CurrentBalance >= cost)
        {
            // eğer yeterli para varsa kule oluşturulabilir
            Instantiate(tower.gameObject, position, Quaternion.identity);
            // kule oluşturulduktan sonra kulenin maliyeti kadar para azaltılır
            bank.Withdraw(cost);
            return true; // kule oluşturuldu
        }
        return false;
    }

    // kule inşaa zamanlayıcısı
    IEnumerator Build()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            foreach (Transform grandchild in child)
            {
                grandchild.gameObject.SetActive(false);
            }
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true); // kulenin tabanı, kulenin tepesi
            yield return new WaitForSeconds(buildDelay);
            foreach (Transform grandchild in child) // tüm torunların arasında döngü yapmadan önce bir buildDelay kadar daha bekleyeceğim.
            {
                grandchild.gameObject.SetActive(true); // parçacık sistemi
            }
        }
        // kulenin tabanı - buildDelay - kulenin tepesi - buildDelay - parçacık sistemi
        // kule tepesinin bir torunu var. parçacık sistemidir
    }
}

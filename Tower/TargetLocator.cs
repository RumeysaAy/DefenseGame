using UnityEngine;

public class TargetLocator : MonoBehaviour
{
    [SerializeField] Transform weapon; // silah kulenin üst kısmıdır(top)
    [SerializeField] ParticleSystem projectileParticles;
    [SerializeField] float range = 15f; // menzil
    Transform target; // düşman

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget(); // En Yakın Hedefi Bul
        AimWeapon();
    }

    void FindClosestTarget()
    {
        // Sahnedeki hangi düşman kuleye en yakınsa kule onu hedef alsın
        // Bunun için sahnedeki her düşmanın kuleye mesafesini karşılaştıracağım
        Enemy[] enemies = FindObjectsOfType<Enemy>(); // Enemy.cs bileşeni sadece düşmanda bulunur
        Transform closestTarget = null; // en yakın hedef
        float maxDistance = Mathf.Infinity;

        foreach (Enemy enemy in enemies)
        {
            float targetDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (targetDistance < maxDistance)
            {
                closestTarget = enemy.transform;
                maxDistance = targetDistance;
            }
        }
        target = closestTarget; // en yakın düşmanı hedef olarak belirledim
    }

    void AimWeapon()
    {
        // düşman kulenin menzilinde mi?
        float targetDistance = Vector3.Distance(transform.position, target.position);

        // silah, hedefe yani düşmana baksın
        weapon.LookAt(target); // düşman menzil dışında olsa bile

        if (targetDistance < range)
        {
            Attack(true); // düşman kulenin menzilindeyse
        }
        else
        {
            Attack(false);
        }
    }

    void Attack(bool isActive)
    {
        // hedef menzil içindeyken parçacık sistemini açmak ve düşman/hedef menzil dışındayken kapatmak 
        var emissionModule = projectileParticles.emission;
        emissionModule.enabled = isActive;
    }
}

// kulelerimizin gerçekten düşmanımıza bakmasını sağlamak
// komut dosyası hedefimizi yakalamaktan ve ardından silahını ateşlemekten sorumlu olacak


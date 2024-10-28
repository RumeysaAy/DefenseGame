using UnityEngine;

[RequireComponent(typeof(Enemy))] // Nesneye bu bileşeni eklediğimde mutlaka Enemy.cs bileşeni de eklenmeli.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHitPoints = 5;

    // Düşman öldüğünde "maxHitPoints"e miktar eklenir.
    [Tooltip("Adds amount to maxHitPoints when enemy dies.")] // difficultyRamp için açıklama
    [SerializeField] int difficultyRamp = 1; // düşmanı zamanla güçlendirmek için

    int currentHitPoints = 0;

    Enemy enemy;

    void OnEnable() // nesne her etkinleştirildiğinde çağrılır
    {
        currentHitPoints = maxHitPoints;
    }

    void Start()
    {
        enemy = GetComponent<Enemy>();

    }

    // Ballista > Particle system > Collision > Send collision messages = true ise çarpışma olduğunda çağrılır
    private void OnParticleCollision(GameObject other)
    {
        ProcessHit();
    }

    void ProcessHit()
    {
        currentHitPoints--;

        if (currentHitPoints <= 0)
        {
            // düşman eğer currentHitPoints kadar vurulursa devre dışı bırakılsın
            gameObject.SetActive(false); // böylece havuzda yeniden kullanılabilir

            // bir sonraki düşmanın ölmesi için difficultyRamp kadar daha fazla vurulması gerekiyor
            maxHitPoints += difficultyRamp; // bir sonraki düşmanın daha zor öldürülmesini sağladım

            // düşman öldüğünde para kazanılacak
            enemy.RewardGold();
        }
    }
}

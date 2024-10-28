using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Bank : MonoBehaviour
{
    [SerializeField] int startingBalance = 150;

    [SerializeField] int currentBalance;
    public int CurrentBalance { get { return currentBalance; } } // mevcut bakiyeye dışarıdan erişebilmek için

    [SerializeField] TextMeshProUGUI displayBalance;

    void Awake()
    {
        currentBalance = startingBalance;
        UpdateDisplay();
    }

    public void Deposit(int amount)
    {
        currentBalance += Mathf.Abs(amount);
        UpdateDisplay();
    }

    public void Withdraw(int amount)
    {
        currentBalance -= Mathf.Abs(amount);
        UpdateDisplay();

        // eğer mevcut bakiye sıfırdan az ise oyun kaybedilir ve sahne yeniden yüklenir.
        if (currentBalance < 0)
        {
            ReloadScene();
        }
    }

    void UpdateDisplay()
    {
        displayBalance.text = "Gold: " + currentBalance;
    }

    void ReloadScene()
    {
        // şu andaki aktif sahne
        Scene currentScene = SceneManager.GetActiveScene();
        // aktif sahnenin indeksi alındı ve bu indekse ait sahne yeniden yüklendi
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}

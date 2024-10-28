using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int goldReward = 25; // düşman öldüğünde kazanılan puan
    [SerializeField] int goldPenalty = 25; // düşman yolun sonuna ulaştığında kaybedilen değer

    Bank bank;

    // Start is called before the first frame update
    void Start()
    {
        bank = FindObjectOfType<Bank>();
    }

    public void RewardGold()
    {
        if (bank == null) { return; }
        // düşman öldürüldüğünde para kazanılacak
        bank.Deposit(goldReward);
    }

    public void StealGold()
    {
        if (bank == null) { return; }
        // düşman yolun sonuna geldiğinde para kaybedilecek
        bank.Withdraw(goldPenalty);
    }
}

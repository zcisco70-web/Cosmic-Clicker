using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockShopSection : MonoBehaviour
{
    [SerializeField]
    private string shopSection;

    [SerializeField]
    private ulong unlockigPrice;

    private bool unlocked;
    void Start()
    {
        GameManager.Instance.AddUnlockableSections(this);

        if (GetComponent<ShopButton>() != null)
        {
            shopSection = GetComponent<ShopButton>().itemName + "Section";
            if (GetComponent<ShopButton>().basePrice > 15)
                unlockigPrice = GetComponent<ShopButton>().basePrice;
            else
                unlockigPrice = 0;
        }


        unlocked = PlayerPrefs.GetInt(shopSection, 0) == 1;
        gameObject.SetActive(unlocked);
    }

    public void UnlockSection()
    {
        unlocked = true;
        PlayerPrefs.SetInt(shopSection, 1);
        gameObject.SetActive(unlocked);
    }

    public ulong GetPrice()
    {
        return unlockigPrice;
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }
}

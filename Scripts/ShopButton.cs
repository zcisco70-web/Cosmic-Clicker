using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public string itemName;
    public string description;
    public ulong basePrice;
    public ulong price;
    public ulong massPerClickItem;
    public float massPerSecondItem;
    public float massPerClickBonus;
    public float massPerSecondBonus;
    public float multiplierEffect;
    public float evolutionDiscount;
    public float itemsDiscount;
    public float cooldown;
    public float inmediateMassTime;
    public int quantity;

    public Text itemNameText;
    public Text itemDescriptionText;
    public Text priceText;
    public Text quantityText;

    [SerializeField]
    public Image cooldownImage;

    private bool unlocked;
    public enum UpgradeType
    {
        Mass,
        Energy,
        Phenomena,
        Inmediate,
        Discount,
        UpgradeRelated,
        BigBang
    }

    public UpgradeType upgradeType;


    private static readonly Dictionary<int, string[]> suffixesByLanguage = new Dictionary<int, string[]>
{
    { 0, new[] { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" } },
    { 1, new[] { "", "k", "M", "MM", "B", "T", "Cu", "Qu", "Se", "Oc", "No", "De" } },
    { 2, new[] { "", "k", "M", "MM", "B", "T", "Cu", "Qu", "Se", "Oc", "No", "De" } },
    { 3, new[] { "", "k", "M", "Md", "B", "T", "Qa", "Qi", "Se", "Oc", "No", "De" } },
    { 4, new[] { "", "k", "M", "Md", "B", "T", "Qa", "Qi", "Se", "Oc", "No", "De" } },
    { 5, new[] { "", "k", "M", "MM", "B", "T", "Qa", "Qi", "Se", "Oc", "No", "Dc" } },
    { 6, new[] { "", "k", "M", "Mrd", "B", "T", "Qa", "Qi", "Se", "Oc", "No", "De" } },
    { 7, new[] { "", "\u5343", "\u4E07", "\u4EBF", "\u5146", "\u4EAC", "\u57D7", "\uD844\uDD71", "\u7A4D", "\u6E1D", "\u6F97", "\u6B63" } },
    { 8, new[] { "", "\u5343", "\u4E07", "\u5104", "\u5146", "\u4EAC", "\u57D7", "\uD844\uDD71", "\u7A4D", "\u6E1D", "\u6F97", "\u6B63" } },
    { 9, new[] { "", "\uCC9C", "\uB9CC", "\uC5B5", "\uC870", "\uACBD", "\uD574", "\uC790", "\uC591", "\uAD6C", "\uAC04", "\uC815" } },
    { 10, new[] { "", "\u0442\u044B\u0441", "\u043C\u043B\u043D", "\u043C\u043B\u0440\u0434", "\u0442\u0440\u043B\u043D", "\u043A\u0432\u0438\u043D", "\u0441\u0435\u043A\u0441\u0442", "\u0441\u0435\u043F\u0442", "\u043E\u043A\u0442", "\u043D\u043E\u043D\u0438\u043B", "\u0434\u0435\u0446\u0438\u043B" } },
};


    private void Start()
    {
        GameManager.Instance.AddShopItem(this);
        unlocked = PlayerPrefs.GetInt(itemName + "Section", 0) == 1;

        //basePrice = ulong.Parse(PlayerPrefs.GetString(name, "0"));
        quantity = PlayerPrefs.GetInt(itemName, 0);

        //itemNameText.text = itemName;
        //itemDescriptionText.text = description;
        if (quantity <= 0)
            price = basePrice;
        else
            price = (ulong)(basePrice * Math.Pow(1.15, quantity));


        if (cooldownImage != null)
            cooldownImage.fillAmount = 0;

        itemNameText.fontSize = 16;
        itemDescriptionText.fontSize = 10;
        quantityText.fontSize = 30;
        UpdateUI();
        gameObject.SetActive(unlocked);
    }

    public void OnButtonClick()
    {
        if (GameManager.Instance.mass >= price)
        {
            GameManager.Instance.mass -= price;
            quantity++;
            GameManager.Instance.StoreAchievement(price, upgradeType.ToString(), itemName);
            price = (ulong)(basePrice * Math.Pow(1.15, quantity));
            UpdateUI();
            ApplyUpgradeEffect();
        }
    }

    private void ApplyUpgradeEffect()
    {
        switch (upgradeType)
        {
            case UpgradeType.Mass:
                // Increment passive mass generation
                if (GameManager.Instance.massPerSecond + massPerSecondItem > double.MaxValue)
                    GameManager.Instance.massPerSecond = double.MaxValue;
                else
                    GameManager.Instance.massPerSecond += massPerSecondItem;

                if (GameManager.Instance.massPerClick + massPerClickItem > ulong.MaxValue)
                    GameManager.Instance.massPerClick = ulong.MaxValue;
                else
                    GameManager.Instance.massPerClick += massPerClickItem;
                break;

            case UpgradeType.Energy:
                // Apply energy-based bonuses
                if (massPerClickBonus > 0)
                {
                    if (GameManager.Instance.massPerClick + (ulong)(massPerClickBonus * GameManager.Instance.massPerClick) > ulong.MaxValue)
                        GameManager.Instance.massPerClick = ulong.MaxValue;
                    else
                        GameManager.Instance.massPerClick += (ulong)(massPerClickBonus * GameManager.Instance.massPerClick);
                }
                if (multiplierEffect > 0)
                    GameManager.Instance.ApplyGlobalMultiplier(multiplierEffect);
                if (massPerSecondBonus > 0)
                    GameManager.Instance.massPerSecond += massPerSecondBonus * GameManager.Instance.massPerSecond;

                break;

            case UpgradeType.Phenomena:
                if (multiplierEffect > 0)
                {
                    GameManager.Instance.StartShopCooldown(this, cooldown);
                    GameManager.Instance.ApplyGlobalMultiplier(multiplierEffect, isTemporary: true, cooldown);
                }

                break;
            case UpgradeType.Inmediate:
                if (inmediateMassTime > 0)
                {
                    if (GameManager.Instance.mass + (ulong)(GameManager.Instance.massPerSecond * inmediateMassTime) > ulong.MaxValue)
                        GameManager.Instance.mass = ulong.MaxValue;
                    else
                        GameManager.Instance.mass += (ulong)(GameManager.Instance.massPerSecond * inmediateMassTime);
                }
                break;
            case UpgradeType.Discount:
                if (evolutionDiscount > 0)
                    GameManager.Instance.ReduceMassToEvolve(evolutionDiscount);
                if (itemsDiscount > 0)
                    GameManager.Instance.ReduceItemsPrice(itemsDiscount);
                break;

            case UpgradeType.UpgradeRelated:
                if (massPerClickBonus > 0)
                    GameManager.Instance.IncreaseClickMassPerUpgrade(massPerClickBonus);
                if (massPerSecondBonus > 0)
                    GameManager.Instance.IncreasePassiveMassPerUpgrade(massPerSecondBonus);
                break;
            case UpgradeType.BigBang:
                GameManager.Instance.BigBang();
                break;
        }
    }

    public void UpdateUI()
    {
        if (GameManager.Instance.suffixes)
        {
            quantityText.text = FormatNumberLocalized(quantity, GameManager.Instance.localeId);
            priceText.text = FormatLargeNumberLocalized(price, GameManager.Instance.localeId);
        }
        else
        {
            quantityText.text = quantity.ToString();
            priceText.text = price.ToString("N0");
        }

    }

    private string FormatLargeNumberLocalized(ulong value, int languageCode)
    {
        if (!suffixesByLanguage.ContainsKey(languageCode))
            languageCode = 0;

        string[] suffixes = suffixesByLanguage[languageCode];
        int suffixIndex = 0;
        double displayValue = value;

        // Divide el número hasta encontrar el sufijo adecuado
        while (displayValue >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            displayValue /= 1000.0;
            suffixIndex++;
        }

        // Formatea solo si hay necesidad de mostrar decimales
        if (displayValue >= 100)
        {
            return displayValue.ToString("0") + " " + suffixes[suffixIndex]; // Ej: 123K
        }
        else if (displayValue >= 10)
        {
            return displayValue.ToString("0.#") + " " + suffixes[suffixIndex]; // Ej: 12.3M
        }
        else
        {
            return displayValue.ToString("0.##") + " " + suffixes[suffixIndex]; // Ej: 1.23B
        }
    }
    private string FormatNumberLocalized(int value, int languageCode)
    {
        if (!suffixesByLanguage.ContainsKey(languageCode))
            languageCode = 0;

        string[] suffixes = suffixesByLanguage[languageCode];
        int suffixIndex = 0;
        double displayValue = value;

        // Divide el número hasta encontrar el sufijo adecuado
        while (displayValue >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            displayValue /= 1000.0;
            suffixIndex++;
        }

        // Formatea solo si hay necesidad de mostrar decimales
        if (displayValue >= 100)
        {
            return displayValue.ToString("0") + " " + suffixes[suffixIndex]; // Ej: 123K
        }
        else if (displayValue >= 10)
        {
            return displayValue.ToString("0.#") + " " + suffixes[suffixIndex]; // Ej: 12.3M
        }
        else
        {
            return displayValue.ToString("0.##") + " " + suffixes[suffixIndex]; // Ej: 1.23B
        }
    }
}
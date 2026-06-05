using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using static Achievement;

public class ShowAchievement : MonoBehaviour
{
    [SerializeField]
    private LocalizedString title;
    [SerializeField]
    private LocalizedString description;
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Text titleText;

    [SerializeField]
    private Text descriptionText;

    [SerializeField]
    private Image image;

    [SerializeField]
    private List<Color> colorList;

    [SerializeField]
    private Image backgroundColor;

    public void SetTitle(LocalizedString newTitle)
    {
        title = newTitle;
    }

    public void SetDescription(LocalizedString newDescription)
    {
        description = newDescription;
    }

    public void SetIcon(Sprite newIcon)
    {
        icon = newIcon;
    }

    public void SetRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.common:
                image.color = Color.gray;
                backgroundColor.color = Color.gray;
                break;
            case Rarity.rare:
                image.color = new Color(205f / 255f, 127f / 255f, 50f / 255f); // Bronce
                backgroundColor.color = new Color(205f / 255f, 127f / 255f, 50f / 255f);
                break;
            case Rarity.veryRare:
                image.color = new Color(192f / 255f, 192f / 255f, 192f / 255f); // Plata
                backgroundColor.color = new Color(192f / 255f, 192f / 255f, 192f / 255f);
                break;
            case Rarity.epic:
                image.color = new Color(255f / 255f, 215f / 255f, 0f / 255f); // Oro
                backgroundColor.color = new Color(255f / 255f, 215f / 255f, 0f / 255f);
                break;
            case Rarity.legendary:
                image.color = new Color(185f / 255f, 242f / 255f, 255f / 255f); // Azul brillante (Diamante)
                backgroundColor.color = new Color(185f / 255f, 242f / 255f, 255f / 255f);
                break;
            default:
                image.color = Color.black;
                backgroundColor.color = Color.black;
                break;
        }
    }

    public void UpdateAchievement()
    {
        titleText.text = title.GetLocalizedString();
        descriptionText.text = description.GetLocalizedString();
        image.sprite = icon;
    }
}

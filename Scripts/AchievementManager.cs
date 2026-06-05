using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [SerializeField]
    private List<Achievement> achievements = new List<Achievement>();

    [SerializeField]
    private Text hudProgressText;

    [SerializeField]
    private Sprite lockedIcon; // Icono para logros bloqueados

    private List<GameObject> hudItems = new List<GameObject>();
    private int unlockedCount = 0;

    [SerializeField]
    private Transform achievementListHud;

    [SerializeField]
    private Transform achievementListHud2;

    [SerializeField]
    private GameObject achievementPrefab;

    [SerializeField]
    public Transform hudContainer;

    [SerializeField]
    private LocalizedString lockedText;

    [SerializeField]
    private LocalizedString lockedDescription;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip audioClip;
    void Start()
    {
        InitializeHUD();
        UpdateHUD();
    }

    private void InitializeHUD()
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            GameObject hudItem = null;
            if (i < achievements.Count / 2)
                hudItem = Instantiate(achievementPrefab, achievementListHud);
            else
                hudItem = Instantiate(achievementPrefab, achievementListHud2);
            hudItem.GetComponent<ShowAchievement>().SetTitle(lockedText);
            hudItem.GetComponent<ShowAchievement>().SetDescription(lockedDescription);
            hudItem.GetComponent<ShowAchievement>().SetIcon(lockedIcon);
            hudItem.GetComponent<ShowAchievement>().UpdateAchievement();
            hudItems.Add(hudItem);
            

            if (PlayerPrefs.GetInt(i.ToString(), 0) == 0)
                achievements[i].isUnlocked = false;
            else
                achievements[i].isUnlocked = true;
        }
    }

    public void UnlockAchievement(int index)
    {
        if (index < 0 || index >= achievements.Count || achievements[index].isUnlocked)
            return;
        audioSource.PlayOneShot(audioClip);
        PlayerPrefs.SetInt(index.ToString(), 1);
        //Show Achievement
        GameObject achievement = Instantiate(achievementPrefab, hudContainer);
        achievement.GetComponent<ShowAchievement>().SetTitle(achievements[index].unlockedTitleKey);
        achievement.GetComponent<ShowAchievement>().SetDescription(achievements[index].descriptionKey);
        achievement.GetComponent<ShowAchievement>().SetIcon(achievements[index].icon);
        achievement.GetComponent<ShowAchievement>().UpdateAchievement();


        Destroy(achievement, 3f);
        //Achievements Menu
        achievements[index].isUnlocked = true;
        unlockedCount++;
        if (unlockedCount >= achievements.Count-1)
            UnlockAchievement(15);
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (achievements[i].isUnlocked)
            {
                hudItems[i].GetComponent<ShowAchievement>().SetTitle(achievements[i].unlockedTitleKey);
                hudItems[i].GetComponent<ShowAchievement>().SetDescription(achievements[i].descriptionKey);
                hudItems[i].GetComponent<ShowAchievement>().SetIcon(achievements[i].icon);
                hudItems[i].GetComponent<ShowAchievement>().SetRarity(achievements[i].rarity);
            }
            else
            {
                hudItems[i].GetComponent<ShowAchievement>().SetTitle(lockedText);
                hudItems[i].GetComponent<ShowAchievement>().SetDescription(lockedDescription);
                hudItems[i].GetComponent<ShowAchievement>().SetIcon(lockedIcon);
                //hudItems[i].GetComponent<ShowAchievement>().SetRarity(achievements[i].rarity);
            }
            hudItems[i].GetComponent<ShowAchievement>().UpdateAchievement();
        }

        //hudProgressText.text = $"{unlockedCount}/{achievements.Count} Achievements Unlocked";
    }
}

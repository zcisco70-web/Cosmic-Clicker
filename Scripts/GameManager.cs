using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] celestialStages; // Array de los diferentes cuerpos celestes

    private int currentStage = 0;

    // Celestial Object Variables
    public ulong mass = 0;
    public ulong massPerClick = 0;
    public double massPerSecond = 0;
    private double massAccumulator;

    //Text UI
    public Text massText;
    public Text massPerSecondText;

    //Evolving
    public ParticleSystem evolutionParticles;

    [SerializeField]
    private List<ShopButton> shopButtons = new List<ShopButton>();

    [SerializeField]
    private Button evolutionButton;

    [SerializeField]
    private PostProcessVolume ppVolume;



    private bool isLanguageMenu;
    private bool isExitMenu;
    private bool isRestartMenu;
    private bool isAchievementsMenu;

    [SerializeField]
    private LocalizedString localizedMassString;

    [SerializeField]
    private LocalizedString localizedPerSecondString;

    public int localeId;

    public bool suffixes;

    [SerializeField]
    private GameObject achievementsMenu;

    [SerializeField]
    private GameObject restartMenu;

    [SerializeField]
    private GameObject exitMenu;

    [SerializeField]
    private GameObject languageMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject optionsText;

    [SerializeField]
    private AchievementManager achievementManager;

    private int itemsBought;
    private ulong matterSpent;

    private ulong numberOfClicks;

    [SerializeField]
    private GameObject massPerClickText;

    [SerializeField]
    private Toggle suffixesToggle;

    [SerializeField]
    private Toggle hideOptionsToggle;

    [SerializeField]
    private Toggle massPerClickTextToggle;


    public bool showOptionsText = true;
    public bool showMatterPerClickText = true;

    private bool isUpdatingToggle = false;

    [SerializeField]
    private Slider evolutionSlider;

    [SerializeField]
    private List<UnlockShopSection> sections = new List<UnlockShopSection>();

    private ulong matterMultiplier;

    private bool endGame;

    [SerializeField]
    private Text matterText;

    private bool flashbang;

    private DateTime startTime;

    private DateTime sessionStartTime;
    private double totalTimePlayed;

    private bool endGameAnimation = false;
    private static readonly Dictionary<int, string[]> infinityByLanguage = new Dictionary<int, string[]>
{
    { 1, new[] { "Infinito" } },  // Español
    { 0, new[] { "Infinity" } },  // Inglés
    { 2, new[] { "Infinit" } },   // Catalán
    { 3, new[] { "Infinito" } },  // Italiano
    { 4, new[] { "Infini" } },    // Francés
    { 5, new[] { "Infinito" } },  // Portugués
    { 6, new[] { "Unendlich" } }, // Alemán
    { 7, new[] { "\u8D85\u9650" } }, // Chino (Infinito)
    { 8, new[] { "\u7121\u9650" } }, // Japonés (Infinito)
    { 9, new[] { "\uBBA4\uB2E4" } }, // Coreano (Infinito)
    { 10, new[] { "\u0411\u0435\u0441\u043A\u043E\u043D\u0435\u0447\u043D\u043E\u0441\u0442\u044C" } } // Ruso (Infinito)
};

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
    #region Crimen
    private static readonly Dictionary<int, string[]> statsByLanguage = new Dictionary<int, string[]>
{
    { 1, new[] { // Español
        "Tiempo jugado: ",
        "Materia total recolectada: ",
        "Materia pasiva máxima: ",
        "/s",
        "Materia por clic máxima: ",
        "Clics realizados: ",
        "Evoluciones alcanzadas: ",
        "Compras en la tienda: ",
        "Materia gastada en la tienda: "
    }},
    { 0, new[] { // Inglés
        "Time played: ",
        "Total matter collected: ",
        "Max passive matter: ",
        "/s",
        "Max matter per click: ",
        "Clicks made: ",
        "Evolutions reached: ",
        "Shop purchases: ",
        "Matter spent in shop: "
    }},
    { 2, new[] { // Catalán
        "Temps jugat: ",
        "Matèria total recollida: ",
        "Matèria passiva màxima: ",
        "/s",
        "Matèria per clic màxima: ",
        "Clics realitzats: ",
        "Evolucions assolides: ",
        "Compres a la botiga: ",
        "Matèria gastada a la botiga: "
    }},
    { 3, new[] { // Italiano
        "Tempo di gioco: ",
        "Materia totale raccolta: ",
        "Materia passiva massima: ",
        "/s",
        "Materia per clic massima: ",
        "Click effettuati: ",
        "Evoluzioni raggiunte: ",
        "Acquisti nel negozio: ",
        "Materia spesa nel negozio: "
    }},
    { 4, new[] { // Francés
        "Temps joué: ",
        "Matière totale collectée: ",
        "Matière passive maximale: ",
        "/s",
        "Matière par clic maximale: ",
        "Clics effectués: ",
        "Évolutions atteintes: ",
        "Achats en boutique: ",
        "Matière dépensée en boutique: "
    }},
    { 5, new[] { // Portugués
        "Tempo jogado: ",
        "Matéria total coletada: ",
        "Matéria passiva máxima: ",
        "/s",
        "Matéria por clique máxima: ",
        "Cliques realizados: ",
        "Evoluções alcançadas: ",
        "Compras na loja: ",
        "Matéria gasta na loja: "
    }},
    { 6, new[] { // Alemán
        "Spielzeit: ",
        "Gesammelte Gesamtmaterie: ",
        "Maximale passive Materie: ",
        "/s",
        "Maximale Materie pro Klick: ",
        "Klicks gemacht: ",
        "Erreichte Evolutionen: ",
        "Einkäufe im Laden: ",
        "Im Laden ausgegebene Materie: "
    }},
    { 7, new[] { // Chino
        "\u6E38\u620F\u65F6\u95F4: ",
        "\u603B\u6536\u96C6\u8D28\u91CF: ",
        "\u6700\u5927\u88AB\u52A8\u8D28\u91CF: ",
        "/s",
        "\u6700\u5927\u6BCF\u70B9\u51FB\u8D28\u91CF: ",
        "\u70B9\u51FB\u6B21\u6570: ",
        "\u8FBE\u6210\u7684\u8F6E\u5ED3: ",
        "\u5546\u5E97\u91CC\u7684\u8D2D\u4E70: ",
        "\u5546\u5E97\u91CC\u82B1\u8D39\u7684\u8D28\u91CF: "
    }},
    { 8, new[] { // Japonés
        "\u30D7\u30EC\u30A4\u6642\u9593: ",
        "\u7DCF\u96C6\u3081\u305F\u8CEA\u91CF: ",
        "\u6700\u5927\u81EA\u52D5\u8CEA\u91CF: ",
        "/s",
        "\u6700\u5927\u30AF\u30EA\u30C3\u30AF\u3054\u3068\u306E\u8CEA\u91CF: ",
        "\u30AF\u30EA\u30C3\u30AF\u56DE\u6570: ",
        "\u9054\u6210\u3057\u305F\u9032\u5316: ",
        "\u5E97\u3067\u306E\u8CFC\u5165: ",
        "\u5E97\u3067\u6D88\u8CBB\u3057\u305F\u8CEA\u91CF: "
    }},
    { 9, new[] { // Coreano
        "\uAC8C\uC784 \uC2DC\uAC04: ",
        "\uCD1D \uBAA8\uC740 \uC9C8\uB7C9: ",
        "\uCD5C\uB300 \uC790\uB3D9 \uC9C8\uB7C9: ",
        "/s",
        "\uCD5C\uB300 \uD074\uB9AD \uB2F9 \uC9C8\uB7C9: ",
        "\uD074\uB9AD \uC218: ",
        "\uB3C4\uB2EC\uD55C \uC804\uD22C: ",
        "\uC0C1\uC810 \uAD6C\uB9E4: ",
        "\uC0C1\uC810\uC5D0\uC11C \uC18C\uBE44\uD55C \uC9C8\uB7C9: "
    }},
    { 10, new[] { // Ruso
        "\u0412\u0440\u0435\u043C\u044F \u0438\u0433\u0440\u044B: ",
        "\u041E\u0431\u0449\u0435\u0435 \u0441\u043E\u0431\u0440\u0430\u043D\u043D\u043E\u0435 \u0432\u0435\u0449\u0435\u0441\u0442\u0432\u043E: ",
        "\u041C\u0430\u043A\u0441\u0438\u043C\u0430\u043B\u044C\u043D\u0430\u044F \u043F\u0430\u0441\u0441\u0438\u0432\u043D\u0430\u044F \u043C\u0430\u0441\u0441\u0430: ",
        "/s",
        "\u041C\u0430\u043A\u0441\u0438\u043C\u0430\u043B\u044C\u043D\u0430\u044F \u043C\u0430\u0441\u0441\u0430 \u0437\u0430 \u043A\u043B\u0438\u043A: ",
        "\u0421\u0434\u0435\u043B\u0430\u043D\u043E \u043A\u043B\u0438\u043A\u043E\u0432: ",
        "\u0414\u043E\u0441\u0442\u0438\u0433\u043D\u0443\u0442\u044B\u0435 \u044D\u0432\u043E\u043B\u044E\u0446\u0438\u0438: ",
        "\u041F\u043E\u043A\u0443\u043F\u043A\u0438 \u0432 \u043C\u0430\u0433\u0430\u0437\u0438\u043D\u0435: ",
        "\u0417\u0430\u0442\u0440\u0430\u0447\u0435\u043D\u043D\u0430\u044F \u0432 \u043C\u0430\u0433\u0430\u0437\u0438\u043D\u0435 \u043C\u0430\u0441\u0441\u0430: "
    }}
};
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadGame();

        for (int i = 0; i < celestialStages.Length; i++)
        {
            if (i != currentStage)
            {
                if (celestialStages[i].GetComponent<CelestialObject>() != null)
                    celestialStages[i].GetComponent<CelestialObject>().Disappear();
                celestialStages[i].gameObject.SetActive(false);
            }
            else
            {
                celestialStages[i].gameObject.SetActive(true);
                if (celestialStages[i].GetComponent<DissolvingController>() != null)
                    celestialStages[i].GetComponent<DissolvingController>().Revive();
            }
        }
    }
    private void Start()
    {
        totalTimePlayed = PlayerPrefs.GetFloat("TotalTimePlayed", 0);

        sessionStartTime = DateTime.Now;

        endGame = PlayerPrefs.GetInt("endGame", 0) == 1;

        flashbang = PlayerPrefs.GetInt("Flash", 0) == 1;

        if (flashbang)
            StartCoroutine(FlashScreen());

        PlayerPrefs.SetInt("Flash", 0);

        ChangeLocaleHover(PlayerPrefs.GetInt("LocaleKey", 0));

        if (PlayerPrefs.GetInt("LanguageMenu", 0) == 0)
        {
            isLanguageMenu = true;
            languageMenu.SetActive(true);
            ppVolume.enabled = true;
        }
        else
        {
            isLanguageMenu = false;
            languageMenu.SetActive(false);
            ppVolume.enabled = false;
        }

        matterMultiplier = (ulong) PlayerPrefs.GetInt("Multiplier", 1);

        if (matterMultiplier > 1)
            matterText.text = localizedMassString.GetLocalizedString() + "  x" + matterMultiplier;
        else
            matterText.text = localizedMassString.GetLocalizedString();

        isUpdatingToggle = true;

        exitMenu.SetActive(false);
        isExitMenu = false;
        isAchievementsMenu = false;
        isRestartMenu = false;

        showOptionsText = PlayerPrefs.GetInt("showOptionsText", 1) == 1; // 1 es activado por defecto
        optionsText.SetActive(showOptionsText);

        showMatterPerClickText = PlayerPrefs.GetInt("showMatterPerClickText", 1) == 1;
        suffixes = PlayerPrefs.GetInt("suffixes", 1) == 1;

        suffixesToggle.isOn = suffixes;

        hideOptionsToggle.isOn = showOptionsText;
        massPerClickTextToggle.isOn = showMatterPerClickText;

        itemsBought = PlayerPrefs.GetInt("itemsBought", 0);
        matterSpent = ulong.Parse(PlayerPrefs.GetString("matterSpent", "0"));
        numberOfClicks = ulong.Parse(PlayerPrefs.GetString("numberOfClicks", "0"));

        isUpdatingToggle = false;

        if (endGame && !endGameAnimation)
            EndGame();
    }

    private void Update()
    {
        if (massPerSecond < 0)
            massPerSecond = double.MaxValue;

        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Screen.fullScreenMode == FullScreenMode.Windowed)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }

            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(1280, 720, false);
            }
                
        }

        if (endGame && !endGameAnimation)
            EndGame();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isLanguageMenu && !isExitMenu && !isRestartMenu && !isAchievementsMenu && !endGame)
            {
                OpenOptionsMenu();
            }
        }

        UpdateMassText(mass);
        UpdateMassPerSecondText(massPerSecond);

        evolutionSlider.value = (float)mass / celestialStages[currentStage].GetComponent<CelestialObject>().massToEvolve;


        foreach (UnlockShopSection section in sections)
        {
            if (!section.IsUnlocked())
            {
                if (mass >= section.GetPrice())
                    section.UnlockSection();
            }
            
        }

        if (!endGame)
        {
            // Aplicar el multiplicador a la generación pasiva
            massAccumulator += (massPerSecond * matterMultiplier) * Time.deltaTime;

            if (massAccumulator >= 1.0)
            {
                ulong massToAdd = (ulong)massAccumulator;
                if (Mathf.Abs(mass + massToAdd) < ulong.MaxValue)
                {
                    mass += massToAdd;
                    massAccumulator -= massToAdd;
                    if (massAccumulator < 0)
                        massAccumulator = 0;
                }
                else
                    endGame = true;
            }
        }


        // Comprobar evolución
        if (currentStage < celestialStages.Length - 1 && mass >= celestialStages[currentStage].GetComponent<CelestialObject>().massToEvolve)
        {
            ActivateEvolution();
        }
        if (mass >= celestialStages[celestialStages.Length - 1].GetComponent<CelestialObject>().massToEvolve && !endGameAnimation)
        {
            endGame = true;
            endGameAnimation = true;
            EndGame();
        }

        // Desbloquear logros
        if (mass >= 1_000_000)
        {
            achievementManager.UnlockAchievement(10);
        }
        else if (mass >= 1_000_000_000_000)
        {
            achievementManager.UnlockAchievement(13);
        }
    }

    public void UpdateMassText(ulong mass)
    {
        ulong maxUlongThreshold = (ulong)(System.Math.Pow(2, 64) * 0.9);  // 90% del máximo


        if (suffixes)
            massText.text = FormatLargeNumberLocalized(mass, localeId);
        else
            massText.text = mass.ToString("N0");

        string infinityText;

        if (infinityByLanguage.TryGetValue(localeId, out string[] values))
        {
            infinityText = values[0];
        }
        else
        {
            infinityText = "Infinity";
        }

        /*
        if (mass >= maxUlongThreshold)
            massText.text = infinityText;
        */

    }

    public void UpdateMassPerSecondText(double massRate)
    {
        double maxDoubleThreshold = double.MaxValue * 0.9; // 90% del máximo de double

        if (massRate % 1 == 0)
        {
            if (suffixes)
                massPerSecondText.text = localizedPerSecondString.GetLocalizedString() + FormatNumberLocalized(massRate, localeId);
            else
                massPerSecondText.text = localizedPerSecondString.GetLocalizedString() + massRate.ToString("N0");
        }
        else
        {
            if (suffixes)
                massPerSecondText.text = localizedPerSecondString.GetLocalizedString() + FormatNumberLocalized(massRate, localeId);
            else
                massPerSecondText.text = localizedPerSecondString.GetLocalizedString() + massRate.ToString("N1");
        }

        string infinityText;

        if (infinityByLanguage.TryGetValue(localeId, out string[] values))
        {
            infinityText = values[0];
        }
        else
        {
            infinityText = "Infinity";
        }

        /*
        if (massRate >= maxDoubleThreshold)
            massPerSecondText.text = infinityText;
        */
    }

    [SerializeField]
    private Transform textPoolParent;
    private GameObject GetInactiveText()
    {
        foreach (Transform child in textPoolParent)
        {
            if (!child.gameObject.activeSelf)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public void ShowMatterPerClickText()
    {
        if (!isUpdatingToggle)
        {
            showMatterPerClickText = !showMatterPerClickText;
            SaveGame();
        }

    }

    [SerializeField]
    private Canvas canvas;
    public void AddSize(Vector2 clickPosition)
    {
        if (endGame)
            return;

        if (showMatterPerClickText)
        {
            GameObject a = GetInactiveText();
            a.SetActive(true);
            //a.GetComponent<Text>().color = Color.white;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                Input.mousePosition,
                canvas.worldCamera,
                out localPoint
            );

            // Mover el elemento UI a la posición del clic  
            float randomOffsetX = Random.Range(-10f, 10f);
            localPoint.x += randomOffsetX;
            a.GetComponent<RectTransform>().anchoredPosition = localPoint;
            a.GetComponent<Animator>().Play("Clicked");
            //a.AddComponent<Rigidbody2D>();
            //a.GetComponent<Rigidbody2D>().gravityScale = 10;
            if (suffixes)
                a.GetComponent<Text>().text = "+ " + FormatLargeNumberLocalized(massPerClick * matterMultiplier, localeId);
            else
                a.GetComponent<Text>().text = "+ " + (massPerClick * matterMultiplier).ToString("N0");

            string infinityText;

            if (infinityByLanguage.TryGetValue(localeId, out string[] values))
            {
                infinityText = values[0];
            }
            else
            {
                infinityText = "Infinity";
            }
            float maxFloatThreshold = float.MaxValue * 0.9f; // 90% del máximo de float
            if (massPerClick >= maxFloatThreshold)
                a.GetComponent<Text>().text = infinityText;
        }
        numberOfClicks += 1;
        PlayerPrefs.SetString("numberOfClicks", numberOfClicks.ToString());
        if (numberOfClicks >= 10000)
        {
            achievementManager.UnlockAchievement(4);
        }

        if (mass + massPerClick * matterMultiplier < ulong.MaxValue && mass + massPerClick * matterMultiplier > 0)
            mass += (ulong)(massPerClick * matterMultiplier);
        else
            endGame = true;

        // Actualizar la escala visual
        float newScale = Mathf.Max(1f, mass * celestialStages[currentStage].GetComponent<CelestialObject>().scaleFactor);
        //celestialStages[currentStage].transform.localScale = Vector3.one * newScale;
    }


    public void Evolve()
    {
        if (currentStage < celestialStages.Length - 1)
        {
            StartCoroutine(EnableButtonAfterDelay(6f));
            evolutionButton.gameObject.SetActive(false);
            evolutionSlider.gameObject.SetActive(true);

            evolutionParticles.transform.position = celestialStages[currentStage].transform.position;
            evolutionParticles.Play();
            StartCoroutine(StopParticles());
            massPerSecond += celestialStages[currentStage].GetComponent<CelestialObject>().evolvePassiveMass;
            massPerClick += celestialStages[currentStage].GetComponent<CelestialObject>().evolveClickMass;
            celestialStages[currentStage].GetComponent<CelestialObject>().Disappear();
            if (celestialStages[currentStage].GetComponent<StopParticles>() != null)
            {
                celestialStages[currentStage].GetComponent<StopParticles>().StopCreatingParticles();
            }

            if (currentStage == 0)
                achievementManager.UnlockAchievement(0);
            else if (currentStage == 5)
                achievementManager.UnlockAchievement(9);
            else if (currentStage == 10)
                achievementManager.UnlockAchievement(2);
            else if (currentStage == 12)
                achievementManager.UnlockAchievement(11);
            else if (currentStage == 13)
                achievementManager.UnlockAchievement(5);
            else if (currentStage == 14)
            {
                achievementManager.UnlockAchievement(6);
                achievementManager.UnlockAchievement(7);
            }


            currentStage++;
            celestialStages[currentStage].gameObject.SetActive(true);
            celestialStages[currentStage].GetComponent<CelestialObject>().Appear();



        }
    }

    IEnumerator EnableButtonAfterDelay(float delay)
    {
        evolutionButton.interactable = false;
        yield return new WaitForSeconds(delay);
        evolutionButton.interactable = true;
    }
    private void ActivateEvolution()
    {
        evolutionSlider.gameObject.SetActive(false);
        evolutionButton.gameObject.SetActive(true);
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(3f);
        evolutionParticles.Stop();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetString("Mass", mass.ToString());
        PlayerPrefs.SetFloat("MassPerClick", massPerClick);
        PlayerPrefs.SetString("MassPerSecond", massPerSecond.ToString());
        PlayerPrefs.SetInt("CurrentStage", currentStage);

        // Corregido el guardado de showOptionsText (activado=1, desactivado=0)
        PlayerPrefs.SetInt("showOptionsText", showOptionsText ? 1 : 0);
        PlayerPrefs.SetInt("suffixes", suffixes ? 1 : 0);
        PlayerPrefs.SetInt("showMatterPerClickText", showMatterPerClickText ? 1 : 0);

        foreach (ShopButton shopItem in shopButtons)
        {
            PlayerPrefs.SetInt(shopItem.itemName, shopItem.quantity);
        }

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        mass = ulong.Parse(PlayerPrefs.GetString("Mass", "0"));
        currentStage = PlayerPrefs.GetInt("CurrentStage", 0);
        massPerClick = (ulong) PlayerPrefs.GetFloat("MassPerClick", 1f);
        massPerSecond = double.Parse(PlayerPrefs.GetString("MassPerSecond", "0"));
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        SaveTimePlayed();
    }


    #region Shop

    public void StartShopCooldown(ShopButton shopButton, float seconds)
    {
        StartCoroutine(ShopCooldown(shopButton, seconds));
    }

    private IEnumerator ShopCooldown(ShopButton shopButton, float seconds)
    {
        shopButton.GetComponent<Button>().interactable = false;
        if (shopButton.cooldownImage != null)
            shopButton.cooldownImage.fillAmount = 1;

        float maxSeconds = seconds;
        while (seconds > 0)
        {
            if (shopButton.cooldownImage != null)
                shopButton.cooldownImage.fillAmount = seconds / maxSeconds;
            seconds -= Time.deltaTime;
            yield return null;
        }

        if (shopButton.cooldownImage != null)
            shopButton.cooldownImage.fillAmount = 0;
        shopButton.GetComponent<Button>().interactable = true;
    }

    public void ApplyGlobalMultiplier(float multiplier, bool isTemporary = false, float duration = 0)
    {
        if (massPerSecond * multiplier > double.MaxValue)
            massPerSecond = double.MaxValue;
        else
            massPerSecond *= multiplier;

        if (massPerClick * multiplier > ulong.MaxValue)
            massPerClick = ulong.MaxValue;
        else
            massPerClick = (ulong) (massPerClick * multiplier);

        if (isTemporary)
        {
            StartCoroutine(RemoveMultiplierAfterDuration(multiplier, duration));
        }
    }

    private IEnumerator RemoveMultiplierAfterDuration(float multiplier, float duration)
    {
        yield return new WaitForSeconds(duration);
        massPerSecond /= multiplier;
        massPerClick = (ulong)(massPerClick / multiplier);
    }

    public void ReduceMassToEvolve(float amount)
    {
        celestialStages[currentStage].GetComponent<CelestialObject>().massToEvolve -= (ulong)(celestialStages[currentStage].GetComponent<CelestialObject>().massToEvolve * amount);
    }

    public void AddShopItem(ShopButton shopItem)
    {
        shopButtons.Add(shopItem);
    }

    public void ReduceItemsPrice(float reducedAmount)
    {
        foreach (ShopButton shopItem in shopButtons)
        {
            shopItem.price -= (ulong)(shopItem.price * reducedAmount);
            shopItem.basePrice -= (ulong)(shopItem.basePrice * reducedAmount);
            shopItem.UpdateUI();
        }
    }

    public void IncreaseClickMassPerUpgrade(float bonus)
    {
        int con = 0;
        foreach (ShopButton shopItem in shopButtons)
        {
            con += (int)shopItem.quantity;
        }
        if (massPerClick + (ulong)(massPerClick * (con * bonus)) > ulong.MaxValue)
            massPerClick = ulong.MaxValue;
        else
            massPerClick += (ulong)(massPerClick * (con * bonus));
    }
    public void IncreasePassiveMassPerUpgrade(float bonus)
    {
        int con = 0;
        foreach (ShopButton shopItem in shopButtons)
        {
            con += (int)shopItem.quantity;
        }
        if (massPerSecond + massPerSecond * (con * bonus) > double.MaxValue)
            massPerSecond = double.MaxValue;
        else
            massPerSecond += massPerSecond * (con * bonus);
    }
    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CloseExitMenu()
    {
        ppVolume.enabled = false;
        exitMenu.SetActive(false);
        isExitMenu = false;
    }

    public void CloseLanguageMenu()
    {
        ppVolume.enabled = false;
        languageMenu.SetActive(false);
        isLanguageMenu = false;
    }

    public void CloseRestartMenu()
    {
        ppVolume.enabled = false;
        restartMenu.SetActive(false);
        isRestartMenu = false;
    }
    public void CloseAchievementMenu()
    {
        ppVolume.enabled = false;
        achievementsMenu.SetActive(false);
        isAchievementsMenu = false;
    }
    public void OpenLanguageMenu()
    {
        CloseOptionsMenu();
        ppVolume.enabled = true;
        languageMenu.SetActive(true);
        isLanguageMenu = true;
    }

    public void OpenExitMenu()
    {
        CloseOptionsMenu();
        ppVolume.enabled = true;
        exitMenu.SetActive(true);
        isExitMenu = true;
    }
    public void OpenRestartMenu()
    {
        CloseOptionsMenu();
        ppVolume.enabled = true;
        restartMenu.SetActive(true);
        isRestartMenu = true;
    }

    public void OpenAchievementsMenu()
    {
        CloseOptionsMenu();
        ppVolume.enabled = true;
        achievementsMenu.SetActive(true);
        isAchievementsMenu = true;
    }

    private bool active = false;

    public void ChangeLocale(int localeID)
    {
        if (active) return;

        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        active = false;
        if (matterMultiplier > 1)
            matterText.text = localizedMassString.GetLocalizedString() + "  x" + matterMultiplier;
        else
            matterText.text = localizedMassString.GetLocalizedString();
        localeId = localeID;
        achievementManager.UpdateHUD();
        CloseLanguageMenu();
    }

    IEnumerator SetLocaleHover(int localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt("LocaleKey", localeID);
        PlayerPrefs.SetInt("LanguageMenu", 1);
        if (matterMultiplier > 1)
            matterText.text = localizedMassString.GetLocalizedString() + "  x" + matterMultiplier;
        else
            matterText.text = localizedMassString.GetLocalizedString();
        active = false;
    }

    public void ChangeLocaleHover(int localeID)
    {
        if (active) return;
        StartCoroutine(SetLocaleHover(localeID));
    }

    private string FormatNumberLocalized(double value, int languageCode)
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

        // Formatea para que siempre haya 3 dígitos significativos
        if (displayValue >= 100)
        {
            // Sin decimales para valores mayores o iguales a 100
            return displayValue.ToString("0") + " " + suffixes[suffixIndex]; // Ej: 123K
        }
        else if (displayValue >= 10)
        {
            // Un decimal, pero eliminando el .0 si no es necesario
            return displayValue % 1 == 0
                ? displayValue.ToString("0") + " " + suffixes[suffixIndex] // Ej: 12K
                : displayValue.ToString("0.0") + " " + suffixes[suffixIndex]; // Ej: 12.3K
        }
        else
        {
            // Dos decimales, pero eliminando el .00 si no es necesario
            return displayValue % 1 == 0
                ? displayValue.ToString("0") + " " + suffixes[suffixIndex] // Ej: 1K
                : displayValue.ToString("0.##") + " " + suffixes[suffixIndex]; // Ej: 1.23K
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

        // Determinar si forzar el ".0"
        bool forceDecimalZero = massPerSecond >= 100;

        // Aplicar formato según el valor y la condición
        if (displayValue >= 100)
        {
            return displayValue.ToString("0") + " " + suffixes[suffixIndex]; // Ej: 123K
        }
        else if (displayValue >= 10)
        {
            return forceDecimalZero
                ? displayValue.ToString("0.0") + " " + suffixes[suffixIndex] // Ej: 12.0K
                : displayValue.ToString("0.#") + " " + suffixes[suffixIndex]; // Ej: 12K o 12.3K
        }
        else
        {
            return forceDecimalZero
                ? displayValue.ToString("0.0") + " " + suffixes[suffixIndex] // Ej: 1.0K
                : displayValue.ToString("0.##") + " " + suffixes[suffixIndex]; // Ej: 1K o 1.23K
        }
    }
    public void RestartGame(bool multiplier)
    {
        string[] keysToKeepInt = { "LanguageMenu", "LocaleKey", "suffixes", "showMatterPerClickText", "showOptionsText"};
        string[] keysToKeepFloat = { "MusicVolume", "Menu_Red", "Menu_Green", "Menu_Blue"};

        if (multiplier)
        {
            keysToKeepInt = new string[] { "LanguageMenu", "LocaleKey", "suffixes", "showMatterPerClickText", "showOptionsText", "Multiplier", "Flash" };
            keysToKeepFloat = new string[] { "MusicVolume", "Menu_Red", "Menu_Green", "Menu_Blue", "TotalTimePlayed" };
        }

        Dictionary<string, float> savedValuesFloat = new Dictionary<string, float>();
        Dictionary<string, int> savedValuesInt = new Dictionary<string, int>();

        foreach (string key in keysToKeepFloat)
        {
            if (PlayerPrefs.HasKey(key))
                savedValuesFloat[key] = PlayerPrefs.GetFloat(key);
        }

        foreach (string key in keysToKeepInt)
        {
            if (PlayerPrefs.HasKey(key))
                savedValuesInt[key] = PlayerPrefs.GetInt(key);
        }

        // Borrar todas las configuraciones
        PlayerPrefs.DeleteAll();

        foreach (var pair in savedValuesFloat)
        {
            PlayerPrefs.SetFloat(pair.Key, pair.Value);
        }

        foreach (var pair in savedValuesInt)
        {
            PlayerPrefs.SetInt(pair.Key, pair.Value);
        }

        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [SerializeField]
    private GameObject shop;

    public void BigBang()
    {
        shop.SetActive(false);
        matterMultiplier++;
        PlayerPrefs.SetInt("Multiplier", (int) matterMultiplier);
        PlayerPrefs.SetInt("Flash", 1);
        PlayerPrefs.SetString("Mass", "0");
        PlayerPrefs.SetFloat("MassPerClick", 1f);
        PlayerPrefs.SetString("MassPerSecond", "0");

        StartCoroutine(ShrinkAndExplode());
    }

    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float minScale = 0.01f;
    [SerializeField] private Image flashImage;

    private Vector3 initialScale;

    IEnumerator ShrinkAndExplode()
    {
        initialScale = celestialStages[currentStage].transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            float progress = elapsedTime / 2f;
            celestialStages[currentStage].transform.localScale = Vector3.Lerp(initialScale, Vector3.one * minScale, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        celestialStages[currentStage].transform.localScale = Vector3.one * minScale;

        explosionEffect.transform.position = celestialStages[currentStage].transform.position;
        explosionEffect.SetActive(true);

        //yield return StartCoroutine(FlashScreen());

        yield return new WaitForSeconds(4f);
        //yield return StartCoroutine(FlashScreen());
        
        RestartGame(true);
    }

    IEnumerator FlashScreen()
    {
        Color flashColor = new Color(1, 1, 1, 1);
        flashImage.color = flashColor;
        flashImage.gameObject.SetActive(true);

        // Hacer que el flash desaparezca en 2 segundos
        float fadeTime = 2f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            flashImage.color = new Color(1, 1, 1, 1 - (elapsed / fadeTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        ppVolume.enabled = true;
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        ppVolume.enabled = false;
    }

    public void EnableSuffixes()
    {
        if (!isUpdatingToggle)
        {
            suffixes = !suffixes;

            foreach (ShopButton shopItem in shopButtons)
            {
                shopItem.UpdateUI();
            }
            SaveGame();
        }

    }

    public void EnableOptionsText()
    {
        if (!isUpdatingToggle)
        {
            showOptionsText = !showOptionsText;
            optionsText.SetActive(showOptionsText);
            SaveGame();
        }
    }

    public void StoreAchievement(ulong price, string type, string name)
    {
        bool allAchievements = false;
        if (type == "Phenomena")
            achievementManager.UnlockAchievement(8);

        itemsBought++;
        matterSpent += price;
        PlayerPrefs.SetInt("itemsBought", itemsBought);
        PlayerPrefs.SetString("matterSpent", matterSpent.ToString());

        if (shopButtons.Count >= 16)
        {
            foreach (var shopButton in shopButtons)
            {
                if (shopButton.quantity == 0 && shopButton.itemName != "Big Bang")
                {
                    allAchievements = false;
                    return;
                }
                if (matterMultiplier > 1)
                    allAchievements = true;
            }
        }

        if (allAchievements)
        {
            achievementManager.UnlockAchievement(14);
        }

        if (matterSpent >= 1000000)
        {
            achievementManager.UnlockAchievement(3);
        }

        if (matterSpent >= 1000000000000)
        {
            achievementManager.UnlockAchievement(12);
        }

        if (itemsBought == 10)
        {
            achievementManager.UnlockAchievement(1);
        }
    }

    public void AddUnlockableSections(UnlockShopSection section)
    {
        sections.Add(section);
    }


    [SerializeField]
    private GameObject endBackground;

    [SerializeField]
    private Image statisticsPanel;

    [SerializeField]
    private Text statsText;
    private void EndGame()
    {
        endGameAnimation = true;
        PlayerPrefs.SetInt("endGame", 1);
        endBackground.SetActive(true);
        endBackground.GetComponent<DissolvingController>().Create();
        celestialStages[currentStage].GetComponent<CelestialObject>().Disappear();
        shop.SetActive(false);
        canvas.gameObject.SetActive(false);

        StartCoroutine(FadeIn());

        string[] stats = statsByLanguage[localeId];
        if (suffixes)
            statsText.text = statsText.text = $"{stats[0]}{GetFormattedTime()}\n" +
                 $"{stats[1]}{FormatLargeNumberLocalized(mass, localeId):N0}\n" +
                 $"{stats[2]}{FormatNumberLocalized(massPerSecond, localeId):N1}{stats[3]}\n" +
                 $"{stats[4]}{FormatNumberLocalized(massPerClick, localeId):N0}\n" +
                 $"{stats[5]}{numberOfClicks}\n" +
                 $"{stats[6]}{currentStage + 1}\n" +
                 $"{stats[7]}{itemsBought}\n" +
                 $"{stats[8]}{FormatLargeNumberLocalized(matterSpent,localeId):N0}\n";
        else
            statsText.text = $"{stats[0]}{GetFormattedTime()}\n" +
                 $"{stats[1]}{mass:N0}\n" +
                 $"{stats[2]}{massPerSecond:N1}{stats[3]}\n" +
                 $"{stats[4]}{massPerClick:N0}\n" +
                 $"{stats[5]}{numberOfClicks}\n" +
                 $"{stats[6]}{currentStage + 1}\n" +
                 $"{stats[7]}{itemsBought}\n" +
                 $"{stats[8]}{matterSpent:N0}\n";

    }
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 2f;
    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(4f);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // Asegurar que termina en 1
    }

    private void SaveTimePlayed()
    {
        // Calcular tiempo jugado en esta sesión
        double sessionTime = (DateTime.Now - sessionStartTime).TotalSeconds;

        // Sumar al tiempo total y guardar
        totalTimePlayed += sessionTime;
        PlayerPrefs.SetFloat("TotalTimePlayed", (float)totalTimePlayed);
        PlayerPrefs.Save();
    }

    private string GetFormattedTime()
    {
        double sessionTime = (DateTime.Now - sessionStartTime).TotalSeconds;

        TimeSpan timeSpan = TimeSpan.FromSeconds(totalTimePlayed+sessionTime);
        return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m";
    }
}
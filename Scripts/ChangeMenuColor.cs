using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMenuColor : MonoBehaviour
{
    [SerializeField]
    private List<Image> backgrounds = new List<Image>();  // Lista de backgrounds

    [SerializeField]
    private List<Image> buttons = new List<Image>();  // Lista de botones

    [SerializeField]
    private List<Text> texts = new List<Text>();  // Lista de textos

    [SerializeField]
    private Slider redSlider, greenSlider, blueSlider;  // Sliders para cambiar el color
    [SerializeField]
    private Image previewImage;  // Vista previa del color

    private Color currentColor;  // Color actual

    void Start()
    {
        LoadColor();  // Cargar el color guardado o por defecto

        // Asignar listeners a los sliders
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);
    }

    // Método para cambiar el color
    public void ChangeColor(Color color)
    {
        currentColor = color;

        // Cambiar el color de los backgrounds
        SetBackgroundColor(color);

        // Cambiar el color de los textos
        SetTextColor(color);

        // Guardar el color actual
        SaveColor();
    }

    // Cambiar el color de los backgrounds
    private void SetBackgroundColor(Color color)
    {
        // Cambiar el color de los fondos
        foreach (Image background in backgrounds)
        {
            if (background != null)  // Verificar que no sea null
            {
                background.color = color;  // Cambiar solo el color
            }
        }

        // Cambiar el color de los botones
        foreach (Image button in buttons)
        {
            if (button != null && button.material != null)  // Verificar que no sea null y tenga un material
            {
                button.material.SetColor("_Color", color);  // Cambiar el color del material
            }
        }
    }

    // Cambiar el color de los textos
    private void SetTextColor(Color color)
    {
        foreach (Text text in texts)
        {
            if (text != null)  // Verificar que no sea null
            {
                text.color = color;  // Cambiar solo el color del texto
            }
        }
    }

    // Actualizar el color en base a los valores de los sliders
    private void UpdateColor(float value)
    {
        Color newColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);

        // Actualizar la vista previa
        if (previewImage != null)
            previewImage.color = newColor;

        // Cambiar el color de los elementos
        ChangeColor(newColor);
    }

    // Guardar el color actual
    private void SaveColor()
    {
        PlayerPrefs.SetFloat("Menu_Red", currentColor.r);
        PlayerPrefs.SetFloat("Menu_Green", currentColor.g);
        PlayerPrefs.SetFloat("Menu_Blue", currentColor.b);
        PlayerPrefs.Save();
    }

    // Cargar el color guardado o por defecto
    private void LoadColor()
    {
        float r = PlayerPrefs.GetFloat("Menu_Red", 0f);
        float g = PlayerPrefs.GetFloat("Menu_Green", 140f/255f);
        float b = PlayerPrefs.GetFloat("Menu_Blue", 1f);

        currentColor = new Color(r, g, b);

        // Cambiar el color de los elementos al cargar
        ChangeColor(currentColor);

        // Actualizar los sliders
        redSlider.value = r;
        greenSlider.value = g;
        blueSlider.value = b;

        // Actualizar la vista previa
        if (previewImage != null)
            previewImage.color = currentColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverLocalization : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private int localeID;
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.ChangeLocaleHover(localeID);
    }
}

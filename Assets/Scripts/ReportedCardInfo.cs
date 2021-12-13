using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReportedCardInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseOver;
    public ReportedPokemonCard controllerReference;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        controllerReference.ReportPointerMove(mouseOver);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        controllerReference.ReportPointerMove(mouseOver);
    }
}

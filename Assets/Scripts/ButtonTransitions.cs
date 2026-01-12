using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTransitions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public Color32 m_NormalColor;
//    public Color32 m_HoverColor;
    public Color32 m_DownColor;

    private List<string> a = new List<string>();

    private Image m_Image = null;
    
    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("Enter");
        //m_Image.color = m_HoverColor;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
       // print("Exit");
        m_Image.color = m_NormalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //print("Down");
        m_Image.color = m_DownColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //print("Click");
        m_Image.color = m_NormalColor;
    }

}

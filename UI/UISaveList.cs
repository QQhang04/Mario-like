using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISaveList : MonoBehaviour
{
    public bool focusFirstElement = true;
    public UISaveCard card;
    public RectTransform container;
    protected List<UISaveCard> m_cardList = new List<UISaveCard>();
    protected void Awake()
    {
        var data = GameSaver.Instance.loadList();
        

        for (int i = 0; i < data.Length; i++)
        {
            m_cardList.Add(Instantiate(card, container));
            m_cardList[i].Fill(i, data[i]);
        }

        if (focusFirstElement)
        {
            if (m_cardList[0].isFilled)
            {
                EventSystem.current.SetSelectedGameObject(m_cardList[0].loadButton.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(m_cardList[0].newGameButton.gameObject);
            }
        }
    }
}
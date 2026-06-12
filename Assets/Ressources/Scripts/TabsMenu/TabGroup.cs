using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public List<TabContent> tabsContent;
    public TabButton currentButton;

    private void Start()
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();
        if(tabsContent == null)
            tabsContent = new List<TabContent>();
    }

    public void addTab(TabButton button)
    {
        tabButtons.Add(button);
    }
    public void addContent(TabContent content)
    {
        tabsContent.Add(content);
    }

    public void Onclick(TabButton button)
    {
        currentButton = button;
        int index = button.transform.GetSiblingIndex();
        for(int i = 0; i<tabsContent.Count; i++)
        {
            if(i == index)
            {
                tabsContent[i].gameObject.SetActive(true);
            }
            else
            {
                tabsContent[i].gameObject.SetActive(false);
            }
        }
    }
    public void onEnter() {
        gameObject.SetActive(true);
        currentButton = tabButtons[0];
        int index = 0;
        for (int i = 0; i < tabsContent.Count; i++) {
            if (i == index) {
                tabsContent[i].gameObject.SetActive(true);
            }
            else {
                tabsContent[i].gameObject.SetActive(false);
            }
        }
    }
}

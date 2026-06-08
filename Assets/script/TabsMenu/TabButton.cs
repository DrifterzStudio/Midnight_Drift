using UnityEngine;

public class TabButton : MonoBehaviour
{
    public TabGroup tabGroup;
    
    void Start()
    {
        tabGroup.addTab(this);
    }
    public void Onclick()
    {
        tabGroup.Onclick(this);
    }

}

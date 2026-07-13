using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextSliderScript : MonoBehaviour
{
    public TextSliderManager manager;
    private TMP_Text text;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
        manager.addText(this);
    }
    public TMP_Text getText()
    {
        return text;
    }

}

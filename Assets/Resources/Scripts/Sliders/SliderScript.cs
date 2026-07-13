using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderScript : MonoBehaviour
{
    public TextSliderManager Manager;
    private Slider slider;
    private float previousValue;
    private void Start()
    {
        slider = GetComponent<Slider>();
        previousValue = slider.minValue;
        Manager.addSlider(this);
    }

    public Slider getSlider()
    {
        return slider;
    }
    public bool OnvalueChange()
    {
        if (slider.value == previousValue)
            return false;
        else
        {
            previousValue = slider.value;
            return true;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextSliderManager : MonoBehaviour
{
    [SerializeField] private List<TextSliderScript> textSliders;
    [SerializeField] private List<SliderScript> sliders;


    public void addText(TextSliderScript newTextSlider)
    {
        textSliders.Add(newTextSlider);
    }
    public void addSlider(SliderScript newSlider)
    {
        sliders.Add(newSlider);
    }

    private void Update()
    {
        foreach(SliderScript slider in sliders)
        {
            if(slider.OnvalueChange())
            {
                textSliders[slider.transform.GetSiblingIndex()].getText().text = slider.getSlider().value.ToString("F0");
            }
        }
    }
}

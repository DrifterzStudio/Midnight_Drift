using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextSliderManager : MonoBehaviour
{
    private List<TextSliderScript> textSliders;
    private List<SliderScript> sliders;

    private void Start()
    {
        if(textSliders == null)
            textSliders = new List<TextSliderScript>();
        if(sliders == null)
            sliders = new List<SliderScript>();
    }

    public void addText(TextSliderScript newTextSlider)
    {
        textSliders.Add(newTextSlider);
    }
    public void addSlider(SliderScript newSlider)
    {
        sliders.Add(newSlider);
        textSliders[sliders.Count - 1].getText().text = sliders.Last().getSlider().value.ToString("F1");
    }

    private void Update()
    {
        foreach(SliderScript slider in sliders)
        {
            if(slider.OnvalueChange())
            {
                textSliders[slider.transform.GetSiblingIndex()].getText().text = slider.getSlider().value.ToString("F1");
            }
        }
    }
}

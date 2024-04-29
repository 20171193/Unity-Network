using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSlider : BuilboardUI
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Slider backGroundSlider;

    [SerializeField]
    private float decreaseTime;

    private Coroutine decreaseRoutine;

    public void UpdateSliderValue(float prevValue, float targetValue)
    {
        decreaseRoutine = StartCoroutine(DecreaseRoutine(prevValue, targetValue));
    }

    IEnumerator DecreaseRoutine(float prevValue, float targetValue)
    {
        float rate = 0f;
        slider.value = targetValue;
        yield return null;

        while(rate < 1f)
        {
            rate += Time.deltaTime / decreaseTime;
            backGroundSlider.value = Mathf.Lerp(prevValue, targetValue, rate);
            yield return null;
        }

        backGroundSlider.value = targetValue;
        yield return null;
    }
}

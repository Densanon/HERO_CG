using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIResponseTimer : MonoBehaviour
{
    [SerializeField]
    float timerTime = 6f;
    float timer = 0f;
    public Slider mySlider;
    public TMP_Text mySliderText;

    public static Action OnTimerRunOut = delegate { };

    private void Start()
    {
        Referee.OnWaitTimer += OnStartTimer;
    }

    private void OnDestroy()
    {
        Referee.OnWaitTimer -= OnStartTimer;
    }

    void FixedUpdate()
    {
        if(timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            mySlider.value = timer;
            mySliderText.text = Mathf.Round(timer).ToString();
        }
        else
        {
            OnTimerRunOut?.Invoke();
            mySlider.gameObject.SetActive(false);
        }
    }

    private void OnStartTimer()
    {
        timer = timerTime;
        mySlider.gameObject.SetActive(true);
        mySlider.maxValue = timerTime;
        mySlider.value = timerTime;
        mySliderText.text = timerTime.ToString();
    }
}

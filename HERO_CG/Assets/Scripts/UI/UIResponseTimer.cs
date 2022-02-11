using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIResponseTimer : MonoBehaviour
{
    [SerializeField]
    float timerTime = 10f;
    float timer = 0f;
    public Slider mySlider;
    public TMP_Text mySliderText;
    public GameObject gContainer;
    bool Responding = true;

    public static Action OnTimerRunOut = delegate { };

    private void Start()
    {
        Referee.OnWaitTimer += OnStartTimer;
        Referee.OnPlayerResponded += OnStopTimer;
    }

    private void OnDestroy()
    {
        Referee.OnWaitTimer -= OnStartTimer;
        Referee.OnPlayerResponded -= OnStopTimer;
    }

    private void OnStopTimer()
    {
        timer = 0;
        gContainer.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            mySlider.value = timer;
            mySliderText.text = Mathf.Round(timer).ToString();
        }
        else if (timer < 0)
        {
            if (Responding)
            {
                OnTimerRunOut?.Invoke();
                OnStopTimer();
            }
        }
    }

    private void OnStartTimer(bool responding)
    {
        Responding = responding;
        timer = timerTime;
        gContainer.gameObject.SetActive(true);
        mySlider.maxValue = timerTime;
        mySlider.value = timerTime;
        mySliderText.text = timerTime.ToString();
    }
}

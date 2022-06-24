using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Gann4Games.Thirdym.Utility;

public class NotificationHandler : MonoBehaviour
{
    public static NotificationHandler instance;

    [Header("Visuals")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI label;

    [Header("Sound")]
    [SerializeField] AudioClip notificationSfx;
    [SerializeField] AudioSource soundSource;
    [Space]
    [SerializeField] UnityEvent onNotify;

    float _fadeAmount = 1;

    TimerTool _timer;

    private void Awake() => instance = this;
    private void Update() => NotificationUpdate();
    void NotificationUpdate()
    {
        if(_timer == null) return;
        if (_timer.HasFinished()) // If the time is out, set the canvas alpha to zero.
        {
            if (canvasGroup.alpha != 0) canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * _fadeAmount);
        }
        else // Otherwise, count the time and display the hint.
        {
            _timer.Count();
            if (canvasGroup.alpha != 1) canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.deltaTime * _fadeAmount);
        }
    }
    public void NotifySFX() => soundSource.PlayOneShot(notificationSfx);
    public void NotifyFade(float value) => _fadeAmount = value;
    public void NotifyText(string text) => label.text = text;
    public void NotifyShow(float duration = 5)
    {
        _timer = new TimerTool(duration);
        _timer.Reset();
        onNotify.Invoke();
    }
    
    public static void Notify(string text, float duration=3, float fadeTime=1, bool playsfx = true)
    {
        instance.NotifyFade(fadeTime);
        instance.NotifyText(text);
        instance.NotifyShow(duration);
        if (playsfx) instance.NotifySFX();
    }
}

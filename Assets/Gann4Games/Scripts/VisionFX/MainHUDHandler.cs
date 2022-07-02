using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainHUDHandler : MonoBehaviour {
    public static MainHUDHandler instance;

    public Slider healthbar, energybar;
    [Space]
    [SerializeField] CanvasGroup damageEffectGroup;
    [SerializeField] Image damageEffectImage;
    public RectTransform crosshair;
    [HideInInspector] public Image crosshairImage;

    private TextMeshProUGUI _healthbarText;
    private TextMeshProUGUI _energybarText;
    //private float _fadeAmount = 1;
    private Color _mainColor = Color.black;

    private Tween _fadeTween;
    private Tween _colorTween;

    public static void SetHealthValue(float value, float time = 1)
    {
        instance.healthbar.DOValue(value, time, true);
    } 

    private void Awake()
    {
        instance = this;
        _healthbarText = healthbar.GetComponentInChildren<TextMeshProUGUI>();
        _energybarText = energybar.GetComponentInChildren<TextMeshProUGUI>();
        crosshairImage = crosshair.GetComponent<Image>();
    }

    public void ShowDamageEffect(Color color, float fadeTime = 1)
    {
        SetDamageEffectColor(color, 1, 0, fadeTime);

        // if(_fadeTween != null) _fadeTween.Kill();
        // if(_colorTween != null) _colorTween.Kill();

        // damageEffectGroup.alpha = 1;
        // damageEffectImage.color = color;

        // _fadeTween = damageEffectGroup.DOFade(0, fadeTime);
        // _colorTween = damageEffectImage.DOColor(Color.black, fadeTime);
        // _fadeTween.Play();
        // _colorTween.Play();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="opacity"></param>
    /// <param name="fadeInTime"></param>
    /// <param name="fadeOutTime">-1 for no fading out.</param>
    public static void SetDamageEffectColor(Color color, float opacity = 1, float fadeInTime = 5, float fadeOutTime = -1)
    {
        instance._fadeTween?.Kill();
        instance._colorTween?.Kill();

        instance._fadeTween = instance.damageEffectGroup.DOFade(opacity, fadeInTime);
        instance._colorTween = instance.damageEffectImage.DOColor(color, fadeInTime).OnComplete(() => {
            if(fadeOutTime >= 0) 
            {
                instance._fadeTween = instance.damageEffectGroup.DOFade(0, fadeOutTime);
                instance._colorTween = instance.damageEffectImage.DOColor(Color.black, fadeOutTime);
            }
        });

        instance._fadeTween.Play();
        instance._colorTween.Play();
    }

    private void Update()
    {
        // if (damageEffectGroup.alpha != mainAlpha) damageEffectGroup.alpha = Mathf.Lerp(damageEffectGroup.alpha, mainAlpha, Time.deltaTime * _fadeAmount);
        // if (damageEffectImage.color != _mainColor) damageEffectImage.color = Color.Lerp(damageEffectImage.color, _mainColor, Time.deltaTime * _fadeAmount);

        HealthbarUpdateText();
        EnergybarUpdateText();
    }

    void HealthbarUpdateText() => _healthbarText.text = string.Format("{0} HP", healthbar.value.ToString("F0"));
    void EnergybarUpdateText() => _energybarText.text = string.Format("{0}% Energy", (energybar.value).ToString("F0")); 
}
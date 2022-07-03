using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gann4Games.Thirdym.Enums;
using DG.Tweening;

public class PlayerInventoryHUD : MonoBehaviour {
    static PlayerInventoryHUD instance;

    [Header("Background colors")]
    [SerializeField] private Color colorBackgroundEmpty;
    [SerializeField] private Color colorBackgroundHave;
    [SerializeField] private Color colorBackgroundEquipped;
    [Header("Text colors")]
    [SerializeField] private Color colorTextEmpty;
    [SerializeField] private Color colorTextHave;
    [SerializeField] private Color colorTextEquipped;


    Image[] bgs;
    TextMeshProUGUI[] txts;

    Image _imagePistol, _imageAutomatic, _imageShotgun, _imageEnergy, _imageExplosive, _imageDefibrilator, _imageBlades;
    Color _pistolBackgroundColor, _rifleBackgroundColor, _shotgunBackgroundColor, _heavyBackgroundColor, _colorBackgroundExplosive, 
        _colorBackgroundDefibrilator, _meleeBackgroundColor;

    TextMeshProUGUI _textPistol, _textAutomatic, _textShotgun, _textEnergy, _textExplosive, _textDefibrilator, _textBlades;
    Color _pistolTextColor, _rifleTextColor, _shotgunTextColor, _heavyTextColor, _colorTextExplosive, _colorTextDefibrilator, _meleeTextColor;

    private void Awake()
    {
        instance = this;
        bgs = GetComponentsInChildren<Image>();
        txts = GetComponentsInChildren<TextMeshProUGUI>();

        _imagePistol = bgs[0];
        _imageAutomatic = bgs[1];
        _imageShotgun = bgs[2];
        _imageEnergy = bgs[3];
        _imageExplosive = bgs[4];
        _imageDefibrilator = bgs[5];
        _imageBlades = bgs[6];

        _textPistol = txts[0];
        _textAutomatic = txts[1];
        _textShotgun = txts[2];
        _textEnergy = txts[3];
        _textExplosive = txts[4];
        _textDefibrilator = txts[5];
        _textBlades = txts[6];

        Init();
    }

    private void Init()
    {
        _imagePistol.DOColor(colorBackgroundEmpty, 0);
        _imageAutomatic.DOColor(colorBackgroundEmpty, 0);
        _imageShotgun.DOColor(colorBackgroundEmpty, 0);
        _imageEnergy.DOColor(colorBackgroundEmpty, 0);
        _imageExplosive.DOColor(colorBackgroundEmpty, 0);
        _imageDefibrilator.DOColor(colorBackgroundEmpty, 0);
        _imageBlades.DOColor(colorBackgroundEmpty, 0);

        _textPistol.DOColor(colorTextEmpty, 0);
        _textAutomatic.DOColor(colorTextEmpty, 0);
        _textShotgun.DOColor(colorTextEmpty, 0);
        _textEnergy.DOColor(colorTextEmpty, 0);
        _textExplosive.DOColor(colorTextEmpty, 0);
        _textDefibrilator.DOColor(colorTextEmpty, 0);
        _textBlades.DOColor(colorTextEmpty, 0);
    }
    
    public static void DisplayWeaponAs(WeaponType weaponType, EquipmentSystem.EquipMode mode)
    {
        Color BackgroundColor() 
        {
            Color color = new Color();
            switch(mode)
            {
                case EquipmentSystem.EquipMode.None:
                    color = instance.colorBackgroundEmpty;
                    break;
                case EquipmentSystem.EquipMode.Stored:
                    color = instance.colorBackgroundHave;
                    break;
                case EquipmentSystem.EquipMode.Equipped:
                    color = instance.colorBackgroundEquipped;
                    break;
            }
            return color;
        }

        Color FontColor() 
        {
            Color color = new Color();
            switch (mode)
            {
                case EquipmentSystem.EquipMode.None:
                    color = instance.colorTextEmpty;
                    break;
                case EquipmentSystem.EquipMode.Stored:
                    color = instance.colorTextHave;
                    break;
                case EquipmentSystem.EquipMode.Equipped:
                    color = instance.colorTextEquipped;
                    break;
            }
            return color;
        }

        float transitionTime = 0.25f;

        switch (weaponType)
        {
            case WeaponType.Pistol:
                instance._imagePistol.DOColor(BackgroundColor(), transitionTime);
                instance._textPistol.DOColor(FontColor(), transitionTime);
                break;

            case WeaponType.Rifle:
                instance._imageAutomatic.DOColor(BackgroundColor(), transitionTime);
                instance._textAutomatic.DOColor(FontColor(), transitionTime);
                break;

            case WeaponType.Shotgun:
                instance._imageShotgun.DOColor(BackgroundColor(), transitionTime);
                instance._textShotgun.DOColor(FontColor(), transitionTime);
                break;

            case WeaponType.Heavy:
                instance._imageEnergy.DOColor(BackgroundColor(), transitionTime);
                instance._textEnergy.DOColor(FontColor(), transitionTime);
                break;

            case WeaponType.Tool:
                instance._imageDefibrilator.DOColor(BackgroundColor(), transitionTime);
                instance._textDefibrilator.DOColor(FontColor(), transitionTime);
                break;

            case WeaponType.Melee:
                instance._imageBlades.DOColor(BackgroundColor(), transitionTime);
                instance._textBlades.DOColor(FontColor(), transitionTime);
                break;
        }
    }
}

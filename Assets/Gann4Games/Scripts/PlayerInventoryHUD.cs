using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gann4Games.Thirdym.Enums;
using Gann4Games.Thirdym.ScriptableObjects;

public class PlayerInventoryHUD : MonoBehaviour {
    static PlayerInventoryHUD instance;

    [Header("Background colors")]
    public Color colorBackgroundEmpty;
    public Color colorBackgroundHave;
    public Color colorBackgroundEquipped;
    [Header("Text colors")]
    public Color colorTextEmpty;
    public Color colorTextHave;
    public Color colorTextEquipped;


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
    }
    private void Update()
    {
        float lerp = Time.deltaTime * 10;

        _imagePistol.color = Color.Lerp(_imagePistol.color, _pistolBackgroundColor, lerp);
        _imageAutomatic.color = Color.Lerp(_imageAutomatic.color, _rifleBackgroundColor, lerp);
        _imageShotgun.color = Color.Lerp(_imageShotgun.color, _shotgunBackgroundColor, lerp);
        _imageEnergy.color = Color.Lerp(_imageEnergy.color, _heavyBackgroundColor, lerp);
        _imageExplosive.color = Color.Lerp(_imageExplosive.color, _colorBackgroundExplosive, lerp);
        _imageDefibrilator.color = Color.Lerp(_imageDefibrilator.color, _colorBackgroundDefibrilator, lerp);
        _imageBlades.color = Color.Lerp(_imageBlades.color, _meleeBackgroundColor, lerp);

        _textPistol.color = Color.Lerp(_textPistol.color, _pistolTextColor, lerp);
        _textAutomatic.color = Color.Lerp(_textAutomatic.color, _rifleTextColor, lerp);
        _textShotgun.color = Color.Lerp(_textShotgun.color, _shotgunTextColor, lerp);
        _textEnergy.color = Color.Lerp(_textEnergy.color, _heavyTextColor, lerp);
        _textExplosive.color = Color.Lerp(_textExplosive.color, _colorTextExplosive, lerp);
        _textDefibrilator.color = Color.Lerp(_textDefibrilator.color, _colorTextDefibrilator, lerp);
        _textBlades.color = Color.Lerp(_textBlades.color, _meleeTextColor, lerp);
    }
    
    public static void DisplayWeaponAs(WeaponType weaponType, EquipmentSystem.EquipMode mode)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                switch(mode)
                {
                    case EquipmentSystem.EquipMode.None:
                        instance._pistolBackgroundColor = instance.colorBackgroundEmpty;
                        instance._pistolTextColor = instance.colorTextEmpty;
                        break;

                    case EquipmentSystem.EquipMode.Stored:
                        instance._pistolBackgroundColor = instance.colorBackgroundHave;
                        instance._pistolTextColor = instance.colorTextHave;
                        break;

                    case EquipmentSystem.EquipMode.Equipped:
                        instance._pistolBackgroundColor = instance.colorBackgroundEquipped;
                        instance._pistolTextColor = instance.colorTextEquipped;
                        break;
                }
                break;

            case WeaponType.Rifle:
                switch (mode)
                {
                    case EquipmentSystem.EquipMode.None:
                        break;

                    case EquipmentSystem.EquipMode.Stored:
                        break;

                    case EquipmentSystem.EquipMode.Equipped:
                        break;
                }
                if (mode == EquipmentSystem.EquipMode.None) { 
                    instance._rifleBackgroundColor = instance.colorBackgroundEmpty; 
                    instance._rifleTextColor = instance.colorTextEmpty; 
                }
                else if (mode == EquipmentSystem.EquipMode.Stored) { 
                    instance._rifleBackgroundColor = instance.colorBackgroundHave; 
                    instance._rifleTextColor = instance.colorTextHave; 
                }
                else if (mode == EquipmentSystem.EquipMode.Equipped) { 
                    instance._rifleBackgroundColor = instance.colorBackgroundEquipped; 
                    instance._rifleTextColor = instance.colorTextEquipped; 
                }
                break;

            case WeaponType.Shotgun:
                switch (mode)
                {
                    case EquipmentSystem.EquipMode.None:
                        instance._shotgunBackgroundColor = instance.colorBackgroundEmpty;
                        instance._shotgunTextColor = instance.colorTextEmpty;
                        break;

                    case EquipmentSystem.EquipMode.Stored:
                        instance._shotgunBackgroundColor = instance.colorBackgroundHave;
                        instance._shotgunTextColor = instance.colorTextHave;
                        break;

                    case EquipmentSystem.EquipMode.Equipped:
                        instance._shotgunBackgroundColor = instance.colorBackgroundEquipped;
                        instance._shotgunTextColor = instance.colorTextEquipped;
                        break;
                }
                break;

            case WeaponType.Heavy:
                switch (mode)
                {
                    case EquipmentSystem.EquipMode.None:
                        instance._heavyBackgroundColor = instance.colorBackgroundEmpty;
                        instance._heavyTextColor = instance.colorTextEmpty;
                        break;

                    case EquipmentSystem.EquipMode.Stored:
                        instance._heavyBackgroundColor = instance.colorBackgroundHave;
                        instance._heavyTextColor = instance.colorTextHave;
                        break;

                    case EquipmentSystem.EquipMode.Equipped:
                        instance._heavyBackgroundColor = instance.colorBackgroundEquipped;
                        instance._heavyTextColor = instance.colorTextEquipped;
                        break;
                }
                break;

            case WeaponType.Melee:
                switch (mode)
                {
                    case EquipmentSystem.EquipMode.None:
                        instance._meleeBackgroundColor = instance.colorBackgroundEmpty;
                        instance._meleeTextColor = instance.colorTextEmpty;
                        break;

                    case EquipmentSystem.EquipMode.Stored:
                        instance._meleeBackgroundColor = instance.colorBackgroundHave;
                        instance._meleeTextColor = instance.colorTextHave;
                        break;

                    case EquipmentSystem.EquipMode.Equipped:
                        instance._meleeBackgroundColor = instance.colorBackgroundEquipped;
                        instance._meleeTextColor = instance.colorTextEquipped;
                        break;
                }
                break;
        }
    }
}

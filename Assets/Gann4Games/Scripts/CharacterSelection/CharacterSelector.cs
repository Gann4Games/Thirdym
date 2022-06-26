using UnityEngine;
using Gann4Games.Thirdym.ScriptableObjects;
public class CharacterSelector : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textCharacter;
    public TMPro.TextMeshProUGUI textFaction;
    public UnityEngine.UI.Slider sliderHealth;
    TMPro.TextMeshProUGUI _textHealth;
    public UnityEngine.UI.Slider sliderRegen;
    TMPro.TextMeshProUGUI _textRegen;

    SO_RagdollPreset[] _suitList;
    int _choosenSuit;

    GameObject _currentSuit;

    private void Start()
    {
        _suitList = PlayerPreferences.instance.suit_list;
        _choosenSuit = PlayerPreferences.instance.json_structure.choosen_suit;

        _textHealth = sliderHealth.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _textRegen = sliderRegen.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (_currentSuit == null && transform.childCount > 0) _currentSuit = transform.GetChild(0).gameObject;

        LoadSuit();
        UpdateInformationLabel();
    }
    public void UpdateInformationLabel()
    {
        // Not zombie code, will show player saved content soon.
        // 
        // string saved_character_name = suit_list[PlayerPreferences.instance.json_structure.choosen_suit].character_name;
        // int saved_character_id = PlayerPreferences.instance.json_structure.choosen_suit;

        textCharacter.text = _suitList[_choosenSuit].character_name+string.Format("\n(by {0})", _suitList[_choosenSuit].author);
        textFaction.text = "Faction " + _suitList[_choosenSuit].faction.Split('/')[1];

        sliderHealth.value = _suitList[_choosenSuit].maximumHealth;
        _textHealth.text = "Health (" + _suitList[_choosenSuit].maximumHealth + ")";

        sliderRegen.value = _suitList[_choosenSuit].regeneration_rate;
        _textRegen.text = "Regeneration rate (" + _suitList[_choosenSuit].regeneration_rate + ")";
    }
    public void SaveSuitSelection()
    {
        PlayerPreferences.instance.json_structure.choosen_suit = _choosenSuit;
        PlayerPreferences.instance.RefreshJsonFile();
        string selectedCharacterName = _suitList[_choosenSuit].character_name;
        NotificationHandler.Notify(string.Format("{0} was selected.", selectedCharacterName));
    }
    public void NextSuit()
    {
        _choosenSuit += 1;
        if (_choosenSuit >= PlayerPreferences.instance.suit_count) _choosenSuit = 0;
        LoadSuit();
    }
    public void PrevSuit()
    {
        _choosenSuit -= 1;
        if (_choosenSuit < 0) { _choosenSuit = PlayerPreferences.instance.suit_count - 1; }
        LoadSuit();
    }
    void LoadSuit()
    {
        RemovePreviousSuit();
        _currentSuit = Instantiate(_suitList[_choosenSuit].battleSuit, transform.position, transform.rotation, transform);
    }
    void RemovePreviousSuit()
    {
        if (_currentSuit) Destroy(_currentSuit);
    }
}

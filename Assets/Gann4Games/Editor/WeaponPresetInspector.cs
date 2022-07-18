using UnityEngine;
using UnityEditor;
using Gann4Games.Thirdym.ScriptableObjects;

namespace Gann4Games.EditorTools
{
    [CustomEditor(typeof(SO_WeaponPreset))]
    public class WeaponPresetInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            SO_WeaponPreset weaponPreset = (SO_WeaponPreset)target;
            Texture2D weaponIcon = null;

            if (weaponPreset.weaponIcon) weaponIcon = AssetPreview.GetAssetPreview(weaponPreset.weaponIcon);
            else EditorGUILayout.HelpBox("No icon has been assigned.", MessageType.Info);

            if (weaponPreset.fireSoundEffects.Length < 1)
                EditorGUILayout.HelpBox("Don't forget to add sounds to the weapon.", MessageType.Info);
            if (!weaponPreset.objectToDrop)
                EditorGUILayout.HelpBox("The object to drop can't be empty!", MessageType.Warning);
            if (!weaponPreset.characterAnimationOverride)
                EditorGUILayout.HelpBox("The animation override can't be empty!", MessageType.Warning);
            if (!weaponPreset.rightWeaponModel)
                EditorGUILayout.HelpBox("You need to assign a model at least to the right hand! (Right Weapon Model)", MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            if(weaponIcon) GUILayout.Label(weaponIcon);
            GUILayout.Label(weaponPreset.weaponName);
            EditorGUILayout.EndHorizontal();
            GUILayout.Label($"Damage: {weaponPreset.weaponDamage*weaponPreset.bulletSpawnCount}");
            base.OnInspectorGUI();
        }
    }
}

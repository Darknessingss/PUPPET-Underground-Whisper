using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponItem))]
public class WeaponItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var so = serializedObject;
        so.Update();

        EditorGUILayout.PropertyField(so.FindProperty("itemID"));
        EditorGUILayout.PropertyField(so.FindProperty("displayName"));
        EditorGUILayout.PropertyField(so.FindProperty("icon"));
        EditorGUILayout.PropertyField(so.FindProperty("worldPrefab"));

        var categoryProp = so.FindProperty("category");
        EditorGUILayout.PropertyField(categoryProp);

        bool isRange = categoryProp.enumValueIndex == (int)WeaponCategory.Range;
        var usesAmmoProp = so.FindProperty("usesAmmo");

        if (isRange)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Range", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("bulletPrefab"));
            EditorGUILayout.PropertyField(usesAmmoProp);

            if (usesAmmoProp.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(so.FindProperty("magazineSize"));
                EditorGUILayout.PropertyField(so.FindProperty("maxReserveAmmo"));
                EditorGUILayout.PropertyField(so.FindProperty("reloadTime"));
            }
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Melee", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("slashPrefab"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(so.FindProperty("damage"));
        EditorGUILayout.PropertyField(so.FindProperty("attackSpeed"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            so.FindProperty("attackSound"),
            new GUIContent(isRange ? "Shot Sound" : "Attack Sound"));

        if (isRange && usesAmmoProp.boolValue)
            EditorGUILayout.PropertyField(so.FindProperty("reloadSound"));

        EditorGUILayout.PropertyField(so.FindProperty("volume"));
        EditorGUILayout.PropertyField(so.FindProperty("audioMixerGroup"));

        so.ApplyModifiedProperties();
    }
}
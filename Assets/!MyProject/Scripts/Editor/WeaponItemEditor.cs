using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponItem))]
public class WeaponItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("worldPrefab"));

        var categoryProp = serializedObject.FindProperty("category");
        EditorGUILayout.PropertyField(categoryProp);
        bool isRange = categoryProp.enumValueIndex == (int)WeaponCategory.Range;

        if (!isRange)
        {
            EditorGUILayout.LabelField("Melee", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("slashPrefab"));
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Range", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletPrefab"));

            var usesAmmoProp = serializedObject.FindProperty("usesAmmo");
            EditorGUILayout.PropertyField(usesAmmoProp);

            EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackSpeed"));
            EditorGUILayout.Space();


            if (usesAmmoProp.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("magazineSize"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxReserveAmmo"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("reloadTime"));
                EditorGUILayout.Space();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
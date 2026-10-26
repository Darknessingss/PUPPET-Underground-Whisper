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

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackSpeed"));

        bool isRange = categoryProp.enumValueIndex == (int)WeaponCategory.Range;

        if (isRange)
        {
            EditorGUILayout.Space();
            var usesAmmoProp = serializedObject.FindProperty("usesAmmo");
            EditorGUILayout.PropertyField(usesAmmoProp);

            if (usesAmmoProp.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("magazineSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxReserveAmmo"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
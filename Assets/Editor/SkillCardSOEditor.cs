using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillCardSO))]
public class SkillCardSOEditor : Editor
{
    #region List of Serialized Property

        SerializedProperty nameSkillCard;
        SerializedProperty descriptionSkillCard;
        SerializedProperty iconSkillCard;
        SerializedProperty damageTypeSkillCard;
        SerializedProperty attackBoostSkillCard;
        SerializedProperty durationActiveSkillCard;
        SerializedProperty cooldownSkillCard;
        SerializedProperty arrowData;
        SerializedProperty explosionData;
        SerializedProperty prefabSkillTargeting;
        SerializedProperty targetSkillCard;
        SerializedProperty isAuto;
        SerializedProperty arrowVelocity;
        private SerializedProperty shotAudioClip;
 
    #endregion

    private void OnEnable()
    {
        nameSkillCard = serializedObject.FindProperty("nameSkillCard");
        descriptionSkillCard = serializedObject.FindProperty("descriptionSkillCard");
        iconSkillCard = serializedObject.FindProperty("iconSkillCard");
        damageTypeSkillCard = serializedObject.FindProperty("damageTypeSkillCard");
        attackBoostSkillCard = serializedObject.FindProperty("attackBoostSkillCard");
        durationActiveSkillCard = serializedObject.FindProperty("durationActiveSkillCard");
        cooldownSkillCard = serializedObject.FindProperty("cooldownSkillCard");
        targetSkillCard =  serializedObject.FindProperty("targetSkillCard");
        arrowData = serializedObject.FindProperty("arrowData");
        explosionData = serializedObject.FindProperty("explosionData");
        prefabSkillTargeting = serializedObject.FindProperty("prefabSkillTargeting");
        isAuto =  serializedObject.FindProperty("isAuto");
        arrowVelocity = serializedObject.FindProperty("arrowVelocity");
        shotAudioClip = serializedObject.FindProperty("shotAudioClip");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(nameSkillCard);
        EditorGUILayout.PropertyField(descriptionSkillCard);
        EditorGUILayout.PropertyField(iconSkillCard);
        EditorGUILayout.PropertyField(arrowVelocity);
        EditorGUILayout.PropertyField(isAuto);
        EditorGUILayout.PropertyField(shotAudioClip);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Skill Card Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(damageTypeSkillCard);
        EditorGUILayout.PropertyField(attackBoostSkillCard);
        EditorGUILayout.PropertyField(durationActiveSkillCard);
        EditorGUILayout.PropertyField(cooldownSkillCard);
        EditorGUILayout.PropertyField(targetSkillCard);
        
        
        EditorGUILayout.Space();
        SkillDamageType damageType = (SkillDamageType)damageTypeSkillCard.enumValueIndex;

        switch (damageType)
        {
            case SkillDamageType.Arrow:
                EditorGUILayout.LabelField("Arrow Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(arrowData, true);
                break;
            
            case SkillDamageType.Explosion:
                EditorGUILayout.LabelField("Explosion Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(explosionData, true);
                break;
            
            case SkillDamageType.None:
                EditorGUILayout.HelpBox("Select a Skill Damage Type to configure this skill.", MessageType.Info);
                break;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Skill Card Targeting Prefab", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prefabSkillTargeting);
        
        serializedObject.ApplyModifiedProperties();
    }
}

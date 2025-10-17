using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpellData))]
public class SpellDataEditor : Editor
{
    // Riferimenti alle proprietà serializzate Comuni
    private SerializedProperty spellNameProp;
    private SerializedProperty spriteProp;
    private SerializedProperty spriteTintProp;
    private SerializedProperty effectProp;

    // Riferimenti a tutte le Proprietà Condizionali
    private SerializedProperty baseDamageProp;
    private SerializedProperty totalHitsProp;
    private SerializedProperty timeBetweenHitsProp;
    private SerializedProperty damageIncrementProp;

    private void OnEnable()
    {
        // Proprietà Comuni
        spellNameProp = serializedObject.FindProperty("spellName");
        spriteProp = serializedObject.FindProperty("sprite");
        spriteTintProp = serializedObject.FindProperty("spriteTint");
        effectProp = serializedObject.FindProperty("effect");

        // Proprietà Condizionali (devono corrispondere ESATTAMENTE ai nomi in SpellData)
        baseDamageProp = serializedObject.FindProperty("baseDamage");
        totalHitsProp = serializedObject.FindProperty("totalHits");
        timeBetweenHitsProp = serializedObject.FindProperty("timeBetweenHits");
        damageIncrementProp = serializedObject.FindProperty("damageIncrement");
    }

    public override void OnInspectorGUI()
    {
        // Inizia a monitorare eventuali modifiche
        serializedObject.Update();

        // -----------------------------------------------------
        // --- SEZIONE 1: Appearance (Comune a tutti) ---
        // -----------------------------------------------------
        EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spellNameProp);
        EditorGUILayout.PropertyField(spriteProp);
        EditorGUILayout.PropertyField(spriteTintProp);

        EditorGUILayout.Space(10);

        // -----------------------------------------------------
        // --- SEZIONE 2: Effect Type Selector ---
        // -----------------------------------------------------
        EditorGUILayout.LabelField("Effect", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(effectProp);

        
        // Ottiene il valore corrente dell'enum 'effect'
        SpellData.EffectType currentEffect = (SpellData.EffectType)effectProp.enumValueIndex;

        // -----------------------------------------------------
        // --- SEZIONE 3: Logica Condizionale ---
        // -----------------------------------------------------
        
        // La baseDamage è condivisa tra tutti
        EditorGUILayout.PropertyField(baseDamageProp, new GUIContent("Base Damage"));

        switch (currentEffect)
        {
            case SpellData.EffectType.ONESHOT:
                // Proprietà: Solo baseDamage (già mostrata sopra)
                break;

            case SpellData.EffectType.MULTIPLE:
                // Proprietà: baseDamage, totalHits, timeBetweenHits
                
                EditorGUILayout.Space(5);
                
                EditorGUILayout.PropertyField(totalHitsProp, new GUIContent("Total Hits"));
                EditorGUILayout.PropertyField(timeBetweenHitsProp, new GUIContent("Time Between Hits (s)"));
                
                break;
            
            case SpellData.EffectType.INCREMENTAL:
                // Proprietà: baseDamage, totalHits, timeBetweenHits, damageIncrement
                
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(totalHitsProp, new GUIContent("Total Hits"));
                EditorGUILayout.PropertyField(timeBetweenHitsProp, new GUIContent("Time Between Hits (s)"));
                EditorGUILayout.PropertyField(damageIncrementProp, new GUIContent("Damage Increment"));
                
                break;
        }

        // Applica le modifiche all'oggetto ScriptableObject
        serializedObject.ApplyModifiedProperties();
    }
}
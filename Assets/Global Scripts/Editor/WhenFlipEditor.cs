using UnityEditor;

//[CustomEditor(typeof(HidePartWhenFlip))]
public class WhenFlipEditor : Editor
{
    // The various categories the editor will display the variables in 
    public enum DisplayCategory
    {
        Sprite, Object, LocalScale
    }

    // The enum field that will determine what variables to display in the Inspector
    public DisplayCategory categoryToDisplay;

    /*[System.Serializable] 
    public class config{
        public DisplayCategory[] categories;
    }*/

    //public config Config;

    // The function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        // Display the enum popup in the inspector
        categoryToDisplay = (DisplayCategory) EditorGUILayout.EnumPopup("Display", categoryToDisplay);

        // Create a space to separate this enum popup from other variables 
        EditorGUILayout.Space(); 
        
        // Switch statement to handle what happens for each category
        switch (categoryToDisplay)
        {
            case DisplayCategory.Sprite:
                DisplaySpriteInfo(); 
                break;

            case DisplayCategory.Object:
                DisplayObjectInfo();
                break;

            case DisplayCategory.LocalScale:
                DisplayLocalScaleInfo();
                break; 

        }
        serializedObject.ApplyModifiedProperties();
    }

    // When the categoryToDisplay enum is at "Basic"
    void DisplaySpriteInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("part"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("numberSpriteLevels"));
    }

    // When the categoryToDisplay enum is at "Combat"
    void DisplayObjectInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("part"));
    }

    void DisplayLocalScaleInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("part"));
    }

    /*  THIS IS FOR HIDEAWAY PROPERTIES BEHIND ONE ENABLING BOOL
    // When the categoryToDisplay enum is at "Magic"
    void DisplayMagicInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("magicResistance"));
        
        // Store the hasMagic bool as a serializedProperty so we can access it
        SerializedProperty hasMagicProperty = serializedObject.FindProperty("hasMagic");

        // Draw a property for the hasMagic bool
        EditorGUILayout.PropertyField(hasMagicProperty);

        // Check if hasMagic is true
        if (hasMagicProperty.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mana"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("magicType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("magicDamage"));
        }
    }*/
}
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using TinyGiantStudio.Modules;

#if MODULAR_3D_TEXT
using TinyGiantStudio.Text;
#endif

using TinyGiantStudio.EditorHelpers;


namespace TinyGiantStudio.Layout
{
    [CustomEditor(typeof(LinearLayoutGroup))]
    public class LinearLayoutGroupEditor : Editor
    {
#if MODULAR_3D_TEXT
        static AssetSettings settings;
#endif


        LinearLayoutGroup myTarget;
        SerializedObject soTarget;


        SerializedProperty autoItemSize;
        SerializedProperty spacing;
        SerializedProperty alignment;
        SerializedProperty elementUpdater;

        SerializedProperty alwaysUpdateInPlayMode;
        SerializedProperty alwaysUpdateBounds;

        SerializedProperty randomizeRotations;
        SerializedProperty minimumRandomRotation;
        SerializedProperty maximumRandomRotation;

        SerializedProperty bounds;

        AnimBool showDebug;


        private static GUIStyle foldOutStyle = null;
        static GUIStyle toggleStyle = null;

        private static Color openedFoldoutTitleColor = new Color(124 / 255f, 170 / 255f, 239 / 255f, 0.9f);


        void OnEnable()
        {
            myTarget = (LinearLayoutGroup)target;
            soTarget = new SerializedObject(target);

#if MODULAR_3D_TEXT
            if (!settings)
                settings = StaticMethods.VerifySettings(settings);
#endif
            FindProperties();
        }


        public override void OnInspectorGUI()
        {
            GenerateStlye();
            EditorGUI.BeginChangeCheck();

            Alignment();

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(autoItemSize);
            EditorGUILayout.PropertyField(spacing);


            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(randomizeRotations);
            if (myTarget.randomizeRotations)
            {
                EditorGUILayout.PropertyField(minimumRandomRotation);
                EditorGUILayout.PropertyField(maximumRandomRotation);
            }

            EditorGUILayout.Space(5);
            if (ModuleDrawer.ElementUpdatersExist())
                ModuleDrawer.ElementUpdaterContainerList("Element Updater", "", myTarget.elementUpdater, elementUpdater, soTarget);

            EditorGUILayout.Space(5);
            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateInPlayMode, "Always update in playmode", "For performance, it's better to leave it to false and call UpdateLayout() after making changes.\nTurn this on if you are in a hurry or testing stuff.", FieldSize.gigantic);
            MText_Editor_Methods.ItalicHorizontalField(alwaysUpdateBounds, "Always update bounds", "For performance, it's better to leave it to false and call GetAllChildBounds() when a bound(size of an element) changes", FieldSize.gigantic);
            EditorGUILayout.Space(5);

            DebugFoldout();
            if (EditorGUI.EndChangeCheck())
            {
                //soTarget.ApplyModifiedProperties();
                LinearLayoutGroup.Alignment anchor = myTarget.alignment;
                if (soTarget.ApplyModifiedProperties())
                {
                    myTarget.rotationChanged = true;
#if MODULAR_3D_TEXT
                    if (myTarget.GetComponent<Modular3DText>())
                    {
                        if (anchor != myTarget.alignment)
                        {
                            myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                        }
                        else
                        {
                            //if (!myTarget.GetComponent<Modular3DText>().ShouldItCreateChild())
                            {
                                myTarget.GetComponent<Modular3DText>().CleanUpdateText();
                            }
                        }
                    }
#endif
                }
                //EditorUtility.SetDirty(myTarget);
            }
        }

        static Color toggledOnButtonColor = Color.white;
        static Color toggledOffButtonColor = Color.gray;
        void Alignment()
        {
            EditorGUILayout.PropertyField(alignment, GUIContent.none);

            GUILayout.BeginHorizontal();
            Color originalColor = GUI.color;


            if (myTarget.secondaryAlignment == LinearLayoutGroup.Alignment.Top)
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (LeftButton(EditorGUIUtility.IconContent("d_align_vertically_top")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                myTarget.secondaryAlignment = LinearLayoutGroup.Alignment.Top;
                EditorUtility.SetDirty(myTarget);
            }


            if (myTarget.secondaryAlignment == LinearLayoutGroup.Alignment.VerticleMiddle)
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (MidButton(EditorGUIUtility.IconContent("d_align_vertically_center")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                myTarget.secondaryAlignment = LinearLayoutGroup.Alignment.VerticleMiddle;
                EditorUtility.SetDirty(myTarget);
            }


            if (myTarget.secondaryAlignment == LinearLayoutGroup.Alignment.Bottom)
                GUI.color = toggledOnButtonColor;
            else
                GUI.color = toggledOffButtonColor;
            if (RightButton(EditorGUIUtility.IconContent("d_align_vertically_bottom")))
            {
                Undo.RecordObject(myTarget, "Update layout");
                myTarget.secondaryAlignment = LinearLayoutGroup.Alignment.Bottom;
                EditorUtility.SetDirty(myTarget);
            }
            GUILayout.EndHorizontal();
            GUI.color = originalColor;
        }




        private static void GenerateStlye()
        {
#if MODULAR_3D_TEXT
            if (EditorGUIUtility.isProSkin)
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_darkSkin;
            }
            else
            {
                if (settings)
                    openedFoldoutTitleColor = settings.openedFoldoutTitleColor_lightSkin;
            }
#endif
            if (foldOutStyle == null)
            {
                foldOutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    overflow = new RectOffset(-10, 0, 3, 0),
                    padding = new RectOffset(15, 0, -3, 0),
                    fontStyle = FontStyle.Bold
                };
                foldOutStyle.onNormal.textColor = openedFoldoutTitleColor;
            }

            if (toggleStyle == null)
            {
                toggleStyle = new GUIStyle(GUI.skin.button);
                toggleStyle.margin = new RectOffset(0, 0, toggleStyle.margin.top, toggleStyle.margin.bottom);
            }

        }

        private void DebugFoldout()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical(EditorStyles.toolbar);
            showDebug.target = EditorGUILayout.Foldout(showDebug.target, "Debug", true, foldOutStyle);
            GUILayout.EndVertical();
            if (EditorGUILayout.BeginFadeGroup(showDebug.faded))
            {
                EditorGUILayout.PropertyField(bounds);
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.EndVertical();
        }











        bool LeftButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);

            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(0, 0, rect.width + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;

            GUI.EndGroup();
            return clicked;
        }
        bool MidButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);


            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left + toggleStyle.border.right, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }
        bool RightButton(GUIContent content)
        {
            bool clicked = false;
            Rect rect = GUILayoutUtility.GetRect(20, 20);


            GUI.BeginGroup(rect);
            if (GUI.Button(new Rect(-toggleStyle.border.left, 0, rect.width + toggleStyle.border.left, rect.height), content, toggleStyle))
                clicked = true;
            GUI.EndGroup();
            return clicked;
        }










        private void FindProperties()
        {
            autoItemSize = soTarget.FindProperty("autoItemSize");
            spacing = soTarget.FindProperty("spacing");

            randomizeRotations = soTarget.FindProperty("randomizeRotations");
            minimumRandomRotation = soTarget.FindProperty("_minimumRandomRotation");
            maximumRandomRotation = soTarget.FindProperty("maximumRandomRotation");

            alignment = soTarget.FindProperty("alignment");

            alwaysUpdateInPlayMode = soTarget.FindProperty("alwaysUpdateInPlayMode");
            alwaysUpdateBounds = soTarget.FindProperty("alwaysUpdateBounds");

            elementUpdater = soTarget.FindProperty("elementUpdater");

            bounds = soTarget.FindProperty("bounds");

            showDebug = new AnimBool(false);
            showDebug.valueChanged.AddListener(Repaint);
        }
    }
}
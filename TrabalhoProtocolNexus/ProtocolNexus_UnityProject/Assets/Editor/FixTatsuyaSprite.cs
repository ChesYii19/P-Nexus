using UnityEngine;
using UnityEditor;

public class FixTatsuyaSprite
{
    [InitializeOnLoadMethod]
    static void FixSprite()
    {
        string prefabPath = "Assets/Prefabs/Tatsuya_Completo.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Load the Idle sprite directly
                Sprite idleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/MainCharacter(FreePack)/MainCharacter(FreePack)/Idle.png");
                if (idleSprite != null)
                {
                    sr.sprite = idleSprite;
                }
            }

            Animator anim = prefab.GetComponent<Animator>();
            if (anim != null)
            {
                UnityEditor.Animations.AnimatorController controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Animations/Tatsuya_Controller.controller");
                if (controller == null)
                {
                    System.IO.Directory.CreateDirectory("Assets/Animations");
                    controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/Tatsuya_Controller.controller");
                }
                anim.runtimeAnimatorController = controller;
            }

            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            Debug.Log("o. Tatsuya Sprite and Animator Fixed!");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartUpSceneLoader
{
    static StartUpSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadStartUpScene;
    }

    private static void LoadStartUpScene(PlayModeStateChange state)
    {
        //실행버튼을 누른 순간.
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}

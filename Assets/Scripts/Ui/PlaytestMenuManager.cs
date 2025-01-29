#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;

public class PlaytestMenuManager : Singleton<PlaytestMenuManager> {
    
    public GameObject SceneSelectButtonPrefab;
    public GameObject ButtonParent;
    public float DistanceBetweenButtons = 200;

    public List<String> ScenesToCreate;

    float _offset = 0;
    List<GameObject> _playtestButtons = new List<GameObject>();

    protected override void Awake() {

        base.Awake();

        int totalScenes = ScenesToCreate.Count;

        for (int i = 0; i < totalScenes; i++) {
            var scenePath = ScenesToCreate[i];

            string sceneName = ObjectNames.NicifyVariableName(Path.GetFileNameWithoutExtension(scenePath));

            _playtestButtons.Add(CreateButton(sceneName, scenePath));
        }
    }

    /// <summary>
    /// Create a new scene Navigation Button 
    /// </summary>
    /// <param name="sceneName"> The name to display on Button. </param>
    /// <param name="scenePath"> Path to scene in Asset Database.</param>
    GameObject CreateButton(string sceneName, string scenePath) {

        // Create button
        var button = Instantiate(SceneSelectButtonPrefab, Vector3.zero, Quaternion.identity, ButtonParent.transform);
        var buttonManager = button.GetComponent<ButtonManager>();

        if (buttonManager == null) return button;

        // Align to position
        buttonManager.SetLocalPos(new Vector3(0, _offset, 0));
        _offset -= DistanceBetweenButtons;

        // Set name
        buttonManager.SetText(sceneName);

        // Link Button to scene
        buttonManager.SetOnClick(delegate () { SceneManager.LoadScene(scenePath); });

        return button;
    }
}

[CustomEditor(typeof(PlaytestMenuManager))]

public class PlaytestMenuEditor : Editor {

    /// <summary>
    /// Init button to load all scenes in build path.
    /// </summary>
    public override void OnInspectorGUI() {

        base.OnInspectorGUI();

        // Add a custom button in the Inspector

        if (GUILayout.Button("Add Scenes to Playtest Menu.")) {

            PlaytestMenuManager playtestMenu = (PlaytestMenuManager)target;

            int oldCount = playtestMenu.ScenesToCreate.Count;

            playtestMenu.ScenesToCreate = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                playtestMenu.ScenesToCreate.Add(scene.path);
            }

            Debug.Log("Added " + (playtestMenu.ScenesToCreate.Count - oldCount) + " to scene!");
        }
    }
}

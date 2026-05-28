//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2026 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class RCCP_MirrorInitLoad : MonoBehaviour {

    /// <summary>
    /// EditorPrefs key set during first-import (Cycle 1, pre-recompile) to request that the
    /// next InitOnLoad cycle (Cycle 2, post-recompile) show the welcome dialog and offer the
    /// RP material conversion. Persisted via EditorPrefs so it survives the domain reload
    /// triggered by the RCCP_MIRROR symbol set — modal dialogs and EditorWindow opens during
    /// the reload race the editor's HostView wiring and can NRE / hang.
    /// </summary>
    private const string PendingFirstRunKey = "RCCP_Mirror_PendingFirstRun";

    [InitializeOnLoadMethod]
    static void InitOnLoad() {

        EditorApplication.delayCall += EditorUpdate;

    }

    public static void EditorUpdate() {

        bool hasKey = false;

#if RCCP_MIRROR
        hasKey = true;
#endif

        if (!hasKey) {

            // Cycle 1: flipping RCCP_MIRROR queues a recompile + domain reload. The original
            // code showed a modal dialog and then synchronously opened RCCP_RenderPipelineConverterWindow
            // in the same call — both race the reload (dialogs hang, set_position NREs because
            // m_Parent isn't wired). Defer everything to the next InitOnLoad cycle.
            EditorPrefs.SetBool(PendingFirstRunKey, true);
            RCCP_SetScriptingSymbol.SetEnabled("RCCP_MIRROR", true);

        } else if (EditorPrefs.GetBool(PendingFirstRunKey, false)) {

            // Cycle 2 (post-recompile after first import): clear the flag and run the
            // first-run UI once the editor is settled.
            EditorPrefs.DeleteKey(PendingFirstRunKey);

            EditorApplication.delayCall += () => {

                EditorUtility.DisplayDialog("Realistic Car Controller Pro | Mirror For Realistic Car Controller Pro", "Be sure you have imported latest Mirror to your project. Run the RCCP_Scene_Blank_Mirror demo scene. You can find more detailed info in documentation.", "Close");

                RCCP_SceneUpdater.Check();

                RenderPipelineAsset rp = GraphicsSettings.currentRenderPipeline;

                if (rp == null)   // Built-in - nothing to convert
                    return;

                bool isURP = rp.GetType().ToString().Contains("Universal");
                bool isHDRP = rp.GetType().ToString().Contains("HD");

                if (!isURP && !isHDRP)
                    return;

                string rpName = isURP ? "URP" : "HDRP";
                bool ok = EditorUtility.DisplayDialog(
                    "Convert Materials",
                    $"Your project is using {rpName}.\n\n" +
                    $"You'll need to convert the imported assets to be working with {rpName}.\n\n" +
                    $"You can open the RCCP Render Pipeline Converter Window and proceed.",
                    "Yes, open converter",
                    "No thanks"
                );

                if (!ok)
                    return;

                RCCP_RenderPipelineConverterWindow.Init();

            };

        }

    }

}

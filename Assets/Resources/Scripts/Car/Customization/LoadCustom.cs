using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies the visual customization chosen in the garage to this vehicle when its scene loads.
/// Attach to the vehicle root (the object holding RCCP_CarController).
///
/// Reads the SaveCustom container, which reaches this scene because SaveCustomData carries
/// DontDestroy. It used to read ChangeSpoilers.instance / ChangeWheels.instance instead: those
/// live on ComponentUpgrade, destroyed with the garage scene, so their statics pointed at dead
/// objects and every "!= null" check silently failed Unity's fake-null test.
/// </summary>
public class LoadCustom : MonoBehaviour {

    [Tooltip("Left empty, it resolves from this GameObject.")]
    public RCCP_CarController controller;

    [Tooltip("Wheel renderers repainted by the wheel material choice.")]
    public List<MeshRenderer> wheels;

    [Tooltip("Wheel materials, in the same order as the garage's list.")]
    public List<Material> wheelMaterials;

    [Tooltip("Spoiler variants, in the same order as the garage's list.")]
    public List<GameObject> spoilers;

    void Awake() {
        if (controller == null)
            controller = GetComponent<RCCP_CarController>();

        if (controller == null)
            controller = GetComponentInParent<RCCP_CarController>();

        SaveCustom data = FindFirstObjectByType<SaveCustom>(FindObjectsInactive.Include);

        if (data == null) {
            Debug.LogWarning("LoadCustom: no SaveCustom found, vehicle keeps its default look.", this);
            return;
        }

        ApplyBodyColor(data);
        ApplySpoiler(data);
        ApplyWheelMaterial(data);
    }

    // Alpha 0 means the player never picked a colour, so the prefab's own paint is left alone.
    void ApplyBodyColor(SaveCustom data) {
        if (data.bodyColor.a <= 0f || controller == null || controller.Customizer == null)
            return;

        RCCP_VehicleUpgrade_PaintManager paintManager = controller.Customizer.PaintManager;

        if (paintManager != null)
            paintManager.Paint(data.bodyColor);
    }

    void ApplySpoiler(SaveCustom data) {
        if (spoilers == null || data.currentSpoiler < 0 || data.currentSpoiler >= spoilers.Count)
            return;

        if (spoilers[data.currentSpoiler] != null)
            spoilers[data.currentSpoiler].SetActive(true);
    }

    void ApplyWheelMaterial(SaveCustom data) {
        if (wheels == null || wheelMaterials == null)
            return;

        if (data.currentMat < 0 || data.currentMat >= wheelMaterials.Count)
            return;

        Material material = wheelMaterials[data.currentMat];

        if (material == null)
            return;

        foreach (MeshRenderer wheel in wheels) {
            if (wheel != null)
                wheel.material = material;
        }
    }
}

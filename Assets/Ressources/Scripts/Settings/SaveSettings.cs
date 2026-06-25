using UnityEditor.TerrainTools;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static RCCP_Gearbox;
using static RCCP_Gearbox.CurrentGearState;

public class SaveSettings : MonoBehaviour, IGameData {

    public string dataDirPath;
    public string dataFileName;
    public bool useEncryption;
    public string encryption;
    public bool usePrettyPrint;

    // Suspension
    public float distValue;
    public float forceValue;
    public float targetValue;
    public float damperValue;

    // Driving Aid
    public bool ABS;
    public bool TCS;
    public bool ESP;
    public bool SH;
    public bool TH;
    public float ASPValue;

    // Wheels
    //public float steeringCurve;
    public float sensitivityValue;

    // Camber
    public float frontAngle;
    public float rearAngle;

    // Grip
    public float forwardValue;
    public float rearSidewaysValue;
    public float frontSidewaysValue;

    // Braking
    public bool isHandbrake;
    public float handbrakeMultiplier;
    public float brakeMultiplier;

    // Gearbox
    public GearState isReverse;
    public TransmissionType transmissionType;
    public float GSTValue;
    public float shiftingDelay;
    public float clutchThreshold;
    public int ARFValue;

    // Propulsion Type
    public bool frontAxleSteer;
    public bool frontAxleHandbrake;
    public bool rearAxleSteer;
    public bool rearAxleHandbrake;

    // Others
    public float steerValue;
    public float throttleValue;
    public float brakeValue;
    public float handbrakeValue;
    public float clutchValue;

    public bool isTelemetry = false;

    void IGameData.setData(IGameData data) {
        SaveSettings tmp = data as SaveSettings;

        distValue = tmp.distValue;
        forceValue = tmp.forceValue;
        targetValue = tmp.targetValue;
        damperValue = tmp.damperValue;

        ABS = tmp.ABS;
        TCS = tmp.TCS;
        ESP = tmp.ESP;
        SH = tmp.SH;
        TH = tmp.TH;
        ASPValue = tmp.ASPValue;

        //steeringCurve = tmp.steeringCurve;
        sensitivityValue = tmp.sensitivityValue;

        frontAngle = tmp.frontAngle;
        rearAngle = tmp.rearAngle;

        forwardValue = tmp.forwardValue;
        rearSidewaysValue = tmp.rearSidewaysValue;
        frontSidewaysValue = tmp.frontSidewaysValue;

        isHandbrake = tmp.isHandbrake;
        handbrakeMultiplier = tmp.handbrakeMultiplier;
        brakeMultiplier = tmp.brakeMultiplier;

        isReverse = tmp.isReverse;
        transmissionType = tmp.transmissionType;
        GSTValue = tmp.GSTValue;
        shiftingDelay = tmp.shiftingDelay;
        clutchThreshold = tmp.clutchThreshold;
        ARFValue = tmp.ARFValue;

        frontAxleSteer = tmp.frontAxleSteer;
        frontAxleHandbrake = tmp.frontAxleHandbrake;
        rearAxleSteer = tmp.rearAxleSteer;
        rearAxleHandbrake = tmp.rearAxleHandbrake;

        steerValue = tmp.steerValue;
        throttleValue = tmp.throttleValue;
        brakeValue = tmp.brakeValue;
        handbrakeValue = tmp.handbrakeValue;
        clutchValue = tmp.clutchValue;

        isTelemetry = tmp.isTelemetry;
    }

    string IGameData.getDataDirPath() {
        return dataDirPath;
    }

    string IGameData.getDataFileName() {
        return dataFileName;
    }

    bool IGameData.useEncryption() {
        return useEncryption;
    }

    string IGameData.getEncryptionKey() {
        return encryption;
    }

    bool IGameData.usePrettyPrint() {
        return usePrettyPrint;
    }
}

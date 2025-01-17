﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour {

    public GameObject MissionObject;
    public Text DisplayText;
    [HideInInspector] public Targets TargetScript;

    MissionsAbstract Mission;
    MissionsAbstract[] MissionsList;

    List<MissionTargets> MissionTargets;
    int NumberOfFases; //quantity of fases a mission have
    int CurrentFase = 0;

    GameObject Cargo;
    int CargoFase;
    bool UsesCargo = false;
    bool CargoSpawned = false;

    int MissionNumber = 0;

    void Awake() {
        MissionsList = MissionObject.GetComponents<MissionsAbstract>();
        TargetScript = MissionObject.GetComponent<Targets>();

        for (int i = 0; i < MissionsList.Length; i++) {
            MissionsAbstract temp = MissionsList[i];
            int randomIndex = Random.Range(i, MissionsList.Length);
            MissionsList[i] = MissionsList[randomIndex];
            MissionsList[randomIndex] = temp;
        }
    }

    // Use this for initialization
    void Start () {
        TargetScript.SetMission(this);
        RandomizeMission();
    }

    public void RandomizeMission() {
        MissionNumber = MissionNumber + 1;
        if (MissionNumber >= MissionsList.Length) {
            MissionNumber = 0;
        }

        Mission = MissionsList[MissionNumber];
        Mission.InitiateMission(this);
    }

    public void InitMission(MissionsAbstract currentMission, List<MissionTargets> missionTargets, GameObject missionModel = null, int numberOfFases = 1) {
        if (missionModel != null)  GameManager.Player.ReplaceModel(missionModel); //change player model if needed

        MissionTargets = missionTargets;
        SetMissionTarget(); //Set mission target [default = 0]

        NumberOfFases = numberOfFases;
        CurrentFase = 0;

        SetDisplayText();
    }

    public void SetCargo(GameObject cargo = null, int cargoFase = 0) {
        Cargo = cargo;
        CargoFase = cargoFase;
        if (cargo != null) UsesCargo = true;
    }


    public void SetArrowDisplay(bool state) {
        TargetScript.SetArrowDisplay(state);
    }

    public void SetTriggerPointDisplay(bool state) {
        TargetScript.SetTriggerPointDisplay(state);
    }

    public void GoalCompleted() {
        IncrementCurrentFase();
        if (UsesCargo && CargoSpawned) {
            GameManager.Player.Model.GetComponent<SpawnCargo>().DestroyCargo();
            CargoSpawned = false;
        }
        if (IsMissionCompleted()) RandomizeMission();
        else NextFase();
    }

    public void NextFase() {
        if(UsesCargo && CargoFase == CurrentFase) {
            GameManager.Player.Model.GetComponent<SpawnCargo>().Spawn(Cargo, this);
            CargoSpawned = true;
        }
        SetMissionTarget(CurrentFase);
        SetDisplayText();
    }

    public void MissionFailed() {
        Debug.Log("mission failed");
        TargetScript.MissionFailed(); //destroy current target
        CargoSpawned = false;
        RandomizeMission();
    }

    void SetDisplayText() {
        DisplayText.enabled = true;
        DisplayText.text = MissionTargets[CurrentFase].MissionDescription;
        DisplayText.GetComponent<TextMissionTween>().StartTextTween();
    }

    void SetMissionTarget(int targetIndex = 0) {
        TargetScript.GetComponent<Targets>().NewTarget(MissionTargets[targetIndex].Target);
    }

    void IncrementCurrentFase() {
        CurrentFase++;
    }

    bool IsMissionCompleted() {
        if (CurrentFase >= NumberOfFases)
            return true;
        else
            return false;
    }
}

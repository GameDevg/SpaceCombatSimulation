﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Src.Evolution;
using Assets.Src.Database;
using System;
using UnityEngine.SceneManagement;
using Assets.Src.Menus;

public class EditBrConfig : MonoBehaviour {
    private int _loadedId = -1;
    private bool _hasLoadedExisting;

    public EditMutationConfig MutationConfig;
    public EditMatchConfig MatchConfig;

    public InputField RunName;
    public InputField MinMatchesPerIndividual;
    public InputField WinnersFromEachGeneration;
    private int _generationNumber;
    public InputField NumberOfTeams;
    public InputField InSphereRandomisationRadius;
    public InputField OnSphereRandomisationRadius;

    public Button RunButton;
    public Button CoppyButton;
    public Button CancelButton;
    
    public string EvolutionSceneToLoad = "1v1Evolution";
    public string MainMenuSceneToLoad = "MainMenu";

    private EvolutionBrDatabaseHandler _handler;
    private EvolutionBrConfig _loaded;

    // Use this for initialization
    void Start () {
        _handler = new EvolutionBrDatabaseHandler();

        LoadConfig();
        
        RunButton.onClick.AddListener(delegate () { SaveAndRun(); });
        CoppyButton.onClick.AddListener(delegate () { SaveNewAndRun(); });
        CancelButton.onClick.AddListener(delegate () { ReturnToMainMenu(); });
    }

    private void ReturnToMainMenu()
    {
        if (!string.IsNullOrEmpty(MainMenuSceneToLoad))
        {
            SceneManager.LoadScene(MainMenuSceneToLoad);
        }
    }

    private void SaveAndRun()
    {
        var config = ReadControlls();

        if (_hasLoadedExisting)
        {
            _handler.UpdateExistingConfig(config);
        } else
        {
            _loadedId = _handler.SaveNewConfig(config);
        }

        ArgumentStore.IdToLoad = _loadedId;
        
        SceneManager.LoadScene(EvolutionSceneToLoad);
    }

    private void SaveNewAndRun()
    {
        var config = ReadControlls();

        config.GenerationNumber = 0;

        _loadedId = _handler.SaveNewConfig(config);

        ArgumentStore.IdToLoad = _loadedId;
        
        SceneManager.LoadScene(EvolutionSceneToLoad);
    }

    private EvolutionBrConfig ReadControlls()
    {
        _loaded.MatchConfig = MatchConfig.ReadFromControls();
        _loaded.MutationConfig = MutationConfig.ReadFromControls();

        _loaded.RunName = RunName.text;
        _loaded.MinMatchesPerIndividual = int.Parse(MinMatchesPerIndividual.text);
        _loaded.WinnersFromEachGeneration = int.Parse(WinnersFromEachGeneration.text);
        _loaded.GenerationNumber = _generationNumber;
        _loaded.NumberOfCombatants = int.Parse(NumberOfTeams.text);
        _loaded.InSphereRandomisationRadius = float.Parse(InSphereRandomisationRadius.text);
        _loaded.OnSphereRandomisationRadius = float.Parse(OnSphereRandomisationRadius.text);

        return _loaded;
    }

    private void LoadConfig()
    {
        _hasLoadedExisting = ArgumentStore.IdToLoad.HasValue;
        _loadedId = ArgumentStore.IdToLoad ?? -1;
        _loaded = _hasLoadedExisting ? _handler.ReadConfig(_loadedId) : new EvolutionBrConfig();
        
        RunName.text = _loaded.RunName;
        MinMatchesPerIndividual.text = _loaded.MinMatchesPerIndividual.ToString();
        WinnersFromEachGeneration.text = _loaded.WinnersFromEachGeneration.ToString();
        NumberOfTeams.text = _loaded.NumberOfCombatants.ToString();

        InSphereRandomisationRadius.text = _loaded.InSphereRandomisationRadius.ToString();
        OnSphereRandomisationRadius.text = _loaded.OnSphereRandomisationRadius.ToString();

        _generationNumber = _loaded.GenerationNumber;
        
        MatchConfig.LoadConfig(_loaded.MatchConfig, _hasLoadedExisting);
        MutationConfig.LoadConfig(_loaded.MutationConfig, _hasLoadedExisting);
    }
}
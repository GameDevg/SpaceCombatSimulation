﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Assets.src.Evolution;
using Assets.Src.Database;
using System;
using Assets.Src.Evolution;

public class EvolutionBRDatabaseHandlerSaveTests
{
    private string _dbPathStart = "/../tmp/TestDB/";
    private string _dbPathExtension = ".s3db";
    private string _dbPath;
    private string _createCommandPath = "/../Test/TestDB/CreateTestDB.sql";
    EvolutionBrDatabaseHandler _handler;
    DatabaseInitialiser _initialiser;

    [SetUp]
    public void Setup()
    {
        _dbPath = _dbPathStart + Guid.NewGuid().ToString() + _dbPathExtension;

        _initialiser = new DatabaseInitialiser
        {
            DatabasePath = _dbPath
        };

        _handler = new EvolutionBrDatabaseHandler(_dbPath, _createCommandPath);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            _initialiser.DropDatabase();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to tear down database: " + e.Message);
        }
    }

    #region top level
    [Test]
    public void SetCurrentGeneration_UpdatesCurrentGeneration()
    {
        var config = _handler.ReadConfig(3);
        Assert.AreEqual(3, config.DatabaseId);

        _handler.SetCurrentGenerationNumber(3, 5);
        
        var config2 = _handler.ReadConfig(3);
        Assert.AreEqual(5, config2.GenerationNumber);  //has been read back out

        //repeat with a different number, to be sure it wasn't just 5 to begin with.
        _handler.SetCurrentGenerationNumber(3, 7);
        
        var config3 = _handler.ReadConfig(3);
        Assert.AreEqual(7, config3.GenerationNumber);  //has been read back out
    }

    [Test]
    public void SaveConfig_savesWholeThingAndReturnsId()
    {
        var config = new EvolutionBrConfig
        {
            RunName = "SaveConfigTest",
            NumberOfCombatants = 3,
            GenerationNumber = 42,
            MinMatchesPerIndividual = 6,
            WinnersFromEachGeneration = 7,
            InSphereRandomisationRadius = 43,
            OnSphereRandomisationRadius = 44,
            MatchConfig = new MatchConfig(),
            MutationConfig = new MutationConfig
            {
                DefaultGenome = "SaveConfigTest_DefaultGenome"
            }
        };

        config.DatabaseId = -13; //set id to something really obvious to show if it hasn't been set correctly.

        int result = _handler.SaveNewConfig(config);

        var expectedId = 4;

        Assert.AreEqual(expectedId, result);

        var retrieved = _handler.ReadConfig(expectedId);

        Assert.AreEqual(expectedId, retrieved.DatabaseId);
        Assert.AreEqual("SaveConfigTest", retrieved.RunName);
        Assert.AreEqual(3, retrieved.NumberOfCombatants);
        Assert.AreEqual(43, retrieved.InSphereRandomisationRadius);
        Assert.AreEqual(44, retrieved.OnSphereRandomisationRadius);
    }

    [Test]
    public void UpdateTest()
    {
        var config = _handler.ReadConfig(2);

        config.RunName = "Altered";
        config.MatchConfig.InitialRange++;
        config.MutationConfig.GenomeLength++;
        config.NumberOfCombatants++;
        config.InSphereRandomisationRadius++;
        config.OnSphereRandomisationRadius++;

        _handler.UpdateExistingConfig(config);

        var updated = _handler.ReadConfig(2);

        Assert.AreEqual(config.RunName, updated.RunName);
        Assert.AreEqual("Altered", updated.RunName);
        Assert.AreEqual(config.MatchConfig.InitialRange, updated.MatchConfig.InitialRange);
        Assert.AreEqual(config.MutationConfig.GenomeLength, updated.MutationConfig.GenomeLength);
        Assert.AreEqual(config.NumberOfCombatants, updated.NumberOfCombatants);
        Assert.AreEqual(config.InSphereRandomisationRadius, updated.InSphereRandomisationRadius);
        Assert.AreEqual(config.OnSphereRandomisationRadius, updated.OnSphereRandomisationRadius);
    }
    #endregion
}
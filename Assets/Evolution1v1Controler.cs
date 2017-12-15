﻿using Assets.src.Evolution;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using Assets.Src.ObjectManagement;
using Assets.Src.Database;

public class Evolution1v1Controler : MonoBehaviour
{
    public int DatabaseId;

    public string RunName;

    public EvolutionShipConfig ShipConfig;
    public EvolutionMutationController MutationControl;
    public EvolutionMatchController MatchControl;
    
    private Dictionary<string, string> _currentGenomes;

    /// <summary>
    /// The generation is over when every individual has had at least this many matches.
    /// </summary>
    public int MinMatchesPerIndividual = 3;

    /// <summary>
    /// The number of individuals to keep for the next generation
    /// </summary>
    public int WinnersFromEachGeneration = 3;
    
    public int MaxShootAngle = 180;
    public int MaxTorqueMultiplier = 2000;
    public int MaxLocationAimWeighting = 10;
    public int MaxSlowdownWeighting = 60;
    public int MaxLocationTollerance = 1000;
    public int MaxVelociyTollerance = 200;
    public int MaxAngularDragForTorquers = 1;

    public int GenerationNumber;
    private Generation1v1 _currentGeneration;

    public float SuddenDeathDamage = 10;
    /// <summary>
    /// Time for repeating the sudden death damage.
    /// Also used as the minimum score for winning a match.
    /// </summary>
    public float SuddenDeathReloadTime = 200;

    Evolution1v1DatabaseHandler _dbHandler;

    // Use this for initialization
    void Start()
    {
        _dbHandler = new Evolution1v1DatabaseHandler(this);

        _dbHandler.ReadConfig(DatabaseId);

        ReadInGeneration();

        SpawnShips();
    }

    // Update is called once per frame
    void Update()
    {
        var winningGenome = DetectVictorsGenome();
        if (winningGenome == null && !MatchControl.IsOutOfTime())
        {
            return;
        }
        else if (MatchControl.IsOutOfTime()/* && _previousWinner == null*/)
        {
            //Debug.Log("Match Timeout!");
            ActivateSuddenDeath();
        }

        if (winningGenome != null)
        {
            Debug.Log("\"" + winningGenome + "\" Wins!");
            var a = _currentGenomes.Values.First();
            var b = _currentGenomes.Values.Skip(1).First();

            var winScore = Math.Max(MatchControl.RemainingTime(), SuddenDeathReloadTime);

            var losScore = -SuddenDeathReloadTime;
            var drawScore = -SuddenDeathReloadTime/2;

            _currentGeneration.RecordMatch(a, b, winningGenome, winScore, losScore, drawScore);

            _dbHandler.UpdateGeneration(_currentGeneration, DatabaseId, GenerationNumber);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ActivateSuddenDeath()
    {
        //Debug.Log("Sudden Death!");
        var ships = ListShips();
        foreach (var ship in ships)
        {
            ship.transform.SendMessage("ApplyDamage", SuddenDeathDamage, SendMessageOptions.DontRequireReceiver);
        }
        MatchControl.MatchTimeout = SuddenDeathReloadTime;
        MatchControl.MatchRunTime = 0;
    }
    
    private void SpawnShips()
    {
        var genomes = PickTwoGenomesFromHistory();

        Debug.Log("\"" + string.Join("\" vs \"", genomes.ToArray()) + "\"");

        _currentGenomes = new Dictionary<string, string>();
        var i = 0;
        foreach (var g in genomes)
        {
            ShipConfig.SpawnShip(g, i);
            _currentGenomes[ShipConfig.GetTag(i)] = g;

            i++;
        }
    }
    
    public string _previousWinner;

    /// <summary>
    /// Returns the genome of the victor.
    /// Or null if there's no victor yet.
    /// Or empty string if everyone's dead.
    /// </summary>
    /// <returns></returns>
    private string DetectVictorsGenome()
    {
        if (MatchControl.ShouldPollForWinners())
        {
            string currentWinner = null;

            var tags = ListShips()
                .Select(s => s.tag)
                .Distinct();
            //Debug.Log(ships.Count() + " ships exist");

            if (tags.Count() == 1)
            {
                var winningTag = tags.First();

                //Debug.Log(StringifyGenomes() + " winning tag: " + winningTag);
                currentWinner = _currentGenomes[winningTag];
            }
            if (tags.Count() == 0)
            {
                Debug.Log("Everyone's dead!");
                currentWinner = string.Empty;
            }

            var actualWinner = currentWinner == _previousWinner ? currentWinner : null;
            _previousWinner = currentWinner;
            //if there's been the same winner for two consectutive periods return that, otherise null.
            return actualWinner;
        }
        return null;
    }

    private IEnumerable<Transform> ListShips()
    {
        return GameObject.FindGameObjectsWithTag(ShipConfig.SpaceShipTag)
                .Where(s =>
                    s.transform.parent != null &&
                    s.transform.parent.GetComponent("Rigidbody") != null
                ).Select(s => s.transform.parent);
    }
        
    private string[] PickTwoGenomesFromHistory()
    {
        var g1 = _currentGeneration.PickCompetitor();
        var g2 = _currentGeneration.PickCompetitor(g1);
        return new string[] { g1, g2 };
    }
    
    private void ReadInGeneration()
    {
        _currentGeneration = _dbHandler.ReadCurrentGeneration();

        if (_currentGeneration == null || _currentGeneration.CountIndividuals() < 2)
        {
            //The current generation does not exist - create a new random generation.
            CreateNewGeneration(null);
        }
        else if (_currentGeneration.MinimumMatchesPlayed >= MinMatchesPerIndividual)
        {
            //the current generation is finished - create a new generation
            var winners = _currentGeneration.PickWinners(WinnersFromEachGeneration);

            GenerationNumber++;

            CreateNewGeneration(winners);
        }
        //Debug.Log("_currentGeneration: " + _currentGeneration);
    }

    /// <summary>
    /// Creates and saves a new generation in the daabese.
    /// If winners are provided, the new generation will be mutatnts of those.
    /// If no winners are provided, the generation number will be reset to 0, and a new default generation will be created.
    /// The current generation is set to the generation that is created.
    /// </summary>
    /// <param name="winners"></param>
    private Generation1v1 CreateNewGeneration(IEnumerable<string> winners)
    {
        if (winners != null && winners.Any())
        {
            _currentGeneration = new Generation1v1(MutationControl.CreateGenerationOfMutants(winners.ToList()));
        }
        else
        {
            Debug.Log("Generating generation from default genomes");
            _currentGeneration = new Generation1v1(MutationControl.CreateDefaultGeneration());
            GenerationNumber = 0;   //it's always generation 0 for a default genteration.
        }

        _dbHandler.SaveNewGeneration(_currentGeneration, DatabaseId, GenerationNumber);
        _dbHandler.SetCurrentGeneration(GenerationNumber);

        return _currentGeneration;
    }
}
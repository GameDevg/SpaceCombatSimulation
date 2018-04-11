--
-- File generated with SQLiteStudio v3.1.1 on Mon Dec 11 12:56:38 2017
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: MainConfig
CREATE TABLE MainConfig (
    autoloadId INTEGER
);
INSERT INTO MainConfig (autoloadId)
VALUES (null);

-- Table: BaseEvolutionConfig
CREATE TABLE BaseEvolutionConfig (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, name VARCHAR (50) NOT NULL ON CONFLICT REPLACE DEFAULT Unnamed, currentGeneration INTEGER DEFAULT '''0''' NOT NULL, minMatchesPerIndividual INTEGER DEFAULT '''3''' NOT NULL, winnersCount INTEGER DEFAULT '''5''' NOT NULL);
INSERT INTO BaseEvolutionConfig (id, name, currentGeneration, minMatchesPerIndividual, winnersCount) 
VALUES (0, 'Run0', 1, 5, 14);
INSERT INTO BaseEvolutionConfig (id, name, currentGeneration, minMatchesPerIndividual, winnersCount) 
VALUES (1, 'Run1', 7, 3, 4);
INSERT INTO BaseEvolutionConfig (id, name, currentGeneration, minMatchesPerIndividual, winnersCount) 
VALUES (2, '1v1' , 2, 3, 5);
INSERT INTO BaseEvolutionConfig (id, name, currentGeneration, minMatchesPerIndividual, winnersCount) 
VALUES (3, '4Way', 1, 4, 6);

-- Table: DroneEvolutionConfig
CREATE TABLE DroneEvolutionConfig (id INTEGER PRIMARY KEY REFERENCES BaseEvolutionConfig (id) ON DELETE CASCADE NOT NULL, minDrones INTEGER DEFAULT '''3''' NOT NULL, droneEscalation FLOAT DEFAULT (0.05) NOT NULL, maxDrones INTEGER DEFAULT '''50''' NOT NULL, killScoreMultiplier FLOAT DEFAULT '''1''' NOT NULL, flatKillBonus FLOAT DEFAULT '''100''' NOT NULL, completionBonus FLOAT DEFAULT '''1''' NOT NULL, deathPenalty FLOAT DEFAULT '''70''' NOT NULL, droneList VARCHAR (3000), shipInSphereRandomRadius FLOAT NOT NULL DEFAULT (0), shipOnSphereRandomRadius FLOAT NOT NULL DEFAULT (0), dronesInSphereRandomRadius FLOAT NOT NULL DEFAULT (0), dronesOnSphereRandomRadius FLOAT NOT NULL DEFAULT (0));
INSERT INTO DroneEvolutionConfig (id, minDrones, droneEscalation, maxDrones, killScoreMultiplier, flatKillBonus, completionBonus, deathPenalty, droneList, shipInSphereRandomRadius, shipOnSphereRandomRadius, dronesInSphereRandomRadius, dronesOnSphereRandomRadius)
VALUES (0, 10, 3, 15, -4, -6, -8, -20, '0,2,1,3,1,1,3,1,5,1,1,1,6,1,1', 100, 101, 102, 103);
INSERT INTO DroneEvolutionConfig (id, minDrones, droneEscalation, maxDrones, killScoreMultiplier, flatKillBonus, completionBonus, deathPenalty, droneList, shipInSphereRandomRadius, shipOnSphereRandomRadius, dronesInSphereRandomRadius, dronesOnSphereRandomRadius)
VALUES (1, 3, 8, 100, 50, 100, 388, 70, '0,2,1,3,1,1,3,1,5,1,1,1,6,1,1', 200, 201, 2002, 203);

-- Table: BrEvolutionConfig
CREATE TABLE BrEvolutionConfig (id INTEGER PRIMARY KEY REFERENCES BaseEvolutionConfig (id) ON DELETE CASCADE NOT NULL, combatants INTEGER NOT NULL DEFAULT (2), inSphereRandomisationRadius FLOAT NOT NULL DEFAULT (0), onSphereRandomisationRadius FLOAT NOT NULL DEFAULT (0));
INSERT INTO BrEvolutionConfig (id, combatants, inSphereRandomisationRadius, onSphereRandomisationRadius) VALUES (2, 2, 106, 107);
INSERT INTO BrEvolutionConfig (id, combatants, inSphereRandomisationRadius, onSphereRandomisationRadius) VALUES (3, 4, 207, 208);

-- Table: MatchConfig
CREATE TABLE MatchConfig (id INTEGER PRIMARY KEY REFERENCES BaseEvolutionConfig (id) ON DELETE CASCADE NOT NULL, matchTimeout FLOAT DEFAULT '300' NOT NULL, winnerPollPeriod FLOAT DEFAULT '2' NOT NULL, initialRange FLOAT DEFAULT '''6000''' NOT NULL, initialSpeed FLOAT DEFAULT '''0''' NOT NULL, randomInitialSpeed FLOAT DEFAULT '''0''' NOT NULL, competitorsPerTeam INTEGER DEFAULT '''1''' NOT NULL, stepForwardProportion FLOAT DEFAULT '''0.5''' NOT NULL, randomiseRotation BOOLEAN DEFAULT 'TRUE' NOT NULL, allowedModules STRING DEFAULT null, budget INTEGER DEFAULT 1000);
INSERT INTO MatchConfig (id, matchTimeout, winnerPollPeriod, initialRange, initialSpeed, randomInitialSpeed, competitorsPerTeam, stepForwardProportion, randomiseRotation, allowedModules, budget) 
VALUES (0, 25, 2, 6005, 5, 5,6,0.5, 1, '1,2,4,5', 12345);
INSERT INTO MatchConfig (id, matchTimeout, winnerPollPeriod, initialRange, initialSpeed, randomInitialSpeed, competitorsPerTeam, stepForwardProportion, randomiseRotation, allowedModules, budget) 
VALUES (1, 2, 1, 6001, 1,1,2, 0.1, 1, '1,2,4,5', null);
INSERT INTO MatchConfig (id, matchTimeout, winnerPollPeriod, initialRange, initialSpeed, randomInitialSpeed, competitorsPerTeam, stepForwardProportion, randomiseRotation, allowedModules, budget) 
VALUES (2, 26, 3, 6002,2,2,3, 0.2, 0, '1,2,4,5', 12345);
INSERT INTO MatchConfig (id, matchTimeout, winnerPollPeriod, initialRange, initialSpeed, randomInitialSpeed, competitorsPerTeam, stepForwardProportion, randomiseRotation, allowedModules, budget) 
VALUES (3, 27, 4, 6003,3,3,4, 0.3, 0, '1,2,4,5', 12345);

-- Table: MutationConfig
CREATE TABLE MutationConfig (id INTEGER PRIMARY KEY REFERENCES BaseEvolutionConfig (id) ON DELETE CASCADE NOT NULL, mutations BIGINT NOT NULL, maxMutationLength INTEGER DEFAULT '5' NOT NULL, genomeLength INTEGER DEFAULT '100' NOT NULL, generationSize INTEGER DEFAULT '20' NOT NULL, randomDefault BOOLEAN DEFAULT 'FALSE' NOT NULL, defaultGenome VARCHAR (1000));
INSERT INTO MutationConfig (id, mutations, maxMutationLength, genomeLength, generationSize, randomDefault, defaultGenome) VALUES (0, 7, 4, 91, 27, 1, 'abc');
INSERT INTO MutationConfig (id, mutations, maxMutationLength, genomeLength, generationSize, randomDefault, defaultGenome) VALUES (1, 3, 5, 100, 17, 0, '');
INSERT INTO MutationConfig (id, mutations, maxMutationLength, genomeLength, generationSize, randomDefault, defaultGenome) VALUES (2, 17, 14, 191, 127, 11, 'abc1');
INSERT INTO MutationConfig (id, mutations, maxMutationLength, genomeLength, generationSize, randomDefault, defaultGenome) VALUES (3, 27, 24, 291, 227, 21, '2abc');

-- Table: BaseIndividual
CREATE TABLE BaseIndividual (runType VARCHAR (1000) NOT NULL, runConfigId INTEGER NOT NULL, generation INTEGER NOT NULL, genome VARCHAR (1000) NOT NULL, score FLOAT NOT NULL, cost FLOAT, modules INTEGER, r FLOAT NOT NULL, g FLOAT NOT NULL, b FLOAT NOT NULL, species VARCHAR (1000), speciesVerbose VARCHAR (1000), subspecies VARCHAR (1000), subspeciesVerbose VARCHAR (1000), PRIMARY KEY (runConfigId, generation, genome));
INSERT INTO BaseIndividual (runType, runConfigId, generation, genome, score, cost, modules, r,g,b, species, speciesVerbose, subspecies, subspeciesVerbose) VALUES ('drone', 0, 0, '123', 42, 5646, 6, 1,2,3, 'species42', 'speciesV42', 'subspecies42', 'subspeciesV42');
INSERT INTO BaseIndividual (runType, runConfigId, generation, genome, score, cost, modules, r,g,b, species, speciesVerbose, subspecies, subspeciesVerbose) VALUES ('drone', 0, 0, '148', 65, 466, 8, 0.5,0.5,0.5, 'species', 'speciesVerbose', 'subspecies', 'subspeciesVerbose');
INSERT INTO BaseIndividual (runType, runConfigId, generation, genome, score, cost, modules, r,g,b, species, speciesVerbose, subspecies, subspeciesVerbose) VALUES ('drone', 0, 1, '1232', 42, 5646, 6, 1,2,3, 'species42', 'speciesV42', 'subspecies42', 'subspeciesV42');
INSERT INTO BaseIndividual (runType, runConfigId, generation, genome, score, cost, modules, r,g,b, species, speciesVerbose, subspecies, subspeciesVerbose) VALUES ('drone', 0, 1, '1482', 65, 466, 8, 0.5,0.5,0.5, 'species', 'speciesVerbose', 'subspecies', 'subspeciesVerbose');
INSERT INTO BaseIndividual (runType, runConfigId, generation, genome, score, cost, modules, r,g,b, species, speciesVerbose, subspecies, subspeciesVerbose) VALUES ('1v1',   2, 0, '123', 42, 123, 6, 1,2,3, 'species42', 'speciesV42', 'subspecies42', 'subspeciesV42');
INSERT INTO BaseIndividual (runType, runConfigId, generation, genome, score, cost, modules, r,g,b, species, speciesVerbose, subspecies, subspeciesVerbose) VALUES ('1v1',   2, 0, '148', 65, 466, 8, 0.5,0.5,0.5, 'species', 'speciesVerbose', 'subspecies', 'subspeciesVerbose');

-- Table: DroneIndividual
CREATE TABLE DroneIndividual (runConfigId INTEGER NOT NULL, generation INTEGER NOT NULL, genome VARCHAR (1000) NOT NULL, matchesPlayed INTEGER, matchesSurvived INTEGER, completeKills INTEGER, totalKills INTEGER, matchScores VARCHAR (500), PRIMARY KEY (runConfigId, generation, genome));
INSERT INTO DroneIndividual (runConfigId, generation, genome, matchesPlayed, matchesSurvived, completeKills, totalKills, matchScores) VALUES (0, 0, '123', 3, 1, 2, 5, '123,321');
INSERT INTO DroneIndividual (runConfigId, generation, genome, matchesPlayed, matchesSurvived, completeKills, totalKills, matchScores) VALUES (0, 0, '148', 2, 0, 1, 1, '3');
INSERT INTO DroneIndividual (runConfigId, generation, genome, matchesPlayed, matchesSurvived, completeKills, totalKills, matchScores) VALUES (0, 1, '1232', 3, 1, 2, 5, '123,321');
INSERT INTO DroneIndividual (runConfigId, generation, genome, matchesPlayed, matchesSurvived, completeKills, totalKills, matchScores) VALUES (0, 1, '1482', 2, 0, 1, 1, '3');

-- Table: BrIndividual
CREATE TABLE BrIndividual (runConfigId INTEGER NOT NULL, generation INTEGER NOT NULL, genome VARCHAR (1000) NOT NULL, wins INTEGER NOT NULL, draws INTEGER NOT NULL, loses INTEGER NOT NULL, previousCombatants VARCHAR (500), PRIMARY KEY (runConfigId, generation, genome));
INSERT INTO BrIndividual (runConfigId, generation, genome, wins, draws, loses, previousCombatants) VALUES (2, 0, '123', 3, 1, 0, '123,321');
INSERT INTO BrIndividual (runConfigId, generation, genome, wins, draws, loses, previousCombatants) VALUES (2, 0, '148', 2, 0, 1, '3');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

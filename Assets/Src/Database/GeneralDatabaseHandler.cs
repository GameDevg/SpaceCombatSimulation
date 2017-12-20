﻿using Assets.src.Evolution;
using Assets.Src.Evolution;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Src.Database
{
    public abstract class GeneralDatabaseHandler
    {
        private const string DEFAULT_COMMAND_PATH = "/Database/CreateBlankDatabase.sql";
        private const string DEFAULT_DB_PATH = "/tmp/SpaceCombatSimulationDB.s3db";
        protected string _connectionString
        {
            get
            {
                var connection = "URI=file:" + Application.dataPath + _databasePath;
                //Debug.Log("connection string: " + connection);
                return connection;
            }
        }
        protected string _databasePath; //Path to database.

        public GeneralDatabaseHandler(string databasePath = DEFAULT_DB_PATH, string dbCreationCommandPath = DEFAULT_COMMAND_PATH)
        {
            _databasePath = databasePath;

            if (!string.IsNullOrEmpty(dbCreationCommandPath))
            {
                var initialiser = new DatabaseInitialiser
                {
                    DatabasePath = _databasePath
                };
                initialiser.EnsureDatabaseExists(dbCreationCommandPath);
            }
        }

        public abstract Dictionary<int, string> ListConfigs();

        protected Dictionary<int, string> ListConfigs(string table)
        {
            var configs = new Dictionary<int, string>();

            string sqlQuery = "SELECT id, name" + " FROM " + table + ";";

            using (var sql_con = new SqliteConnection(_connectionString))
            {
                IDbCommand dbcmd = null;
                IDataReader reader = null;
                try
                {
                    sql_con.Open(); //Open connection to the database.
                    dbcmd = sql_con.CreateCommand();
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = reader.GetInt32(reader.GetOrdinal("id"));
                        var name = reader.GetString(reader.GetOrdinal("name"));
                        configs.Add(id, name);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Caught exception: " + e + ", message: " + e.Message);
                    throw e;
                }
                finally
                {
                    Disconnect(reader, null, dbcmd, sql_con);
                }
            }
            return configs;
        }

        protected string CreateReadConfigQuery(string table, int id)
        {
            string sqlQuery = "SELECT *" +
                        " FROM " + table +
                        " LEFT JOIN MatchConfig on MatchConfig.id = " + table + ".matchConfigId" +
                        " LEFT JOIN MutationConfig on MutationConfig.id = " + table + ".mutationConfigId" +
                        " WHERE " + table + ".id = " + id + ";";
            return sqlQuery;
        }

        protected string CreateReadIndividualsQuery(string table, int runId, int generationNumber)
        {
            string sqlQuery = "SELECT *" +
                        " FROM " + table +
                        " WHERE runConfigId = " + runId + " AND generation = " + generationNumber +
                        ";";
            return sqlQuery;
        }

        protected IDataReader OpenReaderWithCommand(SqliteConnection connection, string command, out IDbCommand dbcmd)
        {
            connection.Open(); //Open connection to the database.
            dbcmd = connection.CreateCommand();
            dbcmd.CommandText = command;
            var reader = dbcmd.ExecuteReader();

            return reader;
        }

        protected MatchConfig ReadMatchConfig(IDataReader reader, int idIndex)
        {
            Debug.Log("randomiseRotation ordinal: " + reader.GetOrdinal("randomiseRotation"));  //-1
            Debug.Log("randomiseRotation value: " + reader.GetBoolean(reader.GetOrdinal("randomiseRotation")));  //-1

            var config = new MatchConfig()
            {
                Id = reader.GetInt32(idIndex),  //TODO check this
                MatchTimeout = reader.GetFloat(reader.GetOrdinal("matchTimeout")), //16
                WinnerPollPeriod = reader.GetFloat(reader.GetOrdinal("winnerPollPeriod")), //17
                InitialRange = reader.GetFloat(reader.GetOrdinal("initialRange")),
                InitialSpeed = reader.GetFloat(reader.GetOrdinal("initialSpeed")),
                RandomInitialSpeed = reader.GetFloat(reader.GetOrdinal("randomInitialSpeed")),
                CompetitorsPerTeam = reader.GetInt32(reader.GetOrdinal("competitorsPerTeam")),
                StepForwardProportion = reader.GetFloat(reader.GetOrdinal("stepForwardProportion")),
                LocationRandomisationRadiaiString = reader.GetString(reader.GetOrdinal("locationRandomisationRadiai")),
                RandomiseRotation = reader.GetBoolean(reader.GetOrdinal("randomiseRotation"))
            };
            return config;
        }

        protected MutationConfig ReadMutationConfig(IDataReader reader, int idIndex)
        {
            //Debug.Log("matchConfigId ordinal: " + reader.GetOrdinal("MatchConfig.Id"));  //-1

            var config = new MutationConfig()
            {
                Id = reader.GetInt32(idIndex),
                Mutations = reader.GetInt32(reader.GetOrdinal("mutations")),
                AllowedCharacters = reader.GetString(reader.GetOrdinal("allowedCharacters")),
                MaxMutationLength = reader.GetInt32(reader.GetOrdinal("maxMutationLength")),
                GenomeLength = reader.GetInt32(reader.GetOrdinal("genomeLength")),
                GenerationSize = reader.GetInt32(reader.GetOrdinal("generationSize")),
                UseCompletelyRandomDefaultGenome = reader.GetBoolean(reader.GetOrdinal("randomDefault")),
                DefaultGenome = reader.GetString(reader.GetOrdinal("defaultGenome"))
            };
            return config;
        }

        protected void Disconnect(IDataReader reader, SqliteTransaction transaction , IDbCommand dbcmd, SqliteConnection sql_con)
        {
            //Debug.Log("Disconnecting");
            if (reader != null)
                reader.Close();
            reader = null;

            if (transaction != null)
                transaction.Dispose();
            transaction = null;

            if (dbcmd != null)
                dbcmd.Dispose();
            dbcmd = null;

            if (sql_con != null)
                sql_con.Close();
        }

        protected void SetCurrentGenerationNumber(string table, int databaseId, int generationNumber)
        {
            using (var sql_con = new SqliteConnection(_connectionString))
            {
                sql_con.Open();
                SqliteCommand command = null;

                try
                {
                    //Debug.Log("Updating generation to " + config.GenerationNumber);
                    command = new SqliteCommand("UPDATE " + table + " SET currentGeneration = ? WHERE id = ?;", sql_con);
                    
                    command.Parameters.Add(new SqliteParameter(DbType.Int32, (object)generationNumber));
                    command.Parameters.Add(new SqliteParameter(DbType.Int32, (object)databaseId));

                    command.ExecuteNonQuery();
                }
                finally
                {
                    Disconnect(null, null, command, sql_con);
                }
            }
        }
    }
}

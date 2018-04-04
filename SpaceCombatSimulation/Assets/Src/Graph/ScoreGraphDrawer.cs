﻿using System.Linq;
using UnityEngine;

namespace Assets.Src.Graph
{
    public class ScoreGraphDrawer : BaseGraphDrawer
    {
        internal override void PrepareGraph()
        {
            var generations = Enumerable.Range(0, EvolutionControler.GenerationNumber)
                .ToDictionary(i => i, i => EvolutionControler.DbHandler.ReadBaseGeneration(EvolutionControler.DatabaseId, i));

            var minScore = new GraphLine(PointTexture) { Colour = Color.red, Name = "Min Score" };
            var avgScore = new GraphLine(PointTexture) { Colour = Color.magenta, Name = "Average Score" };
            var maxScore = new GraphLine(PointTexture) { Colour = Color.green, Name = "Max Score" };

            foreach (var generation in generations)
            {
                minScore.Add(generation.Key, generation.Value.MinScore);
                avgScore.Add(generation.Key, generation.Value.AvgScore);
                maxScore.Add(generation.Key, generation.Value.MaxScore);
            }

            _graph = new LineGraph(GraphRect, BorderTexture,/*minScore,*/ avgScore, maxScore);
        }
    }
}
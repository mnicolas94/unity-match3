using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Core;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameDataExtraction.DataExtractors;
using Match3.Core.Matches.Patterns;
using UnityEditor;
using UnityEngine;

namespace Match3.Editor
{
    public static class SimulationMenuItems
    {
        [MenuItem("Assets/Facticus/Match3/Simulation/Simulate games in level", false)]
        public static async void SimulateGamesInLevel()
        {
            var (level, isSelected) = LevelUtils.TryGetSelectedLevel();
            if (isSelected)
            {
                var gamesCount = EditorInputDialog.Show("Games count selection", "Enter the games count to simulate", "100");
                try
                {
                    int count = int.Parse(gamesCount);
                    var report = await GamesSimulation.SimulateGamesInLevelAsync(level, count);
                    Debug.Log($"Game count: {report.GamesCount}\n" +
                              $"Games won: {report.GamesWon} | {report.WinRate * 100}%\n" +
                              $"Mean turns to beat when won: {report.MeanTurnCount()}");
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }
        
        [MenuItem("Assets/Facticus/Match3/Simulation/Compute match statistics", false)]
        public static async void ComputeMatchStatistics()
        {
            var (level, isSelected) = LevelUtils.TryGetSelectedLevel();
            if (isSelected)
            {
                var patterns = new List<MatchPatternRecognizerBase>
                {
                    MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                    {
                        ScriptableObject.CreateInstance<MatchPatternCross>(),
                        MatchPatternTokenCount.GetPatternRecognizer(9)
                    }, "Cross 9"),
                    MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                    {
                        ScriptableObject.CreateInstance<MatchPatternCross>(),
                        MatchPatternTokenCount.GetPatternRecognizer(8)
                    }, "Cross 8"),
                    MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                    {
                        ScriptableObject.CreateInstance<MatchPatternCross>(),
                        MatchPatternTokenCount.GetPatternRecognizer(7)
                    }, "Cross 7"),
                    MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                    {
                        ScriptableObject.CreateInstance<MatchPatternCross>(),
                        MatchPatternTokenCount.GetPatternRecognizer(6)
                    }, "Cross 6"),
                    MatchPatternComposite.GetComposite(new List<MatchPatternRecognizerBase>
                    {
                        ScriptableObject.CreateInstance<MatchPatternCross>(),
                        MatchPatternTokenCount.GetPatternRecognizer(5)
                    }, "Cross 5"),
                    MatchPatternTokenCount.GetPatternRecognizer(9, "Line 9"),
                    MatchPatternTokenCount.GetPatternRecognizer(8, "Line 8"),
                    MatchPatternTokenCount.GetPatternRecognizer(7, "Line 7"),
                    MatchPatternTokenCount.GetPatternRecognizer(6, "Line 6"),
                    MatchPatternTokenCount.GetPatternRecognizer(5, "Line 5"),
                    MatchPatternTokenCount.GetPatternRecognizer(4, "Line 4"),
                    MatchPatternTokenCount.GetPatternRecognizer(3, "Line 3"),
                };
                var dataExtractors = new List<IDataExtractor>
                {
                    new DataExtractorMatchesPatternCount(patterns)
                };
                var report = await GamesSimulation.SimulateGamesInLevelAsync(level, 1, dataExtractors);
                var gameData = report.GamesData[0];
                var matchesData = gameData.AllTurnsData.GetData<MatchesPatternCountData>();
                
                var patternsCount = matchesData.PatternsCount;
                var patternsCountList = patternsCount.Keys.Select(pattern => (pattern, patternsCount[pattern]));
                var ordered = patternsCountList.OrderByDescending(patternCount => patternCount.Item2);
                var strings = ordered.Select(tuple =>
                {
                    var (pattern, patternCount) = tuple;
                    float percent = (float) patternCount / matchesData.TotalMatches * 100;
                    return $"{pattern.name} ({patternCount}): {percent}%";
                });

                string debugString = string.Join("\n", strings);
                Debug.Log($"Total matches: {matchesData.TotalMatches}\n{debugString}");
            }
        }
    }
}
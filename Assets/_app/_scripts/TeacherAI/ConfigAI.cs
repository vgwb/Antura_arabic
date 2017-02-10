using System;
using UnityEngine;

namespace EA4S.Teacher
{
    /// <summary>
    /// Constants used to configure the Teacher System.
    /// </summary>
    public static class ConfigAI
    {
        // Reporting
        public static bool verboseTeacher = false;
        public static bool verboseQuestionPacks = false;
        public static bool verboseDataFiltering = false;
        public static bool verboseDataSelection = false;
        public static bool verbosePlaySessionInitialisation = false;

        public static bool forceJourneyIgnore = false; // If true, the journey progression logic is turned off, so that all data is usable

        // General configuration
        public const int daysForMaximumRecentPlayMalus = 4;   // Days at which we get the maximum malus for a recent play weight

        // Minigame selection weights
        public const float minigame_playSessionWeight = 1f;
        public const float minigame_recentPlayWeight = 1f;

        // Vocabulary data selection weights
        public const float data_scoreWeight = 1f;
        public const float data_recentPlayWeight = 1f;
        public const float data_currentPlaySessionWeight = 10f;
        public const float data_minimumTotalWeight = 0.1f;

        // Difficulty selection weights
        public const float difficulty_weight_age = 0f;
        public const float difficulty_weight_performance = 1f;

        public const float startingDifficultyForNewMiniGame = 0f;

        public const int lastScoresForPerformanceWindow = 10;
        public const float scoreStarsToDifficultyContribution = 0.15f;


        private static string teacherReportString;

        public static void StartTeacherReport()
        {
            teacherReportString = "----- TEACHER REPORT " + DateTime.Now + "----";
        }

        public static void AppendToTeacherReport(string s)
        {
            teacherReportString += "\n\n" + s;
        }

        public static void PrintTeacherReport()
        {
            Debug.Log(teacherReportString);   
        }

    }

}
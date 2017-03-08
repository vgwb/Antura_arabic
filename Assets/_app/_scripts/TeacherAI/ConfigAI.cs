using System;
using System.Collections.Generic;
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
        public static bool verboseMinigameSelection = true;
        public static bool verboseDifficultySelection = true;
        public static bool verboseQuestionPacks = true;
        public static bool verboseDataFiltering = true;
        public static bool verboseDataSelection = true;
        public static bool verbosePlaySessionInitialisation = true;

        // If true, the teacher will keep retrying if it encounters a selection error, to avoid blocking the game
        // @note: this may HANG the game if an error keeps appearing, so use it only for extreme cases!
        public static bool teacherSafetyFallbackEnabled = true;    

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

        // Logging
        public const int scoreMovingAverageWindow = 5;


        private static string teacherReportString;

        public static void StartTeacherReport()
        {
            teacherReportString = "";
        }

        public static string FormatTeacherHeader(string s)
        {
            return "[Teacher] - " + s + " --------";
        }

        public static void AppendToTeacherReport(string s)
        {
            if (verboseTeacher) teacherReportString += "\n\n" + s;
        }

        public static void PrintTeacherReport()
        {
            teacherReportString = "----- TEACHER REPORT " + DateTime.Now + "----" + teacherReportString;
            if (verboseTeacher) Debug.Log(teacherReportString);
#if UNITY_EDITOR
            if (verboseTeacher) System.IO.File.WriteAllText(Application.persistentDataPath + "/teacher_report.txt", teacherReportString);
#endif
        }

        public static void ReportPacks(List<QuestionPackData> packs)
        {
            if (verboseQuestionPacks)
            {
                string packsString = FormatTeacherHeader("Generated Packs");
                for (int i = 0; i < packs.Count; i++)
                    packsString += "\n" + (i + 1) + ": " + packs[i].ToString();
                AppendToTeacherReport(packsString);
            }
        }

    }

}
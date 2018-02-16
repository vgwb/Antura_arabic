using Antura.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Antura.Core.Services.OnlineAnalytics
{
    public class AnalyticsService
    {

        /// <summary>
        /// 
        /// TODO WIP: this methos saves the gameplay summary to remote/online analytics
        /// data is passed by the LogGamePlayData class
        /// 
        /// 1 - Uuid: the unique player id
        /// 2 - app version(json app version + platform + device type (tablet/smartphone))
        /// 3 - player age(int) - player genre(string M/F)
        /// 
        /// 4 - Journey Position(string Stage.LearningBlock.PlaySession)
        /// 5 - MiniGame(string code)
        /// 
        /// - playtime(int seconds how long the gameplay)
        /// - launch type(from Journey or from Book)
        /// - end type(natural game end or forced exit)
        /// 
        /// - difficulty(float from minigame config)
        /// - number of rounds(int from minigame config)
        /// - result(int 0,1,2,3 bones)
        /// 
        /// - good answers(comma separated codes of vocabulary data)
        /// - wrong answers(comma separated codes of vocabulary data)
        /// - gameplay errors(say the lives in ColorTickle or anything not really related to Learning data)
        /// 
        /// 10 - additional(json encoded additional parameters that we don't know now or custom specific per minigame)
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// 

        private bool areUnityAnalyticsEnabled()
        {
            return AppConfig.UnityAnalyticsEnabled && !Application.isEditor && AppManager.I.AppSettings.OnlineAnalyticsEnabled;
        }

        public void TrackEvent(LogGamePlayData _data)
        {
            if (areUnityAnalyticsEnabled()) {
                var eventName = "GamePlay";
                var evetData = new Dictionary<string, object>{
                { "uuid", _data.Uuid },
                 { "app", 2 },
                 { "player", 3 }
            };
                Analytics.CustomEvent(eventName, evetData);
            }
        }

        public void TrackScene(string sceneName)
        {
            if (areUnityAnalyticsEnabled()) {
                Analytics.CustomEvent("changeScene", new Dictionary<string, object> { { "scene", sceneName } });
            }
        }

        public void TrackPlayerSession(int age, Profile.PlayerGender gender)
        {
            if (areUnityAnalyticsEnabled()) {
                Gender playerGender = (gender == Profile.PlayerGender.F ? Gender.Female : Gender.Male);
                Analytics.SetUserGender(playerGender);

                int birthYear = DateTime.Now.Year - age;
                Analytics.SetUserBirthYear(birthYear);
            }
        }

    }
}

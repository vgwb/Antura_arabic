using UnityEngine;
using System.Collections;
using System;
using ModularFramework.Core;
using ModularFramework.Components;

namespace EA4S.FastCrowd {

    public class FastCrowdVariationLauncher : OnClickButtonChangeScene {

        public FastCrowdGameplayInfo.GameVariant Variant;

        public override void OnClick() {
            FastCrowdGameplayInfo gameplayInfo = new FastCrowdGameplayInfo();
            gameplayInfo.Variant = Variant;
            GameManager.Instance.Modules.GameplayModule.GameplayStart(gameplayInfo);

            base.OnClick();
        }
    }
}

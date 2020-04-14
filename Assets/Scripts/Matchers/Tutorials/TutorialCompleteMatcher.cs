using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.State.Matchers
{
    public class TutorialCompleteMatcher : TutorialStatusStateMatcher
    {
        public TutorialCompleteMatcher(string achievementKey) : base(achievementKey, Tutorial.Status.COMPLETE)
        {
        }
    }
}
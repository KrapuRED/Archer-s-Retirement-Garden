using UnityEngine;
using MoreMountains.Feedbacks;

public class SoundFeedbackPlayerHelper : Helper
{
    [SerializeField] private MMFeedbacks feedBacks;

    public override void UseHelper()
    {
        feedBacks?.PlayFeedbacks();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    private static AchievementController _instance;
    public static AchievementController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AchievementController>();
            }
            return _instance;
        }
    }
    
    [SerializeField] private Transform _popUpTransform;
    [SerializeField] private Text _popUpText;
    [SerializeField] private float _popUpShowDuration = 3f;
    [SerializeField] private List<AchievementData> _achievementList;

    private float _popUpShowDurationCounter;

    void Update()
    {
        if (_popUpShowDurationCounter > 0)
        {
            _popUpShowDurationCounter -= Time.unscaledDeltaTime;
            _popUpTransform.localScale = Vector3.LerpUnclamped(_popUpTransform.localScale, Vector3.one, 0.5f);
        } 
        else 
        {
            _popUpTransform.localScale = Vector2.LerpUnclamped(_popUpTransform.localScale, Vector3.right, 0.5f);
        }
    }
    
    public void UnlockAchievement(AchievementType type, string value)
    {
        // Digunakan jika AchievementType hanya unlock resource 
        // AchievementData achievement = _achievementList.Find(a => a.Type == type && a.Value == value);

        // Mengubah untuk achievement mencapai x gold
        List<AchievementData> filteredAchievementList = _achievementList.FindAll(a => a.Type == type);
        AchievementData achievement;

        switch (type)
        {
            case AchievementType.UnlockResource:
                achievement = filteredAchievementList.Find(a => a.Value == value);
                break;
            case AchievementType.Gold:
                achievement = filteredAchievementList.Find(a => a.IsUnlocked == false && Double.Parse(a.Value) <= Double.Parse(value));
                break;
            default:
                return;
        }

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchievementPopUp(achievement);
        }
    }

    private void ShowAchievementPopUp(AchievementData achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector3.right;
    }
}

[System.Serializable]
public class AchievementData
{
    public string Title;
    public AchievementType Type;
    public string Value;
    public bool IsUnlocked;
}

public enum AchievementType
{
    UnlockResource, Gold
}
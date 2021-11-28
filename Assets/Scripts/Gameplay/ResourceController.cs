using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    public bool isUnlocked { get; private set; }

    private int _index;
    private int _level
    {
        set
        {
            UserDataManager.Progress.ResourcesLevels[_index] = value;
            UserDataManager.Save(true);
        }

        get
        {
            if (!UserDataManager.HasResource(_index))
            {
                return 1;

            }

            return UserDataManager.Progress.ResourcesLevels[_index];
        }
    }

    private void Start()
    {
        ResourceButton.onClick.AddListener(() =>
        {
            if (isUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    public void SetConfig(int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        ResourceDescription.text = $"{_config.Name} Lv. {_level}\n+{GetOutput().ToString("0")}";
        ResourceUnlockCost.text = $"Unlock Cost\n{GetUnlockCost()}";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";

        SetUnlocked(_config.UnlockCost == 0 || UserDataManager.HasResource(_index));
    }

    public double GetOutput()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();

        if (UserDataManager.Progress.Gold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;

        ResourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        ResourceDescription.text = $"{_config.Name} Lv. {_level}\n+{GetOutput().ToString("0")}";

        AnalyticsManager.LogUpgradeEvent(_index, _level);
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();

        if (UserDataManager.Progress.Gold < unlockCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-unlockCost);
        isUnlocked = true;

        SetUnlocked(true);
        GameManager.Instance.ShowNextResource();

        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);
        
        AnalyticsManager.LogUnlockEvent(_index);
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (unlocked)
        {
            if (!UserDataManager.HasResource(_index))
            {
                UserDataManager.Progress.ResourcesLevels.Add(_level);
                UserDataManager.Save(true);
            }
        }

        ResourceImage.color = isUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }
}

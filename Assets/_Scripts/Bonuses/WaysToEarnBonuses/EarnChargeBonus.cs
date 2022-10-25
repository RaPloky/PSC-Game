using UnityEngine;

public class EarnChargeBonus : EarnBonus
{
    [SerializeField] private int updateProgressAmount;

    private void Start()
    {
        Progress = 0;
        UpdateBonusStatus();
    }

    private void OnEnable()
    {
        EventManager.OnNonBonusEnergyAdded += UpdateAddedEnergyAmount;
    }

    private void OnDisable()
    {
        EventManager.OnNonBonusEnergyAdded -= UpdateAddedEnergyAmount;
    }

    private void UpdateAddedEnergyAmount()
    {
        if (!bonus.IsBonusEnabled)
        {
            Progress += updateProgressAmount;
            UpdateBonusStatus();
        }

        if (Progress >= goal)
            EnableBonus();
    }
}

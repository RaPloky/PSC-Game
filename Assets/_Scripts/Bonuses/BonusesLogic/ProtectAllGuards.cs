using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectAllGuards : Bonus
{
    [Header("Resist bonus")]
    [SerializeField] private List<Guard> guards;
    [SerializeField] private List<GameObject> guardShields;
    private float shieldDuration;

    private void Start()
    {
        if (UpgradeManager.Instance != null)
            shieldDuration = UpgradeManager.Instance.CurrProtectionEffectValue;
    }

    public void ActivateBonus()
    {
        if (!isBonusEnabled)
            return;

        PlaySound(activationSound);
        effectDurationGO.SetActive(true);
        StartCoroutine(ProtectGuards());

        ChangeActivationButtonStatus(false);
        isBonusEnabled = false;
        ResetSoundBool();
    }

    private IEnumerator ProtectGuards()
    {
        for (int i = 0; i < guards.Count; i++)
            guards[i].IsProtectBonusActivated = true;

        StartEffectTimer(shieldDuration);
        ReactivateShields(true);
        PlayActivationParticleSystem();

        yield return new WaitForSeconds(shieldDuration);

        for (int i = 0; i < guards.Count; i++)
            guards[i].IsProtectBonusActivated = false;

        PlaySound(activationSound);
        ReactivateShields(false);
    }

    private void ReactivateShields(bool condition)
    {
        for (int i = 0; i < guardShields.Count; i++)
            guardShields[i].SetActive(condition);
    }
}

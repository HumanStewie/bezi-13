using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradeGrantingLogic : MonoBehaviour
{
    public static UpgradeGrantingLogic instance;

    [Header("UI Panels")]
    public GameObject categorySelectionPanel; 
    public GameObject upgradeSelectionPanel;
    public GameObject background;
    public Animator categoryAnimator;
    public Animator upgradeAnimator;

    [Header("Upgrade Pools")]
    public List<UpgradeData> unlockedTippingSkills = new();
    public List<UpgradeData> unlockedAttackingSkills = new();
    public List<UpgradeData> UniqueSkills = new();

    [Header("UI Buttons")]
    public UpgradeButton Upgrade1;
    public UpgradeButton Upgrade2;
    public UpgradeButton Upgrade3;

    void Awake()
    {
        instance = this;
    }

    public void StartGrantingUpgrades()
    {
        upgradeSelectionPanel.SetActive(false);
        categorySelectionPanel.SetActive(true);
        categoryAnimator.SetTrigger("Open");
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < 1)
        {
            elapsed += Time.unscaledDeltaTime; // unscaledDeltaTime allows fading while paused
            background.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
            yield return null;
        }
    }

    public void ChoosenAttack()
    {
        SkillRandomizer(unlockedAttackingSkills);
        StartCoroutine(CloseCategory());
    }

    public void ChoosenTipping()
    {
        SkillRandomizer(unlockedTippingSkills);
        StartCoroutine(CloseCategory());
    }    
    IEnumerator CloseCategory()
    {
        categoryAnimator.SetTrigger("Close");
        
        yield return new WaitForSecondsRealtime(0.30f);
        upgradeSelectionPanel.SetActive(true);
        upgradeAnimator.SetTrigger("Open");
        categorySelectionPanel.SetActive(false);
    }

    void SkillRandomizer(List<UpgradeData> datas)
    {
        List<UpgradeData> pool = new List<UpgradeData>(datas);
        List<UpgradeData> chosen = new List<UpgradeData>();

        int amountToPick = Mathf.Min(3, pool.Count);
        for (int i = 0; i < amountToPick; i++)
        {
            int rand = UnityEngine.Random.Range(0, pool.Count);
            chosen.Add(pool[rand]);
            pool.RemoveAt(rand);
        }

        if (chosen.Count > 0) {
            Upgrade1.gameObject.SetActive(true);
            Upgrade1.SetUpgrade(chosen[0]);
        } 
        else {
            Upgrade1.gameObject.SetActive(false); 
        }
        if (chosen.Count > 1) {
            Upgrade2.gameObject.SetActive(true);
            Upgrade2.SetUpgrade(chosen[1]);
        } 
        else {
            Upgrade2.gameObject.SetActive(false); 
        }
        if (chosen.Count > 2) {
            Upgrade3.gameObject.SetActive(true);
            Upgrade3.SetUpgrade(chosen[2]);
        }
        else {
            Upgrade3.gameObject.SetActive(false); }
    }

    public void OnUpgradeSelected(UpgradeData GottenUpgrade)
    {
        FindAnyObjectByType<PlayerUpgrades>().AddNewUpgrade(GottenUpgrade);

        if (unlockedAttackingSkills.Contains(GottenUpgrade))
        {
            unlockedAttackingSkills.Remove(GottenUpgrade);
        }
        else if (unlockedTippingSkills.Contains(GottenUpgrade))
        {
            unlockedTippingSkills.Remove(GottenUpgrade);
        }

        if (GottenUpgrade.nextTier != null)
        {
            if (GottenUpgrade.isAttack) unlockedAttackingSkills.Add(GottenUpgrade.nextTier);
            else unlockedTippingSkills.Add(GottenUpgrade.nextTier);
        }

        CheckForCombined();
        MusicManager.Instance.PlayUpgradeButtonSound();
        StartCoroutine(CloseUpgrade());
        StartCoroutine(FadeOut());
    }
    IEnumerator CloseUpgrade()
    {
        upgradeAnimator.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(0.3f);
        upgradeSelectionPanel.SetActive(false);
        ProceedToNextWave();
    }
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < 1)
        {
            elapsed += Time.unscaledDeltaTime; // unscaledDeltaTime allows fading while paused
            background.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, elapsed / 0.4f);
            yield return null;
        }
    }

    void CheckForCombined()
    {
        if (CheckIfExist("PillarMan2") && CheckIfExist("LegoWalk2")) RandomlyAddtoUpgradePile(UniqueSkills[0]);
        if (CheckIfExist("TheGreatReset2") && CheckIfExist("FriendsInNeed2")) RandomlyAddtoUpgradePile(UniqueSkills[1]);
        if (CheckIfExist("BlockThrower2") && CheckIfExist("RageQuit2")) RandomlyAddtoUpgradePile(UniqueSkills[2]);
        if (CheckIfExist("GoldenWind2") && CheckIfExist("HatredForHand2")) RandomlyAddtoUpgradePile(UniqueSkills[3]);
    }

    public bool CheckIfExist(string IDName)
    {
        foreach (var upgrade in FindAnyObjectByType<PlayerUpgrades>().upgrades)
        {
            if (upgrade.IDName == IDName) return true;
        }
        return false;
    }

    public void RandomlyAddtoUpgradePile(UpgradeData upgrade)
    {
        if (unlockedAttackingSkills.Contains(upgrade) || unlockedTippingSkills.Contains(upgrade)) return;

        if (Random.Range(0, 2) == 0) unlockedAttackingSkills.Add(upgrade);
        else unlockedTippingSkills.Add(upgrade);
    }

    void ProceedToNextWave()
    {
        Time.timeScale = 1f;
        GameManager.instance.ProceedNextWave();
    }
}
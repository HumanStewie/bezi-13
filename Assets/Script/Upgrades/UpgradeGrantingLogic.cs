using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeGrantingLogic : MonoBehaviour
{
    public static UpgradeGrantingLogic instance;

    [Header("UI Panels")]
    public GameObject categorySelectionPanel; 
    public GameObject upgradeSelectionPanel;

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
        Time.timeScale = 0f;
        upgradeSelectionPanel.SetActive(false);
        categorySelectionPanel.SetActive(true); 
    }

    public void ChoosenAttack()
    {
        upgradeSelectionPanel.SetActive(true);
        categorySelectionPanel.SetActive(false);
        SkillRandomizer(unlockedAttackingSkills);
    
    }

    public void ChoosenTipping()
    {
        upgradeSelectionPanel.SetActive(true);
        categorySelectionPanel.SetActive(false);
        SkillRandomizer(unlockedTippingSkills);
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

        upgradeSelectionPanel.SetActive(false);
        ProceedToNextWave();
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
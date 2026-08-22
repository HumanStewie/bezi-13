using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI Upgradename;
    public TextMeshProUGUI description;

    public UpgradeData upgradeData;

    public void SetUpgrade(UpgradeData data)
    {
        upgradeData = data;
        Upgradename.text = data.IDName;
        description.text = data.Description;
    }

    public void OnClick()
    {
        UpgradeGrantingLogic.instance.OnUpgradeSelected(this.upgradeData);
    }
}

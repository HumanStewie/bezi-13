using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string IDName;
    public bool isAttack;
    public string Description;


    public UpgradeData nextTier;
}

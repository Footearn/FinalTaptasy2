using UnityEngine;
using UnityEngine.UI;

public class BossRaidResultBoxAnimation : MonoBehaviour
{
	public BossRaidManager.BossRaidChestType currentChestType;

	public Text countText;

	public void crashEvent()
	{
		Singleton<AudioManager>.instance.playEffectSound("monster_blowup");
		switch (currentChestType)
		{
		case BossRaidManager.BossRaidChestType.Bronze:
			countText.text = Singleton<BossRaidManager>.instance.collectedBronzeChestList.Count.ToString();
			break;
		case BossRaidManager.BossRaidChestType.Gold:
			countText.text = Singleton<BossRaidManager>.instance.collectedGoldChestList.Count.ToString();
			break;
		case BossRaidManager.BossRaidChestType.Dia:
			countText.text = Singleton<BossRaidManager>.instance.collectedDiaChestList.Count.ToString();
			break;
		}
	}
}

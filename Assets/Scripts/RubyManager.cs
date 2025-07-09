using UnityEngine;

public class RubyManager : Singleton<RubyManager>
{
	public static float rubyDissapearTimer = 1f;

	public float rubyCatchRange = 5f;

	public Transform rubyUITransform;

	public ChangeNumberAnimate[] rubyValueAnimateTexts;

	private void Start()
	{
		for (int i = 0; i < rubyValueAnimateTexts.Length; i++)
		{
			rubyValueAnimateTexts[i].CurrentPrintType = ChangeNumberAnimate.PrintType.Number;
		}
		displayRuby(false, false);
	}

	public void startGame()
	{
		displayRuby(false, false);
	}

	public void spawnRuby(Vector3 spawnPosition, long value)
	{
		Singleton<DropItemManager>.instance.spawnDropItem(DropItemManager.DropItemType.Ruby, spawnPosition, value);
	}

	public void displayRuby(bool isWithResorcesEffect, bool isIncrease)
	{
		for (int i = 0; i < rubyValueAnimateTexts.Length; i++)
		{
			if (isIncrease)
			{
				if (isWithResorcesEffect)
				{
					rubyValueAnimateTexts[i].SetValue(Singleton<DataManager>.instance.currentGameData._ruby, 0.8f, 0.8f);
				}
				else
				{
					rubyValueAnimateTexts[i].SetValue(Singleton<DataManager>.instance.currentGameData._ruby, 1f);
				}
			}
			else
			{
				rubyValueAnimateTexts[i].SetText(Singleton<DataManager>.instance.currentGameData._ruby);
			}
		}
	}

	public void increaseRuby(double value, bool isWithResourceEffect = false)
	{
		if (Singleton<DataManager>.instance.currentGameData._ruby == (long)Singleton<DataManager>.instance.currentGameData.obscuredRuby)
		{
			Singleton<DataManager>.instance.currentGameData.obscuredRuby = (long)Util.Clamp((double)(long)Singleton<DataManager>.instance.currentGameData.obscuredRuby + value, 0.0, 9.2233720368547758E+18);
			Singleton<DataManager>.instance.currentGameData.ruby = Singleton<DataManager>.instance.currentGameData.obscuredRuby;
			Singleton<DataManager>.instance.currentGameData.rubyRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.rubyRecord) + (long)value);
			displayRuby(isWithResourceEffect, true);
			if (GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				UIWindowManageTreasure.instance.refreshSlots();
				UIWindowOutgame.instance.refreshTreasureIndicator();
			}
			else if (UIWindowElopeMode.instance.isOpen)
			{
				UIWindowElopeMode.instance.elopePrincessScrollRect.refreshAll();
			}
		}
		else
		{
			Singleton<DataManager>.instance.currentGameData.ruby = Singleton<DataManager>.instance.currentGameData.obscuredRuby;
		}
	}

	public void decreaseRuby(double value)
	{
		if (Singleton<DataManager>.instance.currentGameData._ruby == (long)Singleton<DataManager>.instance.currentGameData.obscuredRuby)
		{
			Singleton<DataManager>.instance.currentGameData.obscuredRuby = (long)Util.Clamp((double)(long)Singleton<DataManager>.instance.currentGameData.obscuredRuby - value, 0.0, 9.2233720368547758E+18);
			Singleton<DataManager>.instance.currentGameData.ruby = Singleton<DataManager>.instance.currentGameData.obscuredRuby;
			Singleton<DataManager>.instance.currentGameData.rubyRecord = Base36.Encode(Base36.Decode(Singleton<DataManager>.instance.currentGameData.rubyRecord) - (long)value);
			displayRuby(true, true);
			if (GameManager.currentGameState == GameManager.GameState.OutGame)
			{
				UIWindowManageTreasure.instance.refreshSlots();
				UIWindowOutgame.instance.refreshTreasureIndicator();
			}
			else if (UIWindowElopeMode.instance.isOpen)
			{
				UIWindowElopeMode.instance.elopePrincessScrollRect.refreshAll();
			}
		}
		else
		{
			Singleton<DataManager>.instance.currentGameData.ruby = Singleton<DataManager>.instance.currentGameData.obscuredRuby;
		}
	}
}

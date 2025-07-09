using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowPVPResult : UIWindow
{
	public static UIWindowPVPResult instance;

	public GameObject surBurstEffectObject;

	public Image titleImage;

	public CharacterUIObject currentCharacterObject;

	public CanvasGroup cachedCanvasGroup;

	public Text myNameText;

	public Text myTierText;

	public Image myTierImage;

	public Text myMMRText;

	public GameObject winObject;

	private bool m_isWin;

	public override void Awake()
	{
		instance = this;
		base.Awake();
	}

	public void openPVPResult(bool isWin)
	{
		m_isWin = isWin;
		surBurstEffectObject.SetActive(m_isWin);
		winObject.SetActive(m_isWin);
		Action resultAction = delegate
		{
			Singleton<AudioManager>.instance.stopBackgroundSound();
			Singleton<AudioManager>.instance.playEffectSound((!m_isWin) ? "result_fail" : "result_clear");
			PVPManager.PvPData myPVPData = Singleton<PVPManager>.instance.myPVPData;
			if (myPVPData == null || myPVPData.player == null || Singleton<PVPManager>.instance.currentRecordResponse == null)
			{
				myTierImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(0);
				myNameText.text = "--";
				myTierText.text = "--";
				myMMRText.text = "--";
			}
			else
			{
				int result = 0;
				int num = int.Parse(myPVPData.player.point);
				myNameText.text = myPVPData.player.nickname;
				if (Singleton<PVPManager>.instance.isPracticePVP)
				{
					result = int.Parse(myPVPData.player.point);
					myMMRText.text = string.Format(I18NManager.Get("PVP_MMR"), result.ToString());
					myTierImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(myPVPData.player.grade);
					myTierText.text = Singleton<PVPManager>.instance.getTierName(myPVPData.player.grade);
				}
				else
				{
					if (!int.TryParse(Singleton<PVPManager>.instance.currentRecordResponse.point, out result))
					{
						result = 0;
					}
					int num2 = result - num;
					myMMRText.text = string.Format(I18NManager.Get("PVP_MMR"), result.ToString()) + " (" + ((num2 < 0) ? string.Empty : "+") + num2 + ")";
					myTierText.text = Singleton<PVPManager>.instance.getTierName(Singleton<PVPManager>.instance.currentRecordResponse.grade);
					myTierImage.sprite = Singleton<PVPManager>.instance.getTierIconSprite(Singleton<PVPManager>.instance.currentRecordResponse.grade);
				}
			}
			myTierImage.SetNativeSize();
			if (m_isWin)
			{
				titleImage.sprite = Singleton<CachedManager>.instance.winSprite;
			}
			else
			{
				titleImage.sprite = Singleton<CachedManager>.instance.loseSprite;
			}
			titleImage.SetNativeSize();
			currentCharacterObject.initCharacterUIObject(Singleton<DataManager>.instance.currentGameData.equippedWarriorSkin, cachedCanvasGroup, "PopUpLayer2", 201);
			open();
			if (!m_isWin)
			{
				currentCharacterObject.characterBoneAnimation.Stop();
				currentCharacterObject.characterBoneAnimation.Play("Die");
			}
			else
			{
				currentCharacterObject.characterBoneAnimation.transform.localEulerAngles = Vector3.zero;
			}
		};
		if (Singleton<PVPManager>.instance.isPracticePVP)
		{
			resultAction();
		}
		else if (!Singleton<PVPManager>.instance.isAbuser())
		{
			Dictionary<PVPManager.FuntionParameterType, double> dictionary = new Dictionary<PVPManager.FuntionParameterType, double>();
			dictionary.Add(PVPManager.FuntionParameterType.IsWin, m_isWin ? 1 : 0);
			Singleton<PVPManager>.instance.CallAPI(PVPManager.PVPAPIType.SET_RECORD, dictionary, delegate(bool isSuccess)
			{
				if (isSuccess)
				{
					resultAction();
				}
				else
				{
					UIWindowDialog.openDescription("ERROR_FOR_TOWER_MODE", UIWindowDialog.DialogState.DelegateOKUI, delegate
					{
						Singleton<PVPManager>.instance.endPVP();
					}, string.Empty);
				}
			}, false);
		}
		else
		{
			Singleton<PVPManager>.instance.endPVP();
		}
	}

	public override bool OnBeforeClose()
	{
		surBurstEffectObject.SetActive(false);
		return base.OnBeforeClose();
	}

	public override void OnAfterClose()
	{
		cachedCanvasGroup.alpha = 0f;
		base.OnAfterClose();
	}

	public void OnClickCloseResult()
	{
		Singleton<PVPManager>.instance.endPVP();
	}
}

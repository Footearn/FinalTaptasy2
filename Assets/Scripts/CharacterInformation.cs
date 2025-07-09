using UnityEngine;
using UnityEngine.UI;

public class CharacterInformation : MonoBehaviour
{
	public CharacterObject targetCharacter;

	public Image curHPProgress;

	public Text damageText;

	public Text totalHpText;

	public Text curHpText;

	private double m_prevCurHP;

	public void setProperties()
	{
		damageText.text = targetCharacter.curDamage.ToString("f0");
		totalHpText.text = targetCharacter.maxHealth.ToString("f0");
		curHpText.text = targetCharacter.curHealth.ToString("f0");
		m_prevCurHP = targetCharacter.curHealth;
		float fillAmount = (float)(targetCharacter.curHealth / targetCharacter.maxHealth);
		curHPProgress.fillAmount = fillAmount;
	}

	public void clearProperties()
	{
		m_prevCurHP = 3.4028234663852886E+38;
	}

	private void Update()
	{
		if (m_prevCurHP != targetCharacter.curHealth)
		{
			float num = 0f;
			m_prevCurHP = targetCharacter.curHealth;
			curHpText.text = targetCharacter.curHealth.ToString("f0");
			num = (float)(targetCharacter.curHealth / targetCharacter.maxHealth);
			curHPProgress.fillAmount = num;
		}
	}
}

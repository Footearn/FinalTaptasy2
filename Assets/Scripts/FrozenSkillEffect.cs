using System.Collections;
using UnityEngine;

public class FrozenSkillEffect : ObjectBase
{
	public enum FrozenGroundType
	{
		NormalGround,
		MiniBossGround,
		BossGround
	}

	public FrozenGroundType currentFrozenGroundType;

	public GameObject[] frostEffectObjects;

	public SpriteRenderer frozenGroundSpriteRenderer;

	public SpriteGroup cachedSpriteGroup;

	public SpriteRenderer stairRenderer;

	public Transform stairTransform;

	public GameObject stairObject;

	private float m_duration;

	public void init(FrozenGroundType targetFrozenGround, float duration, MovingObject.Direction inPointToOutPointDirection, bool isGroundFloor)
	{
		currentFrozenGroundType = targetFrozenGround;
		stairObject.SetActive(!isGroundFloor);
		switch (targetFrozenGround)
		{
		case FrozenGroundType.NormalGround:
			switch (inPointToOutPointDirection)
			{
			case MovingObject.Direction.Left:
				stairTransform.localPosition = new Vector3(3.499f, 1.15f, 0f);
				stairTransform.localScale = new Vector3(1f, 1f, 1f);
				break;
			case MovingObject.Direction.Right:
				stairTransform.localPosition = new Vector3(-3.499f, 1.15f, 0f);
				stairTransform.localScale = new Vector3(-1f, 1f, 1f);
				break;
			}
			break;
		case FrozenGroundType.MiniBossGround:
			switch (inPointToOutPointDirection)
			{
			case MovingObject.Direction.Left:
				stairTransform.localPosition = new Vector3(3f, 1.93f, 0f);
				stairTransform.localScale = new Vector3(-1f, 1f, 1f);
				break;
			case MovingObject.Direction.Right:
				stairTransform.localPosition = new Vector3(-3f, 1.93f, 0f);
				stairTransform.localScale = new Vector3(1f, 1f, 1f);
				break;
			}
			break;
		case FrozenGroundType.BossGround:
			switch (inPointToOutPointDirection)
			{
			case MovingObject.Direction.Left:
				stairTransform.localPosition = new Vector3(2.9772f, 2.73f, 0f);
				stairTransform.localScale = new Vector3(1f, 1f, 1f);
				break;
			case MovingObject.Direction.Right:
				stairTransform.localPosition = new Vector3(-2.9772f, 2.73f, 0f);
				stairTransform.localScale = new Vector3(-1f, 1f, 1f);
				break;
			}
			break;
		}
		for (int i = 0; i < frostEffectObjects.Length; i++)
		{
			if (i == (int)currentFrozenGroundType)
			{
				if (!frostEffectObjects[i].activeSelf)
				{
					frostEffectObjects[i].SetActive(true);
				}
			}
			else if (frostEffectObjects[i].activeSelf)
			{
				frostEffectObjects[i].SetActive(false);
			}
		}
		m_duration = duration;
		frozenGroundSpriteRenderer.sprite = Singleton<CachedManager>.instance.frozenGroundSprites[(int)currentFrozenGroundType];
		stairRenderer.sprite = Singleton<CachedManager>.instance.frozenStairSprites[(int)currentFrozenGroundType];
		cachedSpriteGroup.setAlpha(1f);
		Singleton<AudioManager>.instance.playEffectSound("skill_frost_wall");
		StopCoroutine("frozenUpdate");
		StartCoroutine("frozenUpdate");
	}

	private IEnumerator frozenUpdate()
	{
		float timer = 0f;
		while (timer < m_duration)
		{
			if (!GameManager.isPause)
			{
				timer += Time.deltaTime * GameManager.timeScale;
			}
			yield return null;
		}
		endFrozenEffect();
	}

	private void endFrozenEffect()
	{
		StopCoroutine("frozenEndEffect");
		StartCoroutine("frozenEndEffect");
	}

	private IEnumerator frozenEndEffect()
	{
		float alpha = 1f;
		while (cachedSpriteGroup.alpha > 0f)
		{
			if (!GameManager.isPause)
			{
				alpha -= Time.deltaTime * GameManager.timeScale * 4f;
				cachedSpriteGroup.setAlpha(alpha);
			}
			yield return null;
		}
		ObjectPool.Recycle(base.name, base.cachedGameObject);
	}
}

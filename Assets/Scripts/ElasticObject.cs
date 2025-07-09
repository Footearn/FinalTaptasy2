using UnityEngine;

public class ElasticObject : ObjectBase
{
	public SpriteRenderer scaler;

	public SpriteRenderer hitObject;

	private float m_bossFix = 2f;

	private float m_elastic;

	public void Resetform()
	{
		scaler.transform.localRotation = Quaternion.Euler(Vector3.zero);
		scaler.transform.localScale = new Vector3(4f, 4f, 1f);
		hitObject.transform.localScale = Vector3.one;
	}

	public void SetTarget(MonsterObject master, CharacterObject target)
	{
		m_bossFix = ((!master.isMiniboss) ? 1 : 4);
		Vector2 vector = (Vector2)target.cachedTransform.position + new Vector2(0f, 0.5f) - (Vector2)scaler.transform.position;
		if (master.getDirectionEnum() == MovingObject.Direction.Right)
		{
			scaler.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f));
		}
		else
		{
			scaler.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(vector.y, 0f - vector.x) * 57.29578f));
		}
		m_elastic = Vector2.Distance(scaler.transform.position, (Vector2)target.cachedTransform.position + new Vector2(0f, 0.5f));
		scaler.transform.localScale = new Vector3(m_elastic * 4f * 4f / m_bossFix, 4f, 1f);
		hitObject.transform.localScale = new Vector3(1f / scaler.transform.localScale.x * 4f, 1f, 1f);
		double num = master.curDamage * (double)Random.Range(0.9f, 1.1f);
		if (master.isMiniboss || master.isBoss)
		{
			num -= num / 100.0 * Singleton<StatManager>.instance.decreaseDamageFromHitBoss;
		}
		target.decreasesHealth(num);
		target.setDamageText(num, false);
	}
}

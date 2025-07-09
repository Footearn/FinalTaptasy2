using System;
using System.Collections;
using UnityEngine;

public class MovingObject : ObjectBase
{
	public enum Direction
	{
		Left,
		Right
	}

	public int instanceID;

	public Transform cachedImageTransform;

	public bool isRecycle;

	protected float m_speed;

	private float m_gravity = 16f;

	protected Vector2 m_offset = new Vector2(0f, 0f);

	public bool fixedDirection;

	private bool m_isStopVelocity;

	private bool m_isTransformTarget;

	private bool m_isMoving;

	private bool m_isJumping;

	private Vector2 m_velocity;

	private float m_targetYPos;

	private Vector2 m_targetMovePosition;

	private Direction m_curDirection = Direction.Right;

	private Action m_arriveMoveAction;

	private Action m_arriveJumpAction;

	private Transform m_moveTargetTransform;

	private bool m_isLocalPosition;

	private bool m_isLookAtTarget;

	private bool m_isParabolic;

	private bool m_isParabolicWithRotationLikeBomb;

	private bool m_isRotatable;

	private bool m_isBomb;

	private bool m_isTranslateScreenPosition;

	private float m_distanceX;

	private float m_stackTime;

	private bool m_dontStop;

	private IEnumerator m_jumpUpdateCoroutine;

	private IEnumerator m_moveUpdateCoroutine;

	public float ySpeed = 6f;

	private float m_vecDifferenceY;

	private float m_distanceTotal;

	public void setLookAtTarget(bool value)
	{
		m_isLookAtTarget = value;
	}

	public void setParabolic(bool value)
	{
		m_isParabolic = value;
	}

	public void setParabolicWithRotationLikeBomb(bool value)
	{
		m_isParabolicWithRotationLikeBomb = value;
	}

	public void setRotatable(bool value)
	{
		m_isRotatable = value;
	}

	public void setBomb(bool value)
	{
		m_isBomb = value;
	}

	public void setOffset(Vector2 offSet)
	{
		m_offset = offSet;
	}

	public void setTranslateScreenPosition(bool value)
	{
		m_isTranslateScreenPosition = value;
	}

	public void setLocalPosition(bool value)
	{
		m_isLocalPosition = value;
	}

	public void stopMove()
	{
		if (m_moveUpdateCoroutine != null)
		{
			StopCoroutine(m_moveUpdateCoroutine);
			m_moveUpdateCoroutine = null;
		}
		m_arriveMoveAction = null;
	}

	public bool isMoving()
	{
		return m_isMoving;
	}

	public bool isJumping()
	{
		return m_isJumping;
	}

	public void stopJump()
	{
		if (m_isJumping)
		{
			if (m_jumpUpdateCoroutine != null)
			{
				StopCoroutine(m_jumpUpdateCoroutine);
				m_jumpUpdateCoroutine = null;
			}
			m_isJumping = false;
		}
		m_arriveJumpAction = null;
	}

	public void setDontStop(bool flag)
	{
		m_dontStop = flag;
	}

	public void stopAll()
	{
		stopMove();
		stopJump();
	}

	public float getSpeed()
	{
		return m_speed;
	}

	public void setSpeed(float value)
	{
		m_speed = value;
	}

	public void jump(Vector2 velocity, float targetYPos)
	{
		jump(velocity, targetYPos, null);
	}

	public void jump(Vector2 velocity, float targetYPos, Action arriveAction)
	{
		stopJump();
		m_arriveJumpAction = arriveAction;
		m_targetYPos = targetYPos;
		m_velocity = velocity;
		m_isJumping = true;
		m_jumpUpdateCoroutine = jumpUpdate();
		if (base.cachedGameObject.activeSelf && base.cachedGameObject.activeInHierarchy)
		{
			StartCoroutine(m_jumpUpdateCoroutine);
		}
	}

	private IEnumerator jumpUpdate()
	{
		bool isGravity2 = true;
		Vector3 position;
		while (true)
		{
			position = base.cachedTransform.position;
			if (isGravity2)
			{
				position += (Vector3)m_velocity * Time.deltaTime * GameManager.timeScale;
				m_velocity.y -= m_gravity * Time.deltaTime * GameManager.timeScale;
			}
			if (m_velocity.y <= -0.5f && position.y < m_targetYPos)
			{
				break;
			}
			base.cachedTransform.position = position;
			yield return null;
		}
		isGravity2 = false;
		position.y = m_targetYPos;
		base.cachedTransform.position = position;
		if (m_arriveJumpAction != null)
		{
			m_arriveJumpAction();
		}
		stopJump();
	}

	public void moveTo(Vector2 targetPosition, float targetSpeed)
	{
		moveTo(targetPosition, targetSpeed, null);
	}

	public void moveTo(Vector2 targetPosition, float targetSpeed, Action arriveAction)
	{
		m_isMoving = true;
		m_distanceX = Mathf.Abs(targetPosition.x - base.cachedTransform.position.x) + Mathf.Abs(m_offset.x);
		m_vecDifferenceY = targetPosition.y - base.cachedTransform.position.y;
		m_isTransformTarget = false;
		m_offset = Vector2.zero;
		m_moveTargetTransform = null;
		stopMove();
		m_arriveMoveAction = arriveAction;
		m_speed = targetSpeed;
		m_targetMovePosition = targetPosition;
		m_moveUpdateCoroutine = moveUpdate();
		StartCoroutine(m_moveUpdateCoroutine);
	}

	public void moveTo(Transform targetTransform, float targetSpeed)
	{
		moveTo(targetTransform, 0f, targetSpeed, null);
	}

	public void moveTo(Transform targetTransform, float xOffset, float targetSpeed)
	{
		moveTo(targetTransform, xOffset, targetSpeed, null);
	}

	public void moveTo(Transform targetTransform, float targetSpeed, Action arriveAction)
	{
		moveTo(targetTransform, 0f, targetSpeed, arriveAction);
	}

	public void moveTo(Transform targetTransform, float xOffset, float targetSpeed, Action arriveAction)
	{
		m_isMoving = true;
		m_distanceX = Mathf.Abs(targetTransform.position.x - base.cachedTransform.position.x + m_offset.x) + Mathf.Abs(m_offset.x);
		m_isTransformTarget = true;
		m_offset = new Vector2(xOffset, m_offset.y);
		m_moveTargetTransform = targetTransform;
		stopMove();
		m_arriveMoveAction = arriveAction;
		m_speed = targetSpeed;
		m_targetMovePosition = Vector2.zero;
		m_moveUpdateCoroutine = moveUpdate();
		StartCoroutine(m_moveUpdateCoroutine);
	}

	private IEnumerator moveUpdate()
	{
		float deltaSpeed2 = 0f;
		m_stackTime = 0f;
		if (cachedImageTransform != null)
		{
			cachedImageTransform.localPosition = Vector2.zero;
			cachedImageTransform.localRotation = Quaternion.Euler(Vector3.zero);
		}
		float originZPos = 0f;
		yield return null;
		m_isMoving = true;
		while (true)
		{
			if (!GameManager.isPause && !m_isJumping)
			{
				Vector2 targetPosition;
				if (m_moveTargetTransform == null)
				{
					targetPosition = m_targetMovePosition;
					if (m_isTransformTarget)
					{
						if (m_arriveMoveAction != null)
						{
							m_arriveMoveAction();
						}
						break;
					}
				}
				else
				{
					targetPosition = m_moveTargetTransform.position;
				}
				m_stackTime += Time.deltaTime * GameManager.timeScale;
				targetPosition += m_offset;
				deltaSpeed2 = m_speed * Time.deltaTime * GameManager.timeScale;
				Vector3 position = (m_isLocalPosition ? base.cachedTransform.localPosition : base.cachedTransform.position);
				originZPos = position.z;
				if (m_isTranslateScreenPosition)
				{
					targetPosition = Singleton<CachedManager>.instance.ingameCamera.ScreenToWorldPoint(targetPosition);
				}
				Vector2 dist = targetPosition - (Vector2)position;
				if (m_isParabolicWithRotationLikeBomb)
				{
					cachedImageTransform.localPosition = new Vector2(0f, calcBombParabolicOffset(m_stackTime));
					if (m_isLookAtTarget)
					{
						float calculatedValue = calcBombParabolicDirection(m_stackTime, m_curDirection);
						if (!float.IsNaN(calculatedValue))
						{
							cachedImageTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, calculatedValue * 57.29578f));
						}
						else
						{
							Vector2 distanceByImage = targetPosition - (Vector2)cachedImageTransform.position;
							if (m_curDirection == Direction.Right)
							{
								cachedImageTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(distanceByImage.y, distanceByImage.x) * 57.29578f));
							}
							else
							{
								cachedImageTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(distanceByImage.y, 0f - distanceByImage.x) * 57.29578f));
							}
						}
					}
				}
				if (m_isParabolic)
				{
					if (m_isBomb)
					{
						cachedImageTransform.localPosition = new Vector2(0f, calcBombParabolicOffset(m_stackTime));
						cachedImageTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, calcBombParabolicDirection(m_stackTime, m_curDirection) * 57.29578f));
					}
					else
					{
						cachedImageTransform.localPosition = new Vector2(0f, calcParabolicOffset(m_stackTime));
					}
				}
				if (!m_isParabolicWithRotationLikeBomb && m_isLookAtTarget)
				{
					if (cachedImageTransform == null)
					{
						if (m_curDirection == Direction.Right)
						{
							base.cachedTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(dist.y, dist.x) * 57.29578f));
						}
						else
						{
							base.cachedTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(dist.y, 0f - dist.x) * 57.29578f));
						}
					}
					else if (m_curDirection == Direction.Right)
					{
						cachedImageTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(dist.y, dist.x) * 57.29578f));
					}
					else
					{
						cachedImageTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Atan2(dist.y, 0f - dist.x) * 57.29578f));
					}
				}
				if (m_isRotatable)
				{
					cachedImageTransform.Rotate(Vector3.back, deltaSpeed2 * 200f);
				}
				if (dist.sqrMagnitude > deltaSpeed2 * deltaSpeed2)
				{
					m_isMoving = true;
					if (!fixedDirection && targetPosition.x != base.cachedTransform.position.x)
					{
						if (dist.normalized.x > 0f)
						{
							setDirection(Direction.Right);
						}
						else
						{
							setDirection(Direction.Left);
						}
					}
					Vector2 step = dist.normalized * deltaSpeed2;
					position += (Vector3)step;
					position.z = originZPos;
					if (!m_isLocalPosition)
					{
						base.cachedTransform.position = position;
					}
					else
					{
						base.cachedTransform.localPosition = position;
					}
				}
				else
				{
					Vector3 targetFixPosition = new Vector3(targetPosition.x, targetPosition.y, originZPos);
					if (!m_isLocalPosition)
					{
						base.cachedTransform.position = targetFixPosition;
					}
					else
					{
						base.cachedTransform.localPosition = targetFixPosition;
					}
					if (m_arriveMoveAction != null && m_isMoving)
					{
						m_arriveMoveAction();
					}
					m_isMoving = false;
					if (!m_dontStop)
					{
						break;
					}
				}
			}
			yield return null;
		}
		if (isRecycle)
		{
			ObjectPool.Recycle(base.name, base.cachedGameObject);
		}
	}

	public void setDirection(Direction targetDirection)
	{
		Vector2 vector = base.cachedTransform.localScale;
		m_curDirection = targetDirection;
		float num = Mathf.Abs(base.cachedTransform.localScale.x);
		if (targetDirection == Direction.Left)
		{
			num = 0f - num;
		}
		base.cachedTransform.localScale = new Vector3(num, vector.y, 1f);
	}

	public float getDirection()
	{
		return (m_curDirection != 0) ? 1 : (-1);
	}

	public Direction getDirectionEnum()
	{
		return m_curDirection;
	}

	public static Direction calculateDirection(Vector2 startPosition, Vector2 targetPosition)
	{
		Direction direction = Direction.Left;
		if ((targetPosition - startPosition).normalized.x > 0f)
		{
			return Direction.Right;
		}
		return Direction.Left;
	}

	private float calcBombParabolicDirection(float deltaTime, Direction thisDirection)
	{
		float num = 0f;
		m_distanceTotal = Mathf.Sqrt(m_distanceX * m_distanceX + m_vecDifferenceY * m_vecDifferenceY);
		float num2 = m_distanceTotal / m_speed;
		float x = Mathf.Sqrt(m_speed * m_speed - ySpeed * ySpeed);
		num = ((!(Mathf.Abs(m_vecDifferenceY) > 1f)) ? Mathf.Atan2(ySpeed * (1f - 2f * deltaTime / num2), m_speed) : Mathf.Atan2(ySpeed * (1f - 2f * deltaTime / num2), x));
		return (!float.IsNaN(num)) ? num : 0f;
	}

	private float calcParabolicOffset(float deltaTime)
	{
		float num = 0f;
		float num2 = m_distanceX / m_speed;
		num = 8f * deltaTime * (num2 - deltaTime);
		return (!float.IsNaN(num)) ? num : 0f;
	}

	private float calcBombParabolicOffset(float deltaTime)
	{
		float num = 0f;
		m_distanceTotal = Mathf.Sqrt(m_distanceX * m_distanceX + m_vecDifferenceY * m_vecDifferenceY);
		float num2 = m_distanceTotal / m_speed;
		num = ySpeed * deltaTime / num2 * (num2 - deltaTime);
		return (!float.IsNaN(num)) ? num : 0f;
	}
}

using System;
using System.Collections;
using UnityEngine;

public class NewMovingObject : ObjectBase
{
	public enum Direction
	{
		None = -100,
		Left = -1,
		Right = 1
	}

	public Direction currentDirection = Direction.None;

	public Vector3 velocity;

	public bool isMoving;

	private Action m_arriveAction;

	private float m_speed;

	private Vector2 m_moveTargetPosition;

	private Transform m_targetTransform;

	private Vector2 m_offset = Vector2.zero;

	private Coroutine m_moveUpdateCoroutine;

	private bool m_isLookAtTarget;

	private bool m_isParabolic;

	private float m_parabolicValue = 1f;

	private bool m_isRotatable;

	private bool m_isFixedDirection;

	private float m_originDistance;

	public bool isJumping;

	private Vector2 m_currentJumpVelocity;

	private float m_arriveYPos;

	private float m_gravity = 9.8f;

	private Action m_arriveActionForJump;

	private Coroutine m_jumpUpdateCoroutine;

	private Vector3? m_prevPosition;

	public void setOffset(Vector2 offset)
	{
		m_offset = offset;
	}

	public void setLookAtTarget(bool value)
	{
		m_isLookAtTarget = value;
	}

	public void setParabolic(bool value, float multiply = 1f)
	{
		m_isParabolic = value;
		m_parabolicValue = multiply;
	}

	public void setRotatable(bool value)
	{
		m_isRotatable = value;
	}

	public void setFixedDirection(bool value)
	{
		m_isFixedDirection = value;
	}

	public void stopMove()
	{
		isMoving = false;
		m_originDistance = 0f;
		if (m_moveUpdateCoroutine != null)
		{
			StopCoroutine(m_moveUpdateCoroutine);
		}
		m_arriveAction = null;
		m_moveTargetPosition = Vector2.zero;
		m_targetTransform = null;
		m_moveUpdateCoroutine = null;
	}

	public void moveTo(Vector2 targetPosition, float speed, Action arriveAction = null)
	{
		stopMove();
		isMoving = true;
		m_speed = speed;
		m_moveTargetPosition = targetPosition;
		m_arriveAction = arriveAction;
		m_targetTransform = null;
		m_moveUpdateCoroutine = StartCoroutine(moveUpdate());
	}

	public void moveTo(Transform targetTransform, float speed, Action arriveAction = null)
	{
		stopMove();
		isMoving = true;
		m_speed = speed;
		m_moveTargetPosition = Vector2.zero;
		m_targetTransform = targetTransform;
		m_arriveAction = arriveAction;
		m_moveUpdateCoroutine = StartCoroutine(moveUpdate());
	}

	private IEnumerator moveUpdate()
	{
		m_prevPosition = base.cachedTransform.position;
		float realSpeed = 0f;
		Vector3 realMoveTargetPosition = m_moveTargetPosition + m_offset;
		bool isFirstFrameOnMoveUpdate = true;
		if (m_targetTransform != null)
		{
			realMoveTargetPosition = m_targetTransform.position + (Vector3)m_offset;
		}
		Vector2 originStartPosition = base.cachedTransform.position;
		Vector3 positionForParabolic = base.cachedTransform.position;
		Vector3 position;
		while (true)
		{
			if (!GameManager.isPause)
			{
				if (m_targetTransform != null)
				{
					realMoveTargetPosition = m_targetTransform.position + (Vector3)m_offset;
				}
				m_originDistance = Vector2.Distance(originStartPosition, realMoveTargetPosition);
				position = base.cachedTransform.position;
				Vector2 dist = realMoveTargetPosition - position;
				realSpeed = m_speed * Time.deltaTime * GameManager.timeScale;
				if (!(dist.sqrMagnitude > realSpeed * realSpeed))
				{
					break;
				}
				isMoving = true;
				if (realMoveTargetPosition.x != base.cachedTransform.position.x && !m_isFixedDirection)
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
				if (!m_isParabolic)
				{
					Vector3 step2 = dist.normalized * realSpeed;
					position += step2;
				}
				else
				{
					Vector3 step = ((Vector2)(realMoveTargetPosition - positionForParabolic)).normalized * realSpeed;
					positionForParabolic += step;
					float currentDistanceForParabolic = Vector2.Distance(positionForParabolic, realMoveTargetPosition);
					float calculatedYPos = calculateParabolicYpos(currentDistanceForParabolic);
					position.x = positionForParabolic.x;
					position.y = positionForParabolic.y + calculatedYPos;
				}
				base.cachedTransform.position = position;
				calculateVelocity();
				if (m_isLookAtTarget)
				{
					if (isFirstFrameOnMoveUpdate)
					{
						isFirstFrameOnMoveUpdate = false;
						base.cachedTransform.right = (float)currentDirection * velocity.normalized;
					}
					else
					{
						base.cachedTransform.right = (float)currentDirection * Vector2.Lerp(base.cachedTransform.right, velocity.normalized, Time.deltaTime * GameManager.timeScale * 50f);
					}
				}
				else if (m_isRotatable)
				{
					Vector3 rotation = base.cachedTransform.localEulerAngles;
					rotation.z += realSpeed * (0f - (float)currentDirection) * 45f;
					base.cachedTransform.localEulerAngles = rotation;
				}
				base.cachedTransform.position = position;
			}
			yield return null;
		}
		Vector3 targetFixPosition = new Vector3(realMoveTargetPosition.x, realMoveTargetPosition.y, position.z);
		base.cachedTransform.position = targetFixPosition;
		if (m_arriveAction != null && isMoving)
		{
			m_arriveAction();
		}
		isMoving = false;
	}

	private float calculateParabolicYpos(float currentDistance)
	{
		float num = 0f;
		num = m_speed * 0.15f * currentDistance / m_originDistance * (m_originDistance - currentDistance);
		num *= m_parabolicValue;
		return (!float.IsNaN(num)) ? num : 0f;
	}

	public void stopJump()
	{
		if (m_jumpUpdateCoroutine != null)
		{
			StopCoroutine(m_jumpUpdateCoroutine);
		}
		m_jumpUpdateCoroutine = null;
		isJumping = false;
		m_arriveYPos = 0f;
		m_currentJumpVelocity = Vector2.zero;
	}

	public void jump(Vector2 velocity, float arriveYPos, Action arriveAction = null)
	{
		stopJump();
		m_arriveActionForJump = arriveAction;
		m_currentJumpVelocity = velocity;
		m_arriveYPos = arriveYPos;
		m_jumpUpdateCoroutine = StartCoroutine(jumpUpdate());
	}

	private IEnumerator jumpUpdate()
	{
		isJumping = true;
		float currentGravity = 0f;
		Vector3 position;
		while (true)
		{
			if (!GameManager.isPause)
			{
				currentGravity += m_gravity * Time.deltaTime * GameManager.timeScale;
				position = base.cachedTransform.position;
				position += (Vector3)m_currentJumpVelocity * Time.deltaTime * GameManager.timeScale;
				m_currentJumpVelocity = Vector2.Lerp(m_currentJumpVelocity, Vector2.zero, Time.deltaTime * GameManager.timeScale * 5f);
				position.y -= currentGravity * Time.deltaTime * GameManager.timeScale;
				base.cachedTransform.position = position;
				calculateVelocity();
				if (position.y <= m_arriveYPos && velocity.y <= 0f)
				{
					break;
				}
			}
			yield return null;
		}
		if (m_arriveActionForJump != null)
		{
			m_arriveActionForJump();
		}
		position.y = m_arriveYPos;
		base.cachedTransform.position = position;
		stopJump();
	}

	protected virtual void LateUpdate()
	{
		calculateVelocity(true);
	}

	private void calculateVelocity(bool isSyncPrevPosition = false)
	{
		Vector3? prevPosition = m_prevPosition;
		if (!prevPosition.HasValue)
		{
			m_prevPosition = base.cachedTransform.position;
		}
		Vector3 position = base.cachedTransform.position;
		Vector3? prevPosition2 = m_prevPosition;
		velocity = (position - prevPosition2.Value) / Time.deltaTime * GameManager.timeScale;
		if (isSyncPrevPosition)
		{
			Vector3? prevPosition3 = m_prevPosition;
			if (prevPosition3.GetValueOrDefault() != base.cachedTransform.position || !prevPosition3.HasValue)
			{
				m_prevPosition = base.cachedTransform.position;
			}
		}
	}

	public void setDirection(Direction targetDirection)
	{
		Vector2 vector = base.cachedTransform.localScale;
		currentDirection = targetDirection;
		float num = Mathf.Abs(base.cachedTransform.localScale.x);
		if (targetDirection == Direction.Left)
		{
			num = 0f - num;
		}
		if (base.cachedTransform.localScale.x != num)
		{
			base.cachedTransform.localScale = new Vector3(num, vector.y, 1f);
		}
	}

	public float getDirection()
	{
		return (currentDirection != Direction.Left) ? 1 : (-1);
	}

	public Direction getDirectionEnum()
	{
		return currentDirection;
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
}

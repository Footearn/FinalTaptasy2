public class PublicDataManager : Singleton<PublicDataManager>
{
	public enum State
	{
		None = -1,
		Idle,
		Move,
		Attack,
		Die,
		Wait,
		CastSkill,
		Stun,
		Frozen
	}
}

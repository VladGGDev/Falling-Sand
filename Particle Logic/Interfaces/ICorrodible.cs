public interface ICorrodible : IParticle
{
	public int CorrosionResistance { get; init; }
	public float CorrosionChanceMultiplier { get; init; }

	public bool CanBeCorroded(ICorrosive other)
	{
		if (CorrosionResistance > other.CorrosionPower)
			return false;
		return ParticleUtility.RandomBool(other.CorrosionChance * CorrosionChanceMultiplier);
	}
}

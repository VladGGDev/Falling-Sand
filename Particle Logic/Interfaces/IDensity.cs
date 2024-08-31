public interface IDensity : IParticle
{
	public int Density { get; init; }
	public float PassThroughChance { get; init; }

	public bool CanBePassedThrough(IDensity other)
	{
		if (other.Density <= Density)
			return false;
		return ParticleUtility.RandomBool(PassThroughChance * other.PassThroughChance);
	}
}

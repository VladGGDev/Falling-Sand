public interface IDensity : IParticle
{
	public int Density { get; init; }
	public float Viscosity { get; init; }

	public bool CanBePassedThrough(IDensity other)
	{
		if (other.Density < Density)
			return false;
		return !ParticleUtility.RandomBool(Viscosity);
	}
}

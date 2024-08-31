public interface ICorrosive : IParticle
{
	public float CorrosionChance { get; init; }
	public int CorrosionPower { get; init; }

	public bool CanCorrode(ICorrodible other)
	{
		if (other.CorrosionResistance > CorrosionPower)
			return false;
		return ParticleUtility.RandomBool(CorrosionChance * other.CorrosionMultiplier);
	}
}

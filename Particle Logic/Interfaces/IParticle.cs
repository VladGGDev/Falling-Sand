public interface IParticle
{
	public Color GetColor(ParticleGrid grid, Point position);
	public void Update(ParticleGrid grid, Point position);
}

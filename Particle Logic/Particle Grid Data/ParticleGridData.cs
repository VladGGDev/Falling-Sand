public interface IParticleGridData<T>
{
	public T Data { get; set; }
	//public T GetData();
	//public void SetData(T value);
}

public class SingleParticleGridData : IParticleGridData<(int lifetime, int maxLifetime)>
{
	public (int lifetime, int maxLifetime) Data { get; set; }
}

// I don't know about this. I will have to think more about ParticleGridData architecture

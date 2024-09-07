using static System.Runtime.InteropServices.JavaScript.JSType;

public abstract class ParticleData
{
}

public class SingleParticleData : ParticleData
{
	public int Data;
	public SingleParticleData(int data) => Data = data;
}

public class DoubleParticleData : ParticleData
{
	public int Data1;
	public int Data2;
	public DoubleParticleData(int data1, int data2)
	{
		Data1 = data1;
		Data2 = data2;
	}
}

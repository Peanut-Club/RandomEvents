namespace RandomEvents.Interfaces;

public interface IRandomEvent
{
	bool IsActive { get; set; }

	int Chance { get; set; }

	string Name { get; }

	string Description { get; }

	void Start();

	void Stop();

	void Tick();
}

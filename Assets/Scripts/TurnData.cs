public readonly struct TurnData {
	private readonly int roll;
	public int Roll => roll;

	private readonly int destination;
	public int Destination => destination;

	public TurnData(int roll, int destination) {
		this.roll = roll;
		this.destination = destination;
	}
}
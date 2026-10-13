public class MapEvent
{
	public EVENT_TYPE eventType;

	public int distance;

	public int val;

	public bool repeat;

	public MapEvent(EVENT_TYPE eventType, bool repeat, int distance, int val = -1)
	{
		this.eventType = eventType;
		this.repeat = repeat;
		this.distance = distance;
		this.val = val;
	}
}

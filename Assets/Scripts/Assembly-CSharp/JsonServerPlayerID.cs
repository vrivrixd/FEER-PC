using System;

[Serializable]
public class JsonServerPlayerID
{
	public string playerId;

	public string hash;

	public JsonServerPlayerID(string id, string hash)
	{
		playerId = id;
		this.hash = hash;
	}
}

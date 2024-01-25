using System;

public class CapturedPlayerResponse
{
    public DateTime startTime;
    public DateTime endTime;
    public string responseText;

    public CapturedPlayerResponse(DateTime _startTime)
    {
        startTime = _startTime;
        endTime = _startTime;
        responseText = string.Empty;
    }
}

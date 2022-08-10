using System.Collections.Generic;

[System.Serializable]
public class QuestList
{
    public string mType;
    public List<Quest> mQuests; 
}

[System.Serializable]
public class Quest
{
    public string mQuestId;
    public string mQuestName;
    public int mProgress;
    public bool mComplete;
}
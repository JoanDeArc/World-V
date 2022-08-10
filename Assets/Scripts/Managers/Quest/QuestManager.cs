using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        mActiveQuests = LoadQuestsFromJsonData("ActiveQuests"); 
        mCompletedQuests = LoadQuestsFromJsonData("CompletedQuests");  
    }

    private void Update()
    {
        // Testing buttons
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefs.DeleteKey("ActiveQuests");
            PlayerPrefs.DeleteKey("CompletedQuests");
            Start();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogQuests(mActiveQuests);
            LogQuests(mCompletedQuests);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddTestQuest(mCompletedQuests);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddTestQuest(mActiveQuests);
        }
    }

    private void AddTestQuest(QuestList questList)
    {
        int numberOfQuests = questList.mQuests.Count;
        Quest newQuest = new Quest();
        newQuest.mQuestId = "" + numberOfQuests;
        newQuest.mQuestName = "Quest" + numberOfQuests;
        newQuest.mProgress = 0;
        newQuest.mComplete = false;

        questList.mQuests.Add(newQuest);

        SaveQuestsToJsonData(questList.mType, questList);
        PlayerPrefs.Save();
    }

    private void LogQuests(QuestList quests)
    {
        string jsonData = "<<<< " + quests.mType + " >>>>\n\n";
        jsonData += JsonUtility.ToJson(quests, true);
        Debug.Log(jsonData);
    }

    private QuestList LoadQuestsFromJsonData(string dataId)
    {
        string jsonData = PlayerPrefs.GetString(dataId, "");

        if (jsonData == "{}" || jsonData == "")
        {
            QuestList newQuestList = new QuestList();
            newQuestList.mType = dataId;
            newQuestList.mQuests = new List<Quest>();
            return newQuestList;
        }
        return JsonUtility.FromJson<QuestList>(jsonData);
    }

    private void SaveQuestsToJsonData(string dataId, QuestList data)
    {
        if (PlayerPrefs.HasKey(dataId))
        {
            PlayerPrefs.DeleteKey(dataId);
        }

        string jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(dataId, jsonData);
    }

    QuestList mActiveQuests;
    QuestList mCompletedQuests;
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SheetsManager : MonoBehaviour
{
    static readonly string[] scopes = { Google.Apis.Sheets.v4.SheetsService.Scope.Spreadsheets };

    static readonly string applicationName = “ANYTHING”;

    static readonly string spreadsheetId = “SHEET_ID_HERE;

    static private string jsonPath = “DIRECTORY_PATH_TO_JSON_CREDENTIALS;

    static public SheetsService service;

    public LogicManagerScript logic;

    public void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManagerScript>();

        string fullPath = Application.dataPath + jsonPath;

        GoogleCredential credential;

        using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
        }

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName,
        });

        GetLeaderboard();
    }

    public IList<IList<object>> ReadEntries(string range)
    { 
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

        var response = request.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0)
        {
            return values;
        } else
        {
            return null;
        }
    }

    public void CreateEntry(List<object> data, string range)
    {
        var valueRange = new ValueRange();
        valueRange.Values = new List<IList<object>> { data };

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendResponse = appendRequest.Execute();

        //ADD ERROR HANDLING!
    }

    public void GetLeaderboard()
    {
        var rawData = ReadEntries(range: "rawLB!A:B");

        if (rawData != null && rawData.Count > 0)
        {
            Dictionary<string, int> leaderboard = new Dictionary<string, int>();

            string players = "";
            foreach (var row in rawData)
            {
                int num;
                if (int.TryParse(row[1].ToString(), out num))
                {
                    string user = row[0].ToString();
                    leaderboard.Add(user, num);
                    players = $"{players}{user},";
                }
            }

            var sortedLeaderboard = leaderboard.OrderBy(x => x.Value).Take(5);

            string lbString = "";
            foreach (var item in sortedLeaderboard)
            {
                lbString = $"{lbString}{item.Key},{item.Value.ToString()}|";
            }

            if (logic.debug)
            {
                Debug.Log(lbString);
                Debug.Log(players);
            }
            PlayerPrefs.SetString("Leaderboard", lbString);
            PlayerPrefs.SetString("PreviousPlayers", players);
        }
        else
        {
            PlayerPrefs.SetString("Leaderboard", null);
            PlayerPrefs.SetString("PreviousPlayers", null);
        }
        PlayerPrefs.Save();

        if (logic.debug)
        {
            var data = ReadEntries("rawLB!A:B");

            if (data != null && data.Count > 0)
            {
                foreach (var row in data)
                {
                    Debug.Log($"Username: {row[0]}");
                    Debug.Log($"Time: {row[1]}");
                }
            }
        }
    }

    public void AddUserToLeaderboard(string username, int time)
    {
        var toAdd = new List<object>() {username, time};
        CreateEntry(toAdd, "rawLB!A:B");
        if (logic.debug)
        {
            Debug.Log($"INFO: Succesfully added '{username}' with time of '{time.ToString()}' seconds to leaderboard!");
        }
    }
}
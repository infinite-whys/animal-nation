using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Linq;

public class SheetReader
{

    static private String spreadsheetId = "1Eo1KQKneNEwroTO2BehRcGvVVStpIx4KatPfxHin1Ek";

    //static private String serviceAccountID = "dev-full-access@animal-nation-439615.iam.gserviceaccount.com";
    static private SheetsService service;

    const string path = "Creds/creds";

    static SheetReader()
    {
        //  Loading private key from resources as a TextAsset
        //String key = Resources.Load<TextAsset>(path).ToString();

        //Debug.Log(key);

        // Load the JSON file from Resources
        TextAsset credsFile = Resources.Load<TextAsset>(path);
        if (credsFile == null)
        {
            Debug.LogError("Credentials file not found at path: " + path);
            return;
        }

        // Deserialize JSON using JsonUtility
        GoogleCredentials credentials = JsonUtility.FromJson<GoogleCredentials>(credsFile.text);

        if (string.IsNullOrEmpty(credentials.private_key) || string.IsNullOrEmpty(credentials.client_email))
        {
            Debug.LogError("Private key or service account email is missing in the credentials JSON.");
            return;
        }

        // Creating a  ServiceAccountCredential.Initializer
        // ref: https://googleapis.dev/dotnet/Google.Apis.Auth/latest/api/Google.Apis.Auth.OAuth2.ServiceAccountCredential.Initializer.html
        ServiceAccountCredential.Initializer initializer = new ServiceAccountCredential.Initializer(credentials.client_email);

        // Getting ServiceAccountCredential from the private key
        // ref: https://googleapis.dev/dotnet/Google.Apis.Auth/latest/api/Google.Apis.Auth.OAuth2.ServiceAccountCredential.html
        ServiceAccountCredential credential = new ServiceAccountCredential(
            initializer.FromPrivateKey(credentials.private_key)
        );

        service = new SheetsService(
            new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            }
        );
    }

    public List<List<object>> getSheetRange(String sheetNameAndRange)
    {
        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, sheetNameAndRange);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        if (values != null && values.Count > 0)
        {
            List<List<object>> listValues = values.Select(row => row.ToList()).ToList();

            /*foreach (var row in listValues)
            {
                Debug.Log(string.Join(", ", row));
            }*/
            return listValues;
        }
        else
        {
            Debug.Log("No data found.");
            return null;
        }
    }
}

[System.Serializable]
public class GoogleCredentials
{
    public string private_key;
    public string client_email;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour, IDatabase, ICollectionable
{


    FirebaseFirestore database;
    ListenerRegistration listener;

    public string FirstCollectionName { get { return "Room"; } }
    public string SecondCollectionName { get { return "Transformed"; } }
    public string FirstDocumentId { get { return ""; } set { FirstDocumentId = value; } } // Room Name created
    public string SecondDocumentId { get { return ""; } set { FirstDocumentId = value; } } //  ServerObjectName

    // Replace these values with the ones from your google-services.json
    private const string ProjectId = "unityfornotes";
    private const string ApplicationId = "1:254690127134:android:67dc900712927746e4ed0f";
    private const string ApiKey = "AIzaSyAPf7LCO1PRZ6dXqGsv8RUuycb_y_laW7E";
    private const string StorageBucket = "unityfornotes.appspot.com";

    private void Start()
    {
        InitDatabase();
    }

    public void ConnectDatabase(FirebaseApp app)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
       {
           database = Firebase.Firestore.FirebaseFirestore.GetInstance(app);

           Debug.Log("Firebase Firestore initialized successfully.");
       });
    }

    public void InitDatabase()
    {
        // Firebase configuration based on google-services.json content
        Firebase.AppOptions options = new Firebase.AppOptions()
        {
            ProjectId = "unityfornotes",
            AppId = "1:254690127134:android:67dc900712927746e4ed0f", // or the other app id
            ApiKey = "AIzaSyAPf7LCO1PRZ6dXqGsv8RUuycb_y_laW7E",
            StorageBucket = "unityfornotes.appspot.com"
        };

        FirebaseApp app = FirebaseApp.Create(options);
        ConnectDatabase(app);
        Debug.Log("Firebase initialized with AppOptions.");
    }

    public string Type()
    {
        return "Firebase";
    }

    public Dictionary<string, object> GetData()
    {
        Dictionary<string, object> documentDictionary = new Dictionary<string, object>();
        CollectionReference usersRef = database.Collection(FirstCollectionName);

        usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Id == FirstDocumentId)
                {
                    Debug.Log(String.Format("User: {0}", document.Id));
                    documentDictionary = document.ToDictionary();
                    Debug.Log(String.Format("First: {0}", documentDictionary["eeee"]));


                }


            }

            Debug.Log("Read all data from the users collection.");
        });

        return documentDictionary;
    }

    public void CreateACollectionInDocument(string documentID, string collectionName, Dictionary<string, object> dictionary)
    {
        DocumentReference docRef = database.Collection(FirstCollectionName).Document(documentID).Collection(collectionName).Document(SecondCollectionName);
        docRef.SetAsync(dictionary).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Document Created Successfully.");
        });
    }



    public IEnumerator GetRooms(System.Action<List<string>> callback)
    {
        TaskCompletionSource<List<string>> taskCompletionSource = new TaskCompletionSource<List<string>>();
        List<string> list = new List<string>();
        CollectionReference usersRef = database.Collection(FirstCollectionName);

        usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    list.Add(document.Id);
                }
                taskCompletionSource.SetResult(list);
            }
            else
            {
                taskCompletionSource.SetException(task.Exception);
            }
        });

        yield return new WaitUntil(() => taskCompletionSource.Task.IsCompleted);

        if (taskCompletionSource.Task.Exception != null)
        {
            Debug.LogError(taskCompletionSource.Task.Exception);
        }

        callback?.Invoke(taskCompletionSource.Task.Result);
    }


    public void CreateADocument(string documentID, Dictionary<string, object> dictionary, ref bool isCompelete)
    {
        DocumentReference docRef = database.Collection(FirstCollectionName).Document(documentID);
        docRef.SetAsync(dictionary).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Document Created Successfully.");
        });
        isCompelete = true;
    }

    public void SetDataInCollection(string data, object value)
    {
        DocumentReference docRef = database.Collection(FirstCollectionName).Document("room11863");
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        dictionary.Add(data, value);

        docRef.SetAsync(dictionary).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to the alovelace document in the users collection.");
        });
    }

    public void SetDataInTransformedObject(string objectName, string roomName, string secondDocumentName, Dictionary<string, object> dictionary)
    {
        DocumentReference docRef = database.Collection(FirstCollectionName).Document(roomName).Collection(objectName).Document(secondDocumentName);

        docRef.SetAsync(dictionary).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to the alovelace document in the users collection.");
        });
    }
    public async Task<Dictionary<string, object>> GetObjectFromPosition(string roomName, string objectName)
    {
        var docRef = database.Collection(FirstCollectionName).Document(roomName).Collection(objectName).Document(SecondCollectionName);
        var dictionary = new Dictionary<string, object>();

        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            dictionary = snapshot.ToDictionary();
            Debug.Log("Read all data from the users collection.");
        }
        else
        {
            Debug.LogWarning("Document does not exist!");
        }

        return dictionary;
    }



    public void ListenTransformData(string roomName, string objectName, Action<Dictionary<string, object>> callback)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        var docRef = database.Collection(FirstCollectionName).Document(roomName).Collection(objectName).Document(SecondCollectionName);
        listener = docRef.Listen(snapshot =>
        {
            if (snapshot.Exists)
            {
                Dictionary<string, object> documentData = snapshot.ToDictionary();
                Debug.Log("Document updated:");
                dictionary = documentData;
                callback?.Invoke(dictionary);
            }
            else
            {
                Debug.Log("Document does not exist.");
            }
        });
    }






}

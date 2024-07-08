using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour, ICollectionable
{
    public static MultiplayerManager instance;
    private FirebaseManager firebaseManager;
    public List<GameObject> TransformedObjects;
    [HideInInspector] public float serverDelay = 0.5f;
    [HideInInspector] public string activeRoomName;
    private bool isCompeleteForCreatingRoom;
    public Action OnJoinRoom;
    public Action OnRoomCreated;
    public string FirstCollectionName { get { return "Room"; } }
    public string SecondCollectionName { get { return "Transformed"; } }
    public string FirstDocumentId { get { return FirstDocumentId; } set { FirstDocumentId = value; } }
    public string SecondDocumentId { get { return "Transformed"; } set { FirstDocumentId = value; } } 

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        OnJoinRoom += HandleOnJoinRoom;
        OnRoomCreated += HandleOnRoomCreated;

        if (DatabaseSelecting.instance != null)
        {
            var database = DatabaseSelecting.instance.GetDatabase();
            if (database != null)
            {
                firebaseManager = database.GetComponent<FirebaseManager>();
                if (firebaseManager == null)
                {
                    Debug.LogError("FirebaseManager component could not be found.");
                }
            }
            else
            {
                Debug.LogError("Database could not be retrieved.");
            }
        }
        else
        {
            Debug.LogError("DatabaseSelecting instance is null.");
        }


        StartCoroutine(SetSettings());


    }

    protected virtual void HandleOnJoinRoom()
    {
        Debug.Log("Custom handler: Joined on Room");
    }

    protected virtual void HandleOnRoomCreated()
    {
        AddPlayerObjectToFirebaseForRoom(activeRoomName);
        Debug.Log("Custom handler: Created Room");
    }

    private IEnumerator SetSettings()
    {
        yield return new WaitForSeconds(1);
        SetNameOfObjects();
    }

    public void CreateSampleRoom()
    {
        StartCoroutine(CreateRoom());
    }

    public IEnumerator CreateRoom()
    {
        var randomValue = UnityEngine.Random.Range(0, 99999);
        var roomName = "room" + randomValue;
        isCompeleteForCreatingRoom = false;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        firebaseManager.CreateADocument(roomName, dictionary, ref isCompeleteForCreatingRoom);
        yield return new WaitUntil(predicate: () =>
        {
            return isCompeleteForCreatingRoom;
        });
        activeRoomName = roomName;
        OnRoomCreated?.Invoke();
    }

    public void JoinRandomRoom()
    {
        StartCoroutine(firebaseManager.GetRooms(OnRoomsRetrieved));
    }

    public void JoinRoom(string roomName)
    {
        OnRoomsRetrieved(roomName);
    }

    public void OnRoomsRetrieved(List<string> rooms)
    {
        var randomValue = UnityEngine.Random.Range(0, rooms.Count);
        string selectedRoom = rooms[randomValue];
        print(selectedRoom);
        activeRoomName = selectedRoom;
        AddServerObjectToFirebaseForRandomRoom(selectedRoom);
        OnJoinRoom?.Invoke();
    }

    public void OnRoomsRetrieved(string roomName)
    {
        AddServerObjectToFirebaseForRandomRoom(roomName);
        OnJoinRoom?.Invoke();
    }


    private void AddServerObjectToFirebaseForRandomRoom(string roomName)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("x", 0);
        dictionary.Add("y", 0);

        foreach (var obj in TransformedObjects)
        {
            firebaseManager.CreateACollectionInDocument(roomName, obj.GetComponent<ServerObject>().createdName, dictionary);
        }
    }

    private void AddPlayerObjectToFirebaseForRoom(string roomName)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("x", 0);
        dictionary.Add("y", 0);

        foreach (var obj in TransformedObjects)
        {
            if (obj.GetComponent<ServerObject>().IsMine)
                firebaseManager.CreateACollectionInDocument(roomName, obj.GetComponent<ServerObject>().createdName, dictionary);
        }
    }

    private void AddOtherObjectsToFirebaseForRoom(string roomName)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("x", 0);
        dictionary.Add("y", 0);

        foreach (var obj in TransformedObjects)
        {
            if (!obj.GetComponent<ServerObject>().IsMine)
                firebaseManager.CreateACollectionInDocument(roomName, obj.GetComponent<ServerObject>().createdName, dictionary);
        }
    }

    private IEnumerator AddServerObjectToFirebaseForCreatingRoom(string roomName)
    {
        yield return new WaitUntil(predicate: () =>
        {
            return isCompeleteForCreatingRoom;
        });

        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("x", 0);
        dictionary.Add("y", 0);

        foreach (var obj in TransformedObjects)
        {
            firebaseManager.CreateACollectionInDocument(roomName, obj.GetComponent<ServerObject>().createdName, dictionary);
        }
    }

    private void SetNameOfObjects()
    {
        int roomCounter = 1;

        foreach (var obj in TransformedObjects)
        {
            var objectName = "Object" + roomCounter;
            obj.GetComponent<ServerObject>().createdName = objectName;
            roomCounter++;
        }
    }

    public IEnumerator ChangeValueForTransformedObject(string objectName, float x_value, float y_value)
    {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary.Add("x", x_value);
        dictionary.Add("y", y_value);
        var isCompleated = false;

        firebaseManager.SetDataInTransformedObject(objectName, activeRoomName, SecondDocumentId, dictionary);
        yield return new WaitUntil(predicate: () =>
        {
            return isCompleated;
        });
    }

    public async Task<Vector2> GetObjectPositionAsync(GameObject transformedObject)
    {
        var objectName = transformedObject.GetComponent<ServerObject>().createdName;
        var dictionary = await firebaseManager.GetObjectFromPosition(activeRoomName, objectName);
        Vector2 position = new Vector2();

        if (dictionary.Count > 0)
        {
            foreach (var item in dictionary)
            {
                if (item.Key == "x")
                {
                    if (float.TryParse(item.Value.ToString(), out float xPos))
                    {
                        position.x = xPos;
                    }
                    else
                    {
                        Debug.LogError($"Invalid value for x: {item.Value}");
                    }
                }
                if (item.Key == "y")
                {
                    if (float.TryParse(item.Value.ToString(), out float yPos))
                    {
                        position.y = yPos;
                    }
                    else
                    {
                        Debug.LogError($"Invalid value for y: {item.Value}");
                    }
                }
            }
            return position;
        }

        return Vector2.zero; 
    }

    public Vector2 GetObjectPosition(GameObject transformedObject,Action<Vector2> callback)
    {
        var position = new Vector2();
        var objectName = transformedObject.GetComponent<ServerObject>().createdName;
        var dictionary = new Dictionary<string, object>();

        firebaseManager.ListenTransformData(activeRoomName, objectName, (dictionary) =>
        {
            if (dictionary.Count > 0)
            {
                foreach (var item in dictionary)
                {
                    if (item.Key == "x")
                    {
                        if (float.TryParse(item.Value.ToString(), out float xPos))
                        {
                            position.x = xPos;
                        }
                        else
                        {
                            Debug.LogError($"Invalid value for x: {item.Value}");
                        }
                    }
                    if (item.Key == "y")
                    {
                        if (float.TryParse(item.Value.ToString(), out float yPos))
                        {
                            position.y = yPos;
                        }
                        else
                        {
                            Debug.LogError($"Invalid value for y: {item.Value}");
                        }
                    }
                }

                print(position);
                callback?.Invoke(position);

            }

        });

        return position;
    }



}

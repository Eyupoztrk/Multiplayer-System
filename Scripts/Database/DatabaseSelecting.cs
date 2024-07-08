using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseSelecting : MonoBehaviour
{
    public static DatabaseSelecting instance;
    private void Awake()
    {
        instance = this;
    }
    public enum SelectedDatabase
    {
        Firebase,
        MySql,
        Oracle
    }


    public SelectedDatabase selectedDatabase;
    public GameObject firebaseManager;


    public GameObject GetDatabase()
    {

        if (selectedDatabase.Equals(SelectedDatabase.Firebase))
        {
           //firebaseManager.GetComponent<FirebaseManager>().SetDataInTransformedObject("d", "2", 2);
            return firebaseManager;
        }
        else if (selectedDatabase.Equals(SelectedDatabase.MySql))
        {
            return null;
        }
        else if (selectedDatabase.Equals(SelectedDatabase.Oracle))
        {
            return null;
        }
        else
        {
            return null;
        }

    }
}

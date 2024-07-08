using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDatabase
{

    public void InitDatabase();

    public string Type();

    public Dictionary<string, object> GetData();
    public void SetDataInCollection(string dataName, object value);
    public void SetDataInTransformedObject(string objectName, string roomName, string secondDocumentName, Dictionary<string, object> dictionary);
    public void CreateACollectionInDocument(string documentID, string collectionName, Dictionary<string, object> dictionary);
     public void CreateADocument(string documentID, Dictionary<string, object> dictionary, ref bool isCompelete);
}

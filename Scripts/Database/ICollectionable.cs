using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectionable 
{
   string FirstCollectionName{get;}
   string SecondCollectionName{get;}
   string FirstDocumentId{get; set;}
   string SecondDocumentId{get; set;}

}

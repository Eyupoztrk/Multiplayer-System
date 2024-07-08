using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerObject : MonoBehaviour
{
    [HideInInspector] public string createdName = "";

    public bool position;
    public bool rotation;
    public bool scale;

    public bool IsMine;
    private Vector3 lastPosition;

    private void Start()
    {
        StartCoroutine(ChangeValuesForSet());
        GetObjectPosition();
    }

    bool CheckMovingForSet()
    {
        if (transform.position != lastPosition)
        {
            lastPosition = transform.position;
            return true;
        }
        else
        {
            return false;
        }
    }


    IEnumerator ChangeValuesForSet()
    {
        while (true)
        {
            yield return new WaitUntil(predicate: () =>
            {
                return CheckMovingForSet();
            });

            var x = transform.position.x;
            var y = transform.position.y;
            StartCoroutine(MultiplayerManager.instance.ChangeValueForTransformedObject(createdName, x, y));

        }
    }

    void GetObjectPosition()
    {
        var getPositionTask = MultiplayerManager.instance.GetObjectPosition(gameObject, GetPos);
    }

    private void GetPos(Vector2 pos)
    {
        transform.position = pos;
    }

}

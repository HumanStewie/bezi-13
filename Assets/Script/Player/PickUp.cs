using UnityEngine;

public class PickUp : MonoBehaviour
{
    void Start()
    {
             
    }

    // Update is called once per frame
    void Update()
    {
        if (GridManager.Instance.GetEntityAtPosition(this.GetComponent<Entity>().coords).entityName == "Block")
        {
            GridManager.Instance.GetEntityAtPosition(this.GetComponent<Entity>().coords).transform.parent = this.transform;
        }
    }
}

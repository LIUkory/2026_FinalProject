using UnityEngine;
public class DungeonManager : MonoBehaviour
{
    public GameObject roomRerfab;
    void Start()
    {
        Instantiate(
            roomRerfab,
            Vector3.zero,
            Quaternion.identity
        );
    }
}
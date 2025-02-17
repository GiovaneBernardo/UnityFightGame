using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject CharacterPrefab;
    public List<GameObject> AvailableCharacters = new List<GameObject>();
    public Dictionary<int, GameObject> SpawnedCharacters = new Dictionary<int, GameObject>();
    void Start()
    {

    }

    void Update()
    {

    }

    public GameObject SpawnCharacter(int index)
    {
        GameObject character = Instantiate(CharacterPrefab);
        Instantiate(AvailableCharacters[index], character.transform);
        SpawnedCharacters.Add(character.GetInstanceID(), character);
        character.GetComponent<CharacterController>().Start();
        return character;
    }

    public void DestroyCharacter(int instanceId)
    {
        Destroy(SpawnedCharacters[instanceId]);
        SpawnedCharacters.Remove(instanceId);
    }
}

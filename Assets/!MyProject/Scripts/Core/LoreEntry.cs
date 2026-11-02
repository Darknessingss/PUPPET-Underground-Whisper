using UnityEngine;

[System.Serializable]
public class LoreEntry
{
    public string title;
    [TextArea(3, 10)] public string body;
    public bool isUnlocked;

    public LoreEntry() { }

    public LoreEntry(string title, string body)
    {
        this.title = title;
        this.body = body;
    }
}
using System.IO;
using Unity.Entities;
using UnityEngine;

public class LoadGameDataSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<LoadMenuTag>();
    }

    protected override void OnUpdate()
    {
        var path = Application.persistentDataPath + "\\saveData.json";
        if (!File.Exists(path)) return;

        //Ver qué pasa si está vacío

        Debug.Log("LoadGameDataSystem");

        var playerScoreBufferWrapper = JsonUtility.FromJson<PlayerTimeBufferWrapper>(
            File.ReadAllText(path));

        var playerScoreReinterpret = GetBuffer<PlayerTimeBuffer>(
            GetSingletonEntity<GameManagerTag>()).Reinterpret<PlayerTimeBuffer>();
        
        playerScoreReinterpret.Clear();
        foreach (var playerScore in playerScoreBufferWrapper.Buffer)
        {
            playerScoreReinterpret.Add(playerScore);
        }
    }
}

using System.IO;
using Unity.Entities;
using UnityEngine;

public class SaveGameDataSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate(GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<GameManagerTag>() },
            Any = new []
            {
                ComponentType.ReadOnly<ReloadMenuTag>(),
                ComponentType.ReadOnly<ReloadGameTag>()
            }
        }));
    }

    protected override void OnUpdate()
    {
        File.WriteAllText(Application.persistentDataPath + "\\saveData.json",
            JsonUtility.ToJson(
                new PlayerTimeBufferWrapper
                {
                    Buffer = GetBuffer<PlayerTimeBuffer>(GetSingletonEntity<GameManagerTag>()).AsNativeArray().ToArray()
                }));
    }
}

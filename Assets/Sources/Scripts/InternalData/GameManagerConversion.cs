using System;
using Unity.Entities;
using UnityEngine;

[Flags]
public enum GameState
{
    None = 0,
    Ready = 1 << 0,
    Starting = 1 << 1,
    Playing = 1 << 2,
    Shooting = 1 << 3,
    Landing = 1 << 4,
    GameOver = 1 << 5
}

[Serializable]
public struct GameStateData : IComponentData
{
    public GameState Value;
}

public class GameManagerConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponents(entity, new ComponentTypes(
            new ComponentType[]
            {
                typeof(GameManagerTag),
                typeof(GameStateData),
                typeof(OperationAnswer)
            }
        ));

        dstManager.SetComponentData(entity, new GameStateData {Value = GameState.Ready});
    }
}

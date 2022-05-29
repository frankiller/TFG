using Unity.Entities;
using UnityEngine;

public class GameManagerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponents(entity, new ComponentTypes(
            new ComponentType[]
            {
                typeof(GameManagerTag),
                typeof(OperationAnswerBuffer),
                typeof(PlayerTimeBuffer),
                typeof(PlayerGameplayData),
                typeof(LoadMenuTag),
                typeof(Chronometer)
            }
        ));

        dstManager.AddComponentData(entity, new GameStartData
        {
            Mode = GameMode.SumOrSubstract,
            Difficulty = GameDifficulty.Easy
        });

        dstManager.AddComponentData(entity, new ChronometerData
        {
            Action = ChronometerAction.StandBy,
            PenaltyAmountTime = 10
        });
    }
}

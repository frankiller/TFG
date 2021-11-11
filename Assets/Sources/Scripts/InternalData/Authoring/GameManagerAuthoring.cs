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
                typeof(InitializeSystemsTag),
                typeof(OperationAnswerBuffer),
            }
        ));
    }
}

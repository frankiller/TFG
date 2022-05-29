using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<CopyTransformToGameObject>(entity);
        dstManager.AddComponent<CameraTag>(entity);
        dstManager.AddComponent<CameraTargetData>(entity);
        dstManager.AddComponent<CameraTargetEntityData>(entity);
    }
}

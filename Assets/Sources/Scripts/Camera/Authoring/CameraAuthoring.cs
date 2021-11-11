using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<CopyTransformToGameObject>(entity);
        dstManager.AddComponentData(entity, new CameraTag());
        dstManager.AddComponentData(entity, new CameraTargetData());
        dstManager.AddComponentData(entity, new CameraTargetEntityData());
    }
}

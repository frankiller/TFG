using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FollowEntity : MonoBehaviour
{
    public Entity entityToFollow;
    public float3 offset = float3.zero;
    public float3 rotation = float3.zero;

    private EntityManager _entityManager;

    private void Awake() 
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {
        if (!_entityManager.Exists(entityToFollow)) { return; }

        var entPos = _entityManager.GetComponentData<LocalToWorld>(entityToFollow);

        transform.localPosition = entPos.Position - offset;

        if (math.all(offset == float3.zero))
        {
            transform.localRotation = Quaternion.LookRotation(-entPos.Up);
        }
        else
        {
            transform.LookAt(entPos.Position);
        }
    }
}

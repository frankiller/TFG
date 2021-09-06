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

    //private void FixedUpdate()
    //{
    //    if(!_entityManager.Exists(entityToFollow)) { return; }

    //    var entPos = _entityManager.GetComponentData<LocalToWorld>(entityToFollow);

    //    float wantedRotationAngle = entPos.Rotation.value.y;
    //    float wantedHeight = entPos.Position.y + 5.0f;
    //    float currentRotationAngle = transform.eulerAngles.y;
    //    float currentHeight = transform.position.y;

    //    currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, 3.0f * Time.fixedDeltaTime);
    //    currentHeight = Mathf.Lerp(currentHeight, wantedHeight, 2.0f * Time.fixedDeltaTime);

    //    var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

    //    transform.position = entPos.Position;
    //    transform.position -= currentRotation * Vector3.forward * 5.0f;
    //    transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
    //    transform.LookAt(entPos.Position);
    //}
}

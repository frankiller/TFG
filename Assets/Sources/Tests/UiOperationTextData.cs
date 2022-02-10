using Unity.Entities;

public class UiOperationTextData : IComponentData
{
    public UnityEngine.UIElements.Label Value;
    public float Radius;
}

public class UiSuccessTextData : IComponentData
{
    public UnityEngine.UIElements.Label Value;
}

public class UiCrosshairData : IComponentData
{
    public UnityEngine.UIElements.VisualElement Value;
}
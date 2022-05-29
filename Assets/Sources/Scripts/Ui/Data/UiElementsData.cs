using Unity.Entities;

public class UiChronometerTextData : IComponentData
{
    public UnityEngine.UIElements.Label Value;
}

public class UiOperationTextData : IComponentData
{
    public UnityEngine.UIElements.Label Value;
}

public class UiLabel : IComponentData
{
    public UnityEngine.UIElements.Label Value;
}

public class UiSuccessTextData : UiLabel { }

public class UiSuccessNextIslandTextData : UiLabel { }

public class UiFailureTextData : UiLabel { }

public class UiScoreTextData : IComponentData
{
    public UnityEngine.UIElements.Label Value;
}

public class UiCrosshairData : IComponentData
{
    public UnityEngine.UIElements.VisualElement Value;
}
using Unity.Entities;

public enum AnswerType
{
    None,
    Correct,
    Incorrect
}

[GenerateAuthoringComponent]
public struct AnswerLabelData : IComponentData
{
    public AnswerType Type;
    public int SecondsVisible;
}

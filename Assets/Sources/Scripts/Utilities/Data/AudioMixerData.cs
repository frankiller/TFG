using Unity.Entities;

[GenerateAuthoringComponent]
public class AudioMixerData : IComponentData
{
    public UnityEngine.Audio.AudioMixer Value;
}
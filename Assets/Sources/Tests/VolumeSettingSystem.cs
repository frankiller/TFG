using Unity.Entities;
using Unity.Mathematics;

public class VolumeSettingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((AudioMixerData mixer, in VolumeLevelData level) =>
        {
            mixer.Value.SetFloat("MenuBackgroundVolume", math.log10(level.Value) * 20);
        }).WithoutBurst().Run();
    }
}

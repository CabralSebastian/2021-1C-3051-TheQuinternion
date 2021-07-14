using Microsoft.Xna.Framework.Audio;

namespace TGC.MonoGame.TP
{
    internal class SoundManager
    {
        internal void PlaySound(SoundEffectInstance sound, AudioEmitter emitter)
        {
            sound.Apply3D(TGCGame.Camera.Listener, emitter);
            sound.Play();
        }
    }
}

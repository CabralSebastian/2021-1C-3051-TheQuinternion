namespace TGC.MonoGame.TP.Rendering
{
    internal class Material
    {
        private readonly float KAmbient, KDiffuse, KSpecular, Shininess;

        internal Material(float kAmbient, float kDiffuse, float kSpecular, float shininess)
        {
            this.KAmbient = kAmbient;
            this.KDiffuse = kDiffuse;
            this.KSpecular = kSpecular;
            this.Shininess = shininess;
        }

        internal void Set()
        {
            TGCGame.GameContent.E_MainShader.Parameters["KAmbient"]?.SetValue(KAmbient);
            TGCGame.GameContent.E_MainShader.Parameters["KDiffuse"]?.SetValue(KDiffuse);
            TGCGame.GameContent.E_MainShader.Parameters["KSpecular"]?.SetValue(KSpecular);
            TGCGame.GameContent.E_MainShader.Parameters["shininess"]?.SetValue(Shininess);
        }
    }
}
namespace TGC.MonoGame.TP.Rendering
{
    internal class Material
    {
        private readonly float kAmbient, kDiffuse, kSpecular, shininess;

        internal Material(float kAmbient, float kDiffuse, float kSpecular, float shininess)
        {
            this.kAmbient = kAmbient;
            this.kDiffuse = kDiffuse;
            this.kSpecular = kSpecular;
            this.shininess = shininess;
        }

        internal void Set()
        {
            TGCGame.content.E_MainShader.Parameters["KAmbient"]?.SetValue(kAmbient);
            TGCGame.content.E_MainShader.Parameters["KDiffuse"]?.SetValue(kDiffuse);
            TGCGame.content.E_MainShader.Parameters["KSpecular"]?.SetValue(kSpecular);
            TGCGame.content.E_MainShader.Parameters["shininess"]?.SetValue(shininess);
        }
    }
}
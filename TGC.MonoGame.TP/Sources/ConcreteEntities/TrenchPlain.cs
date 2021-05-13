﻿using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class TrenchPlain : StaticPhysicEntity<Box>
    {
        protected override Model Model() => TGCGame.content.M_Trench_Plain;
        protected override Texture2D[] Textures() => TGCGame.content.T_DeathStar;
        protected override Vector3 Scale => Vector3.One * DeathStar.trenchScale / 100f;
        protected override Box Shape => new Box(DeathStar.trenchSize, 20f, DeathStar.trenchSize);
    }
}
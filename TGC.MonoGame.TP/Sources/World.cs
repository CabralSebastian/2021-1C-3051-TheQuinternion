using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.ConcreteEntities;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;

namespace TGC.MonoGame.TP
{
    internal class World
    {
        int RandomNumber = 0;
        Random Random = new Random();

        private readonly List<Entity> pendingEntities = new List<Entity>();
        private readonly List<Entity> entities = new List<Entity>();
        internal XWing xwing;

        internal void Register(Entity entity) => pendingEntities.Add(entity);

        internal void Unregister(Entity entity) => entities.Remove(entity);

        internal void Initialize()
        {
            new DeathStar().Create();
            xwing = new XWing();
            xwing.Instantiate(new Vector3(50f, 0f, 0f));
        }

        internal void Update(double elapsedTime)
        {
            RandomNumber = Random.Next(0, 750);
            if (RandomNumber == 1)
                new TIE().Instantiate(new Vector3((float)Random.Next(100, 400), 0f, 0f));

            pendingEntities.ForEach(entity => entities.Add(entity));
            pendingEntities.Clear();
            entities.ForEach(entity => entity.Update(elapsedTime));
        }

        internal void Draw()
        {
            BasicDrawer.PreDraw();
            LaserDrawer.PreDraw();
            TurretDrawer.PreDraw();
            entities.ForEach(entity => entity.Draw());
        }
    }
}
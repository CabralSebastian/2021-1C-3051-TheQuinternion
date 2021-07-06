using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class DistantFight
    {
        private readonly Random random = new Random();
        private int MaxInstances = 20;
        private Vector3 InitialFightPosition = new Vector3(5000f, 200f, 5000f);

        public void Create()
        {
            for (int i = 0; i < MaxInstances; i++)
            {
                new DummyTIE(0f, 0f, 0f).Instantiate(InitialFightPosition + RandomVectorTIE());
                new DummyXWing(0f, 0f, 0f).Instantiate(InitialFightPosition + RandomVectorXWing());
            }
        }

        private Vector3 RandomVectorTIE()
        {
            return new Vector3((float)random.Next(1000, 2500), (float)random.Next(1000, 5000), (float)random.Next(1000, 2500));
        }

        private Vector3 RandomVectorXWing()
        {
            return new Vector3((float)random.Next(-2500, -1000), (float)random.Next(1000, 5000), (float)random.Next(-2500, -1000));
        }
    }
}
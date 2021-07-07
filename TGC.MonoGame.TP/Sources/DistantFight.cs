using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class DistantFight
    {
        private readonly Random random = new Random();
        private readonly int MaxInstances = 20;
        private Vector3 InitialFightPosition = new Vector3(1000f, 200f, 2000f);

        public void Create()
        {
            for (int i = 0; i < MaxInstances; i++)
            {
                new ShellTIE().Instantiate(InitialFightPosition + RandomVectorTIE());
                new ShellXWing().Instantiate(InitialFightPosition + RandomVectorXWing());
            }
        }

        private Vector3 RandomVectorTIE()
        {
            return new Vector3((float)random.Next(100, 750), (float)random.Next(100, 1000), (float)random.Next(100, 750));
        }

        private Vector3 RandomVectorXWing()
        {
            return new Vector3((float)random.Next(-750, -100), (float)random.Next(100, 1000), (float)random.Next(-750, -100));
        }
    }
}
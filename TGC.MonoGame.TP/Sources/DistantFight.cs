﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class DistantFight
    {
        private readonly Random Random = new Random();
        private readonly int MaxInstances = 20;
        private Vector3 InitialFightPosition = new Vector3(-600f, 300f, -3000f);

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
            return new Vector3((float)Random.Next(100, 800), (float)Random.Next(100, 1000), (float)Random.Next(0, 200));
        }

        private Vector3 RandomVectorXWing()
        {
            return new Vector3((float)Random.Next(-800, -100), (float)Random.Next(100, 1000), (float)Random.Next(0, 200));
        }
    }
}
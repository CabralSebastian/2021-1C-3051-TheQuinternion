using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.ConcreteEntities;

namespace TGC.MonoGame.TP
{
    internal class DeathStar
    {
        private readonly Random Random = new Random();
        private readonly Directions Directions = new Directions();

        internal const float OriginalTrenchSize = 28.2857f;
        internal const float OriginalTrenchHeight = 11f;
        internal const float TrenchScale = 1/7f;
        internal const float TrenchSize = OriginalTrenchSize * TrenchScale * 100f;
        internal const float TrenchHeight = OriginalTrenchHeight * TrenchScale * 100f * 1.3f;
        
        private const int Size = 40;
        private const int HalfSize = Size / 2;
        private const int Margin = 1;
        private const int TrenchCountReference = 400;

        internal WeakPoint WeakPoint;

        internal void Create(bool createTurrets) 
        {
            byte[,] matrix = GenerateMatrix();
            CleanMatrix(matrix);
            InstantiateMatrix(matrix, createTurrets);
            if (createTurrets)
                CreateSurfaceTurrets(matrix);
            CreateWeakPoint(matrix);
        }

        private int RandomMatrixValue() => Random.Next(Margin, Size - Margin + 1);
        private (int, int) RandomMatrixPosition() => (RandomMatrixValue(), RandomMatrixValue());
        private bool RandomBool(float trueProbability = 0.5f) => Random.NextDouble() <= trueProbability;

        private byte[,] GenerateMatrix()
        {
            byte[,] matrix = new byte[Size, Size];
            int totalTrenches = 0;

            AdvanceInMatrix(matrix, ref totalTrenches, (HalfSize, HalfSize), Directions.Up, 1);
            while (totalTrenches < TrenchCountReference)
            {
                (int, int) position;
                do position = RandomMatrixPosition();
                while (
                    IsAvailable(matrix, position) &&
                    IsAvailable(matrix, (position.Item1 - 1, position.Item2 - 1)) &&
                    IsAvailable(matrix, (position.Item1 - 1, position.Item2 + 0)) &&
                    IsAvailable(matrix, (position.Item1 - 1, position.Item2 + 1)) &&
                    IsAvailable(matrix, (position.Item1 + 0, position.Item2 + -1)) &&
                    IsAvailable(matrix, (position.Item1 + 0, position.Item2 + 0)) &&
                    IsAvailable(matrix, (position.Item1 + 0, position.Item2 + 1)) &&
                    IsAvailable(matrix, (position.Item1 + 1, position.Item2 + -1)) &&
                    IsAvailable(matrix, (position.Item1 + 1, position.Item2 + 0)) &&
                    IsAvailable(matrix, (position.Item1 + 1, position.Item2 + 1))
                );
                AdvanceInMatrix(matrix, ref totalTrenches, position, Directions.Up, 1);
            }

            return matrix;
        }

        private bool IsValidMatrixPosition((int, int) position) => position.Item1 > Margin && position.Item1 < Size - Margin && position.Item2 > Margin && position.Item2 < Size - Margin;

        private bool IsAvailable(byte[,] matrix, (int, int) position) => IsValidMatrixPosition(position) && matrix[position.Item1, position.Item2] == 0;

        private void AdvanceInMatrix(byte[,] matrix, ref int totalTrenches, (int, int) position, Direction direction, int forwardLenght)
        {
            matrix[position.Item1, position.Item2] = 1;
            totalTrenches++;
            Direction forward = direction.forward;
            Direction backward = direction.backward;
            Direction right = direction.right;
            Direction left = direction.left;
            (int, int) forwardPos = forward.AdvanceFrom(position);
            (int, int) rightPos = right.AdvanceFrom(position);
            (int, int) leftPos = left.AdvanceFrom(position);

            if (IsAvailable(matrix, forwardPos) && RandomBool(0.99f - (float)Math.Pow(forwardLenght, 2) * 0.0001f))
            {
                if (forwardLenght > 5)
                {
                    float rightProbability = IsAvailable(matrix, rightPos) && IsAvailable(matrix, forward.AdvanceFrom(rightPos)) && IsAvailable(matrix, backward.AdvanceFrom(rightPos)) ? forwardLenght : 0;
                    float leftProbability = IsAvailable(matrix, leftPos) && IsAvailable(matrix, forward.AdvanceFrom(leftPos)) && IsAvailable(matrix, backward.AdvanceFrom(leftPos)) ? forwardLenght : 0;
                    float crossProbability = (rightProbability + leftProbability) / 5;
                    float total = rightProbability + leftProbability + crossProbability;

                    double result = Random.NextDouble();
                    if (result < rightProbability / total)
                    {
                        AdvanceInMatrix(matrix, ref totalTrenches, rightPos, direction.right, 1);
                        AdvanceInMatrix(matrix, ref totalTrenches, forwardPos, direction.forward, 1);
                    }
                    else if (result < leftProbability / total)
                    {
                        AdvanceInMatrix(matrix, ref totalTrenches, leftPos, direction.left, 1);
                        AdvanceInMatrix(matrix, ref totalTrenches, forwardPos, direction.forward, 1);
                    }
                    else if (crossProbability < crossProbability / total)
                    {
                        AdvanceInMatrix(matrix, ref totalTrenches, rightPos, direction.right, 1);
                        AdvanceInMatrix(matrix, ref totalTrenches, leftPos, direction.left, 1);
                        AdvanceInMatrix(matrix, ref totalTrenches, forwardPos, direction.forward, 1);
                    }
                    else
                        AdvanceInMatrix(matrix, ref totalTrenches, forwardPos, direction.forward, forwardLenght + 1);
                }
                else
                    AdvanceInMatrix(matrix, ref totalTrenches, forwardPos, direction.forward, forwardLenght + 1);
            }
            else
            {
                float rightProbability = IsAvailable(matrix, rightPos) ? forwardLenght : 0;
                float leftProbability = IsAvailable(matrix, leftPos) ? forwardLenght : 0;
                float tProbability = (rightProbability + leftProbability) / 3;
                float total = rightProbability + leftProbability + tProbability + 20;

                double result = Random.NextDouble();
                if (result < rightProbability / total)
                    AdvanceInMatrix(matrix, ref totalTrenches, rightPos, direction.right, 1);
                else if (result < leftProbability / total)
                    AdvanceInMatrix(matrix, ref totalTrenches, leftPos, direction.left, 1);
                else if (tProbability < tProbability / total)
                {
                    AdvanceInMatrix(matrix, ref totalTrenches, rightPos, direction.right, 1);
                    AdvanceInMatrix(matrix, ref totalTrenches, leftPos, direction.left, 1);
                }
            }
        }

        private void CleanMatrix(byte[,] matrix)
        {
            for (int x = 0; x < Size - 1; x++)
                for (int z = 0; z < Size - 1; z++)
                    if (matrix[x, z] == 1 && matrix[x + 1, z] == 1 && matrix[x, z + 1] == 1 && matrix[x + 1, z + 1] == 1)
                        matrix[x + (RandomBool() ? 1 : 0), z + (RandomBool() ? 1 : 0)] = 0;
        }

        private void InstantiateMatrix(byte[,] matrix, bool createTurrets)
        {
            Quaternion d0 = Quaternion.Identity;
            Quaternion d90 = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI / 2);
            Quaternion d180 = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)Math.PI);
            Quaternion d270 = Quaternion.CreateFromAxisAngle(Vector3.Up, 3 * (float)Math.PI / 2);

            Dictionary<int, Action<Vector3>> instantiationMethods = new Dictionary<int, Action<Vector3>>
            {
                // UDRL
                {0000, (Vector3 position) => new Trench_Plain().Instantiate(position, d0)},
                {0011, (Vector3 position) => new Trench_Line().Instantiate(position, d0)},
                {1100, (Vector3 position) => new Trench_Line().Instantiate(position, d90)},
                {0001, (Vector3 position) => new Trench_End().Instantiate(position, d180)},
                {0010, (Vector3 position) => new Trench_End().Instantiate(position, d0)},
                {0100, (Vector3 position) => new Trench_End().Instantiate(position, d270)},
                {1000, (Vector3 position) => new Trench_End().Instantiate(position, d90)},
                {1001, (Vector3 position) => new Trench_Corner().Instantiate(position, d90)},
                {1010, (Vector3 position) => new Trench_Corner().Instantiate(position, d0)},
                {0101, (Vector3 position) => new Trench_Corner().Instantiate(position, d180)},
                {0110, (Vector3 position) => new Trench_Corner().Instantiate(position, d270)},
                {0111, (Vector3 position) => new Trench_T().Instantiate(position, d180)},
                {1011, (Vector3 position) => new Trench_T().Instantiate(position, d0)},
                {1101, (Vector3 position) => new Trench_T().Instantiate(position, d90)},
                {1110, (Vector3 position) => new Trench_T().Instantiate(position, d270)},
                {1111, (Vector3 position) => new Trench_Cross().Instantiate(position, d0)}
            };

            for (int x = 0; x < Size; x++)
                for (int z = 0; z < Size; z++)
                    if (matrix[x, z] > 0)
                    {
                        int up = IsValidMatrixPosition((x, z + 1)) && matrix[x, z + 1] > 0 ? 1000 : 0;
                        int down = IsValidMatrixPosition((x, z - 1)) && matrix[x, z - 1] > 0 ? 100 : 0;
                        int right = IsValidMatrixPosition((x - 1, z)) && matrix[x - 1, z] > 0 ? 10 : 0;
                        int left = IsValidMatrixPosition((x + 1, z)) && matrix[x + 1, z] > 0 ? 1 : 0;

                        int shape = up + down + right + left;
                        instantiationMethods[shape](new Vector3((x - HalfSize) * TrenchSize, - 100f, (z - HalfSize) * TrenchSize));

                        if (createTurrets)
                        {
                            if (shape == 1100 && RandomBool(0.3f))
                            {
                                new SmallTurret(false).Instantiate(new Vector3((x - HalfSize) * TrenchSize + RandomLineTrenchOffset(), -TrenchHeight, (z - HalfSize) * TrenchSize + RandomTrenchSizeOffset()));
                                matrix[x, z] = 2;
                            }
                            else if (shape == 0011 && RandomBool(0.3f))
                            {
                                new SmallTurret(true).Instantiate(new Vector3((x - HalfSize) * TrenchSize + RandomTrenchSizeOffset(), -TrenchHeight, (z - HalfSize) * TrenchSize + RandomLineTrenchOffset()), d90);
                                matrix[x, z] = 2;
                            }
                        }
                    }
                    else
                        instantiationMethods[0000](new Vector3((x - HalfSize) * TrenchSize, -100f, (z - HalfSize) * TrenchSize));
        }

        private void CreateSurfaceTurrets(byte[,] matrix)
        {
            for (int x = 0; x < Size; x++)
                for (int z = 0; z < Size; z++)
                    if (matrix[x, z] == 0 && RandomBool(0.15f))
                        new Turret().Instantiate(new Vector3(
                            ((x - HalfSize) * TrenchSize) + RandomTrenchSizeOffset(),
                            -100f,
                            ((z - HalfSize) * TrenchSize) + RandomTrenchSizeOffset()
                        ));
        }

        private void CreateWeakPoint(byte[,] matrix)
        {
            while (true)
            {
                for (int x = 0; x < Size; x++)
                    for (int z = 0; z < Size; z++)
                        if (x + z > 10 && matrix[x, z] == 2 && RandomBool(0.1f))
                        {
                            WeakPoint = new WeakPoint();
                            WeakPoint.Instantiate(new Vector3(
                                (x - HalfSize) * TrenchSize,
                                -TrenchHeight,
                                (z - HalfSize) * TrenchSize
                            ));
                            return;
                        }
            }
        }

        private float RandomTrenchSizeOffset() => -TrenchSize / 2 + ((float)Random.NextDouble() * TrenchSize);

        private float RandomLineTrenchOffset() => -10 + ((float)Random.NextDouble() * 20);
    }
}
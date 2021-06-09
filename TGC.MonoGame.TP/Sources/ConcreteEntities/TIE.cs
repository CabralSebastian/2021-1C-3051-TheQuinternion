using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Scenes;

enum State { SEEKING, ATTACKING, FLEEING };

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class TIE : DynamicEntity, IStaticDamageable, ILaserDamageable
    {
        protected override Drawer Drawer() => TGCGame.content.D_TIE;

        protected override Vector3 Scale => Vector3.One / 100f;
        protected override TypedIndex Shape => TGCGame.content.SH_TIE;
        protected override float Mass => 100f;

        protected State CurrentState = State.SEEKING;

        protected bool ShortFlee = false;
        protected int Health = 100;
        protected float StandarVelocity = 150f;
        protected float FastVelocity = 200f;

        protected float TimeCount = 0f;

        private double LastFire;
        private const double FireCooldownTime = 400;
        private int FireCounter = 0;

        private readonly AudioEmitter Emitter = new AudioEmitter();
        private const float LaserVolume = 0.2f;
        private SoundEffectInstance EngineSound;

        internal override void Update(double elapsedTime, GameTime gameTime)
        {
            StateMachine(gameTime);
        }

        protected internal void StateMachine(GameTime gameTime)
        {
            BodyReference body = Body();

            if (Health <= 0)
            {
                Destroy();
            }

            if (TooClose(body))
            {
                CurrentState = State.FLEEING;
                ShortFlee = true;
                Flee(body, gameTime);
                FireCounter = 0;
            }

            if (CurrentState == State.SEEKING)
            {
                if (XWingInSight(body))
                {
                    GetCloseToXWing(body, gameTime);

                    if (CloseToXWing(body))
                    {
                        CurrentState = State.ATTACKING;
                        ShootXWing(body, gameTime);
                    }
                   
                    if (Health < 40)
                    {
                        CurrentState = State.FLEEING;
                        Flee(body, gameTime);
                        FireCounter = 0;
                    }
                }
                else
                {
                    CurrentState = State.SEEKING;
                    GetCloseToXWing(body, gameTime); 
                }
            }

            else if (CurrentState == State.ATTACKING)
            {
                if (FireCounter >= 15)
                {
                    CurrentState = State.FLEEING;
                    ShortFlee = true;
                    Flee(body, gameTime);
                    FireCounter = 0;
                }

                if (CloseToXWing(body))
                {
                    ShootXWing(body, gameTime);
                }
                else
                {
                    if (Health < 40)
                    {
                        CurrentState = State.FLEEING;
                        Flee(body, gameTime);
                        FireCounter = 0;
                    }
                    else 
                    {
                        CurrentState = State.FLEEING;
                        ShortFlee = true;
                        Flee(body, gameTime);
                        FireCounter = 0;
                    }
                }
            }

            else if (CurrentState == State.FLEEING)
            {

                Flee(body, gameTime);

                if (ShortFlee)
                {                       
                    if (ShortFleeSuccess(body))
                    {
                        CurrentState = State.SEEKING;
                        ShortFlee = false;
                    }
                }

                else
                {
                    if (FleeSuccess(body))
                    {
                        CurrentState = State.SEEKING;
                    }
                }
            }
        }

        private bool FleeSuccess(BodyReference body)
        {
            return DistanceToXWing(body) > 4000f;
        }

        private bool ShortFleeSuccess(BodyReference body)
        {
            return DistanceToXWing(body) > 2000f;
        }

        private bool XWingInSight(BodyReference body)
        {
            return DistanceToXWing(body) < 1000f;
        }

        private bool CloseToXWing(BodyReference body)
        {
            return DistanceToXWing(body) < 300f;
        }

        private bool TooClose(BodyReference body)
        {
            return DistanceToXWing(body) < 100f;
        }

        private float DistanceToXWing(BodyReference body)
        {
            Vector3 TIEPosition = body.Pose.Position.ToVector3();
            Vector3 XWingPosition = World.xwing.Position();

            Vector3 DistanceVector = TIEPosition - XWingPosition;
            DistanceVector.Y = 0f;

            return DistanceVector.Length();
        }

        private void Flee(BodyReference body, GameTime gameTime)
        {
            Quaternion DegreeRotation = new Quaternion(new Vector3(0, 1, 0), 1f);
            DegreeRotation.Normalize();
            Quaternion ActualRotation = body.Pose.Orientation.ToQuaternion();
            ActualRotation.Normalize();

            Quaternion FinalRotation = Quaternion.Slerp(DegreeRotation, ActualRotation, TimeCount);
            TimeCount += (float) gameTime.ElapsedGameTime.TotalSeconds;

            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * FastVelocity).ToBEPU();
        }      

        private void GetCloseToXWing(BodyReference body, GameTime gameTime) 
        {
            Vector3 XWingDirection = World.xwing.Position() - body.Pose.Position.ToVector3();
            Quaternion RotationToXWing = new Quaternion(XWingDirection, 1f);
            RotationToXWing.Normalize();
            Quaternion ActualRotation = body.Pose.Orientation.ToQuaternion();
            ActualRotation.Normalize();

            Quaternion FinalRotation = Quaternion.Slerp(RotationToXWing, ActualRotation, 3 * (float) gameTime.ElapsedGameTime.TotalSeconds);

            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * StandarVelocity).ToBEPU();
        }

        private void FaceXWing(BodyReference body, GameTime gameTime)
        {
            Vector3 XWingDirection = World.xwing.Position() - body.Pose.Position.ToVector3();
            Quaternion RotationToXWing = new Quaternion(XWingDirection, 1f);
            RotationToXWing.Normalize();
            Quaternion ActualRotation = body.Pose.Orientation.ToQuaternion();
            ActualRotation.Normalize();
            Quaternion FinalRotation = Quaternion.Slerp(RotationToXWing, ActualRotation, 8 * (float) gameTime.ElapsedGameTime.TotalSeconds);
            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * StandarVelocity).ToBEPU();
        }

        private void ShootXWing(BodyReference body, GameTime gameTime)
        {
            FaceXWing(body, gameTime);
            Fire(body, gameTime);
        }
    
        protected void Fire(BodyReference body, GameTime gameTime)
        {
            double totalTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (totalTime < LastFire + FireCooldownTime)
                return;

            Vector3 position = body.Pose.Position.ToVector3();
            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);
            Quaternion laserOrientation = PhysicUtils.DirectionsToQuaternion(forward, Vector3.Up);
            World.InstantiateLaser(position, -forward, laserOrientation, Emitter, LaserVolume);
            LastFire = totalTime;
            FireCounter++;
        }

        void IStaticDamageable.ReceiveStaticDamage()
        {
            Destroy();
        }

        void ILaserDamageable.ReceiveLaserDamage()
        {
            Health -= 40;
        }
    }
}
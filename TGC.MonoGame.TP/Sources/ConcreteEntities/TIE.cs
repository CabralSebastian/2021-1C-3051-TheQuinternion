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
        protected override TypedIndex Shape => TGCGame.content.Sh_Sphere20;
        protected override float Mass => 100f;

        protected State CurrentState = State.SEEKING;

        protected bool ShortFlee = false;
        protected int Health = 100;
        protected float Regulator = 20f;
        protected float SlowVelocity = 100f;
        protected float StandarVelocity = 150f;
        protected float FastVelocity = 200f;

        private double LastFire;
        private const double FireCooldownTime = 400;

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

            if (CurrentState == State.SEEKING)
            {
                if (XWingInSight(body))
                {
                    GetCloseToXWing(body);

                    if (CloseToXWing(body))
                    {
                        CurrentState = State.ATTACKING;
                        ShootXWing(body, gameTime);
                    }
                   
                    if (Health < 40)
                    {
                        CurrentState = State.FLEEING;
                        Flee(body);
                    }
                }
                else
                {
                    CurrentState = State.SEEKING;
                    GetCloseToXWing(body); 
                }
            }

            else if (CurrentState == State.ATTACKING)
            {
                if (CloseToXWing(body))
                {
                    ShootXWing(body, gameTime);
                }
                else
                {
                    if (Health < 40)
                    {
                        CurrentState = State.FLEEING;
                        Flee(body);
                    }
                    else 
                    {
                        CurrentState = State.FLEEING;
                        ShortFlee = true;
                        Flee(body);                      
                    }
                }
            }

            else if (CurrentState == State.FLEEING)
            {
                if (ShortFlee)
                {
                    Flee(body);
                        
                    if (ShortFleeSuccess(body))
                    {
                        CurrentState = State.SEEKING;
                        ShortFlee = false;
                    }

                }

                else
                {
                    Flee(body);

                    if (FleeSuccess(body))
                    {
                        CurrentState = State.SEEKING;
                    }
                }
            }
        }

        private bool FleeSuccess(BodyReference body)
        {
            return DistanceToXWing(body) > 3000f;
        }

        private bool ShortFleeSuccess(BodyReference body)
        {
            return DistanceToXWing(body) > 1500f;
        }

        private void Flee(BodyReference body)
        {
            Vector3 XWingDirection = World.xwing.Position() - body.Pose.Position.ToVector3();
            XWingDirection.Y = 0;
            Quaternion RotationFromXWing = new Quaternion(-XWingDirection, 1f);
            Quaternion FinalRotation = Quaternion.Lerp(RotationFromXWing, body.Pose.Orientation.ToQuaternion(), 3f);
            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * FastVelocity).ToBEPU();
        }

        private float DistanceToXWing(BodyReference body) 
        {
            Vector3 TIEPosition = body.Pose.Position.ToVector3();
            Vector3 XWingPosition = World.xwing.Position();

            Vector3 DistanceVector = TIEPosition - XWingPosition;
            DistanceVector.Y = 0f;

            return DistanceVector.Length();
        }

        private void GetCloseToXWing(BodyReference body) 
        {
            Vector3 XWingDirection = World.xwing.Position() - body.Pose.Position.ToVector3();
            Quaternion RotationToXWing = new Quaternion(XWingDirection, 1f);
            Quaternion FinalRotation = Quaternion.Lerp(RotationToXWing, body.Pose.Orientation.ToQuaternion(), 4f);
            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * StandarVelocity).ToBEPU();
        }

        private void FaceXWing(BodyReference body)
        {
            Vector3 XWingDirection = World.xwing.Position() - body.Pose.Position.ToVector3();
            Quaternion RotationToXWing = new Quaternion(XWingDirection, 1f);
            Quaternion FinalRotation = Quaternion.Lerp(RotationToXWing, body.Pose.Orientation.ToQuaternion(), 2f);
            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * SlowVelocity).ToBEPU();
        }

        private bool CloseToXWing(BodyReference body)
        {
            return DistanceToXWing(body) < 800f;
        }

        private void ShootXWing(BodyReference body, GameTime gameTime)
        {
            FaceXWing(body);
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
        }

        private bool XWingInSight(BodyReference body)
        {
            return DistanceToXWing(body) < 2000f;
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
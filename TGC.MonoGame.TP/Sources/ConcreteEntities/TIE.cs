using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.CollitionInterfaces;
using TGC.MonoGame.TP.Drawers;
using TGC.MonoGame.TP.Entities;
using TGC.MonoGame.TP.Physics;

enum State { SEEKING, ATTACKING, FLEEING };

namespace TGC.MonoGame.TP.ConcreteEntities
{
    internal class TIE : DynamicEntity, IStaticDamageable, ILaserDamageable
    {
        protected override Drawer Drawer() => new BasicDrawer(TGCGame.content.M_TIE, TGCGame.content.T_TIE);

        protected override Vector3 Scale => Vector3.One / 100f;
        protected override TypedIndex Shape => TGCGame.content.Sh_Sphere20;
        protected override float Mass => 100f;

        protected State CurrentState = State.SEEKING;

        protected bool ShortFlee = false;
        protected int Health = 100;
        protected float Regulator = 20f;
        protected float SlowVelocity = 75f;
        protected float StandarVelocity = 150f;
        protected float FastVelocity = 200f;

        internal override void Update(double elapsedTime)
        {
            StateMachine();
        }

        protected internal void StateMachine()
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
                        ShootXWing(body);
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
                    ShootXWing(body);
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
            Vector3 XWingDirection = TGCGame.world.xwing.Position() - body.Pose.Position.ToVector3();
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
            Vector3 XWingPosition = TGCGame.world.xwing.Position();

            Vector3 DistanceVector = TIEPosition - XWingPosition;
            DistanceVector.Y = 0f;

            return DistanceVector.Length();
        }

        private void GetCloseToXWing(BodyReference body) 
        {
            Vector3 XWingDirection = TGCGame.world.xwing.Position() - body.Pose.Position.ToVector3();
            XWingDirection.Y = 0;
            Quaternion RotationToXWing = new Quaternion(XWingDirection, 1f);
            Quaternion FinalRotation = Quaternion.Lerp(RotationToXWing, body.Pose.Orientation.ToQuaternion(), 5f);
            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * StandarVelocity).ToBEPU();
        }

        private void GetCloseToXWingSlowly(BodyReference body)
        {
            Vector3 XWingDirection = TGCGame.world.xwing.Position() - body.Pose.Position.ToVector3();
            XWingDirection.Y = 0;
            Quaternion RotationToXWing = new Quaternion(XWingDirection, 1f);
            Quaternion FinalRotation = Quaternion.Lerp(RotationToXWing, body.Pose.Orientation.ToQuaternion(), 3f);
            body.Pose.Orientation = FinalRotation.ToBEPU();

            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);

            body.Velocity.Linear = (forward * SlowVelocity).ToBEPU();
        }

        private bool CloseToXWing(BodyReference body)
        {
            return DistanceToXWing(body) < 500f;
        }

        private void ShootXWing(BodyReference body)
        {
            GetCloseToXWingSlowly(body);
            Fire(body);
        }
    
        protected void Fire(BodyReference body)
        {
            Quaternion rotation = body.Pose.Orientation.ToQuaternion();
            Vector3 forward = PhysicUtils.Forward(rotation);
            new Laser().Instantiate(body.Pose.Position.ToVector3() + forward * 20, body.Pose.Orientation.ToQuaternion());
        }

        private bool XWingInSight(BodyReference body)
        {
            return DistanceToXWing(body) < 1500f;
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
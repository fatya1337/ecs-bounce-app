using System;
using System.Collections.Generic;
using Leopotam.Ecs;

namespace ConsoleApp2
{
    // ECS Компоненты
    public struct PositionComponent
    {
        public Vector2 Position;
    }

    public struct VelocityComponent
    {
        public Vector2 Velocity;
    }

    public struct BallComponent
    {
        public float Radius;
    }

    public struct WallComponent
    {
        public int WallId;
        public Vector2 Start;
        public Vector2 End;
    }

    // ECS Системы
    public class MovementSystem : IEcsRunSystem
    {
        private readonly Scene _scene;
        private EcsFilter<PositionComponent, VelocityComponent, BallComponent> _balls;

        public MovementSystem(Scene scene)
        {
            _scene = scene;
        }

        public void Run()
        {
            foreach (var i in _balls)
            {
                ref var pos = ref _balls.Get1(i);
                ref var vel = ref _balls.Get2(i);

                // Обновляем позицию в ECS
                pos.Position = new Vector2(
                    pos.Position.X + vel.Velocity.X,
                    pos.Position.Y + vel.Velocity.Y
                );

                // Синхронизируем с объектом сцены
                if (_scene.Ball != null)
                {
                    _scene.Ball.Center = pos.Position;
                    _scene.Ball.Velocity = vel.Velocity;
                }
            }
        }
    }

    public class BounceSystem : IEcsRunSystem
    {
        private readonly Scene _scene;
        private EcsFilter<PositionComponent, VelocityComponent, BallComponent> _balls;
        private EcsFilter<WallComponent> _walls;

        public BounceSystem(Scene scene)
        {
            _scene = scene;
        }

        public void Run()
        {
            foreach (var ballIdx in _balls)
            {
                ref var pos = ref _balls.Get1(ballIdx);
                ref var vel = ref _balls.Get2(ballIdx);
                ref var ball = ref _balls.Get3(ballIdx);

                foreach (var wallIdx in _walls)
                {
                    ref var wall = ref _walls.Get1(wallIdx);

                    // Ищем стену по ID
                    Wall foundWall = null;
                    foreach (var w in _scene.Walls)
                    {
                        if (w.Id == wall.WallId)
                        {
                            foundWall = w;
                            break;
                        }
                    }

                    if (foundWall != null && foundWall.CheckCollision(pos.Position, ball.Radius, out var normal))
                    {
                        // Отскок
                        var dot = vel.Velocity.Dot(normal);
                        vel.Velocity = new Vector2(
                            vel.Velocity.X - normal.X * (2 * dot),
                            vel.Velocity.Y - normal.Y * (2 * dot)
                        );

                        // Вывод ID стены
                        Console.WriteLine($" Удар о стену ID: {wall.WallId}");

                        // Корректировка позиции
                        pos.Position = new Vector2(
                            pos.Position.X + normal.X * 0.1f,
                            pos.Position.Y + normal.Y * 0.1f
                        );
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Leopotam.Ecs;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ECS Bounce Application ===");

            // Создаем сцену с коробочкой и шариком
            var scene = new Scene();
            scene.Initialize();

            // Создаем мир ECS
            var world = new EcsWorld();
            var systems = new EcsSystems(world);

            // Создаем ECS сущности на основе объектов сцены
            CreateEcsEntities(world, scene);

            // Добавляем системы
            systems
                .Add(new MovementSystem(scene))
                .Add(new BounceSystem(scene))
                .Init();

            Console.WriteLine("Запуск главного цикла...");
            Console.WriteLine("Нажмите Ctrl+C для выхода\n");

            // Главный игровой цикл
            var frameCount = 0;
            while (true)
            {
                frameCount++;
                systems.Run();

                // Вывод позиции шарика каждые 10 кадров
                if (frameCount % 10 == 0)
                {
                    var ball = scene.GetBall();
                    if (ball != null)
                    {
                        Console.WriteLine($" Позиция шарика: ({ball.Center.X:F2}, {ball.Center.Y:F2})");
                    }
                }

                Thread.Sleep(16); // ~60 FPS
            }

            // Очистка (необязательно для консольного приложения)
            // systems.Destroy();
            // world.Destroy();
        }

        static void CreateEcsEntities(EcsWorld world, Scene scene)
        {
            // Создаем сущности для стен
            foreach (var wall in scene.Walls)
            {
                var entity = world.NewEntity();
                ref var wallComp = ref entity.Get<WallComponent>();
                wallComp.WallId = wall.Id;
                wallComp.Start = wall.Start;
                wallComp.End = wall.End;
            }

            // Создаем сущность для шарика
            var ball = scene.Ball;
            if (ball != null)
            {
                var entity = world.NewEntity();
                entity.Get<PositionComponent>() = new PositionComponent { Position = ball.Center };
                entity.Get<VelocityComponent>() = new VelocityComponent { Velocity = ball.Velocity };
                entity.Get<BallComponent>() = new BallComponent { Radius = ball.Radius };
            }
        }
    }
}
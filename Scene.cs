using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    // Геометрические примитивы
    public struct Vector2
    {
        public float X, Y;
        public Vector2(float x, float y) { X = x; Y = y; }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 v, float s)
        {
            return new Vector2(v.X * s, v.Y * s);
        }

        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        public Vector2 Normalized
        {
            get
            {
                var mag = Magnitude;
                return new Vector2(X / mag, Y / mag);
            }
        }

        public float Dot(Vector2 other)
        {
            return X * other.X + Y * other.Y;
        }
    }

    public class Wall
    {
        public int Id { get; }
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        private static int _nextId = 1;

        public Wall(Vector2 start, Vector2 end)
        {
            Id = _nextId++;
            Start = start;
            End = end;
        }

        public Vector2 Direction
        {
            get
            {
                return End - Start;
            }
        }

        public bool CheckCollision(Vector2 center, float radius, out Vector2 normal)
        {
            normal = default(Vector2);
            var toCenter = center - Start;
            var segmentDir = Direction;
            var segmentLength = Direction.Magnitude;

            var t = toCenter.Dot(segmentDir) / (segmentLength * segmentLength);
            t = Math.Max(0, Math.Min(1, t));

            var closestPoint = Start + segmentDir * t;
            var distance = (center - closestPoint).Magnitude;

            if (distance <= radius)
            {
                normal = new Vector2(-segmentDir.Y, segmentDir.X);
                normal = normal.Normalized;
                return true;
            }

            return false;
        }
    }

    public class Ball
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }
        public Vector2 Velocity { get; set; }

        public Ball(Vector2 center, float radius, Vector2 velocity)
        {
            Center = center;
            Radius = radius;
            Velocity = velocity;
        }
    }

    public class Scene
    {
        public List<Wall> Walls { get; }
        public Ball Ball { get; private set; }

        public Scene()
        {
            Walls = new List<Wall>();
        }

        public void Initialize()
        {
            // 4 стены - коробочка
            Walls.Add(new Wall(new Vector2(0, 0), new Vector2(10, 0)));   // низ
            Walls.Add(new Wall(new Vector2(10, 0), new Vector2(10, 10))); // право
            Walls.Add(new Wall(new Vector2(10, 10), new Vector2(0, 10))); // верх
            Walls.Add(new Wall(new Vector2(0, 10), new Vector2(0, 0)));   // лево

            // Шарик внутри
            Ball = new Ball(new Vector2(5, 5), 0.5f, new Vector2(0.12f, 0.08f));
        }

        public Ball GetBall()
        {
            return Ball;
        }
    }
}
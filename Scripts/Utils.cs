using System;
using System.Collections.Generic;
using Godot;

namespace Pioneer {
    public class Utils {
        public static float Tau = 2f * Mathf.Pi;

        public Utils() {
        }

        public static float Mod(float x, float m) {
            return (x % m + m ) %m;
        }
    }

    public class LongHash {
        public static ulong GetHashCodeInt64(string input) {
            var s1 = input.Substring(0, input.Length / 2);
            var s2 = input.Substring(input.Length / 2);
            var h = (ulong)(s1.GetHashCode()) << 0x20 | (uint)s2.GetHashCode();
            return h;
        }

    }

	public struct Edge<T> {
		public T a;
		public T b;
		public float value;
		public Edge(T a, T b, float value) {
			this.a = a;
			this.b = b;
			this.value = value;
		}

		public override bool Equals(object obj) {
			return obj is Edge<T> edge && (Edge<T>)obj == this;
		}

		public override int GetHashCode() {
			int hashCode = 2118541809;
			hashCode = hashCode * -1521134295 + (EqualityComparer<T>.Default.GetHashCode(a) + 
												 EqualityComparer<T>.Default.GetHashCode(b));
			return hashCode;
		}

		public static bool operator==(Edge<T> u, Edge<T> v) {
			return (u.a.Equals(v.a) && u.b.Equals(v.b)) || (u.a.Equals(v.b) && u.b.Equals(v.a));
		}
		public static bool operator!=(Edge<T> u, Edge<T> v) {
			return (!u.a.Equals(v.a) || !u.b.Equals(v.b)) && (!u.a.Equals(v.b) || !u.b.Equals(v.a));
		}
	}

    public class RandList<T> : List<T> {
    	RandomNumberGenerator rng = new RandomNumberGenerator();
    	public RandList() {
    		rng.Randomize();
    	}

        public RandList(IEnumerable<T> items) {
            rng.Randomize();
            foreach (var i in items) {
                Add(i);
            }
        }

    	public T Pop() {
    		int randIndex = rng.RandiRange(0, Count - 1);
    		T result = this[randIndex];
    		this[randIndex] = this[Count - 1];
    		RemoveAt(Count - 1);
    		return result;
    	}

    	public bool IsEmpty() {
    		return Count == 0;
    	}
    }
    public struct Vector2Int {
        public readonly int x;
		public readonly int y;
		public static readonly Vector2Int Zero = new Vector2Int(0, 0);
        public static readonly Vector2Int N = new Vector2Int(0, 1);
        public static readonly Vector2Int S = new Vector2Int(0, -1);
        public static readonly Vector2Int E = new Vector2Int(1, 0);
        public static readonly Vector2Int W = new Vector2Int(-1, 0);

		public Vector2Int(int x, int y) {
			this.x = x;
			this.y = y;
		}

        public Vector2Int Normalized() {
            if (x == 0 && y == 0) return Vector2Int.Zero;
            double length = Math.Sqrt(x * x + y * y);
            return new Vector2Int((int)Math.Round((double)x / length),
                                  (int)Math.Round((double)y / length));
		}

        public int Manhattan(Vector2Int v) {
            return  Math.Abs(x - v.x) + Math.Abs(y - v.y);
        }

        public float Magnitude() {
            return (float)Math.Sqrt(x * x + y * y);
		}

        public static bool operator ==(Vector2Int a, Vector2Int b) {
			return a.x == b.x && a.y == b.y;
		}
        public static bool operator !=(Vector2Int a, Vector2Int b) {
			return a.x != b.x || a.y != b.y;
		}

        public static Vector2Int operator *(int i, Vector2Int v) {
            return new Vector2Int(i * v.x, i * v.y);
		}

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) {
            return new Vector2Int(a.x + b.x, a.y + b.y);
		}

        public static Vector2Int operator -(Vector2Int a, Vector2Int b) {
            return new Vector2Int(a.x - b.x, a.y - b.y);
		}

        public override bool Equals(object obj) {
            return obj is Vector2Int @int &&
                   x == @int.x &&
                   y == @int.y;
        }

        public override string ToString() {
            return String.Format("({0}, {1})", x, y);
        }

        public override int GetHashCode() {
            int hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }
}

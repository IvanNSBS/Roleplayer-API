#if GODOT
using Godot;

namespace INUlib.Core.Math
{
    public partial struct int2
    {
        public static implicit operator Vector2(int2 i) => new (i.x, i.y);
        public static implicit operator Vector2I(int2 i) => new (i.x, i.y);
        
        public static implicit operator int2(Vector2I i) => new (i.X, i.Y);
    }

    public partial struct int3
    {
        public static implicit operator Vector3(int3 i) => new (i.x, i.y, i.z);
        public static implicit operator Vector3I(int3 i) => new (i.x, i.y, i.z);

        public static implicit operator int3(Vector3I i) => new (i.X, i.Y, i.Z);
    }

    public partial struct int4
    {
        public static implicit operator Vector3(int4 i) => new (i.x, i.y, i.z);
        public static implicit operator Vector3I(int4 i) => new (i.x, i.y, i.z);
        
        public static implicit operator Vector4(int4 i) => new (i.x, i.y, i.z, i.w);
    }
    
    public partial struct float2
    {
        public static implicit operator Vector2(float2 i) => new (i.x, i.y);
        public static implicit operator Vector3(float2 i) => new (i.x, i.y, 0);
        
        public static implicit operator float2(Vector2 i) => new (i.X, i.Y);
        public static implicit operator float2(Vector2I i) => new (i.Y, i.Y);
    }

    public partial struct float3
    {
        public static implicit operator Vector3(float3 i) => new (i.x, i.y, i.z);

        public static implicit operator float3(Vector3 i) => new (i.X, i.Y, i.Z);
        public static implicit operator float3(Vector3I i) => new (i.X, i.Y, i.Z);
    }

    public partial struct float4
    {
        public static implicit operator Vector3(float4 i) => new (i.x, i.y, i.z);
        public static implicit operator Vector4(float4 i) => new (i.x, i.y, i.z, i.w);
        
        public static implicit operator float4(Vector4 i) => new (i.X, i.Y, i.Z, i.W);
    }
}

#endif
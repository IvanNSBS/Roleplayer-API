#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace Utils.Math
{
    public partial struct int2
    {
        public static implicit operator Vector2(int2 i) => new (i.x, i.y);
        public static implicit operator Vector2Int(int2 i) => new (i.x, i.y);
        
        public static implicit operator int2(Vector2Int i) => new (i.x, i.y);
    }

    public partial struct int3
    {
        public static implicit operator Vector3(int3 i) => new (i.x, i.y, i.z);
        public static implicit operator Vector3Int(int3 i) => new (i.x, i.y, i.z);

        public static implicit operator int3(Vector3Int i) => new (i.x, i.y, i.z);
    }

    public partial struct int4
    {
        public static implicit operator Vector3(int4 i) => new (i.x, i.y, i.z);
        public static implicit operator Vector3Int(int4 i) => new (i.x, i.y, i.z);
        
        public static implicit operator Vector4(int4 i) => new (i.x, i.y, i.z, i.w);
    }
    
    public partial struct float2
    {
        public static implicit operator Vector2(float2 i) => new (i.x, i.y);
        public static implicit operator Vector3(float2 i) => new (i.x, i.y, 0);
        
        public static implicit operator float2(Vector2 i) => new (i.x, i.y);
        public static implicit operator float2(Vector2Int i) => new (i.x, i.y);
    }

    public partial struct float3
    {
        public static implicit operator Vector3(float3 i) => new (i.x, i.y, i.z);

        public static implicit operator float3(Vector3 i) => new (i.x, i.y, i.z);
        public static implicit operator float3(Vector3Int i) => new (i.x, i.y, i.z);
    }

    public partial struct float4
    {
        public static implicit operator Vector3(float4 i) => new (i.x, i.y, i.z);
        public static implicit operator Vector4(float4 i) => new (i.x, i.y, i.z, i.w);
        
        public static implicit operator float4(Vector4 i) => new (i.x, i.y, i.z, i.w);
    }
}

#endif
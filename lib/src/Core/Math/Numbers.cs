namespace INUlib.Core.Math
{
    public partial struct int2
    {
        private int v0, v1;

        public int x => v0;
        public int y => v1;
        public int r => v0;
        public int g => v1;
        
        public int2 xx => new (x, x);
        public int2 yy => new (y, y);

        public int2 yx => new (y, x);
        public int2 xy => new (x, y);
        
        public int2 rr => new (x, x);
        public int2 gg => new (y, y);

        public int2 gr => new (y, x);
        public int2 rg => new (x, y);

        public int2(int v1, int v2)
        {
            this.v0 = v1;
            this.v1 = v2;
        }
        
        public static int2 operator +(int2 a, int2 b) => new (a.x + b.x, a.y + b.y);
        public static int2 operator +(int2 a, int3 b) => new (a.x + b.x, a.y + b.y);
        public static int2 operator +(int2 a, int4 b) => new (a.x + b.x, a.y + b.y);
        
        public static int2 operator -(int2 a, int2 b) => new (a.x - b.x, a.y - b.y);
        public static int2 operator -(int2 a, int3 b) => new (a.x - b.x, a.y - b.y);
        public static int2 operator -(int2 a, int4 b) => new (a.x - b.x, a.y - b.y);

        public static int2 operator *(int2 a, int b) => new (a.x * b, a.y * b);
        public static int2 operator /(int2 a, int b) => new (a.x / b, a.y / b);
    }

    public partial struct int3
    {
        private int v0, v1, v2;
        
        public int x => v0;
        public int y => v1;
        public int z => v2;
        
        public int r => v0;
        public int g => v1;
        public int b => v2;


        public int2 xx => new (x, x);
        public int2 yy => new (y, y);
        public int2 zz => new (z, z);
        
        public int2 yx => new (y, x);
        public int2 yz => new (y, z);
        
        public int2 xy => new (x, y);
        public int2 xz => new (x, z);
        
        public int2 zy => new (z, y);
        public int2 zx => new (z, x);

        public int3 xxx => new(x, x, x);
        public int3 xxy => new(x, x, y);
        public int3 xxz => new(x, x, z);
        
        public int3 xyx => new(x, y, x);
        public int3 xyy => new(x, y, y);
        public int3 xyz => new(x, y, z);
        
        public int3 xzx => new(x, z, x);
        public int3 xzy => new(x, z, y);
        public int3 xzz => new(x, z, z);
        
        public int3 yxx => new(y, x, x);
        public int3 yxy => new(y, x, y);
        public int3 yxz => new(y, x, z);
        
        public int3 yyx => new(y, y, x);
        public int3 yyy => new(y, y, y);
        public int3 yyz => new(y, y, z);
        
        public int3 yzx => new(y, z, x);
        public int3 yzy => new(y, z, y);
        public int3 yzz => new(y, z, z);
        
        public int3 zxx => new(z, x, x);
        public int3 zxy => new(z, x, y);
        public int3 zxz => new(z, x, z);
        
        public int3 zyx => new(z, y, x);
        public int3 zyy => new(z, y, y);
        public int3 zyz => new(z, y, z);
        
        public int3 zzx => new(z, z, x);
        public int3 zzy => new(z, z, y);
        public int3 zzz => new(z, z, z);
        
        public int3(int v1, int v2, int v3)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
        }
        
        public static int3 operator +(int3 a, int2 b) => new (a.x + b.x, a.y + b.y, a.z);
        public static int3 operator +(int3 a, int3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z);
        public static int3 operator +(int3 a, int4 b) => new (a.x + b.x, a.y + b.y, a.z + b.z);
        
        public static int3 operator -(int3 a, int2 b) => new (a.x - b.x, a.y - b.y, a.z);
        public static int3 operator -(int3 a, int3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z);
        public static int3 operator -(int3 a, int4 b) => new (a.x - b.x, a.y - b.y, a.z - b.z);
        
        public static int3 operator *(int3 a, int b) => new (a.x * b, a.y * b, a.z * b);
        public static int3 operator /(int3 a, int b) => new (a.x / b, a.y / b, a.z / b);
    }

    public partial struct int4
    {
        private int v0, v1, v2, v3;
        
        public int x => v0;
        public int y => v1;
        public int z => v2;
        public int w => v3;
        
        public int r => v0;
        public int g => v1;
        public int b => v2;
        public int a => v3;

        public int4(int v1, int v2, int v3, int v4)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
            this.v3 = v4;
        }
        
        public static int4 operator +(int4 a, int2 b) => new (a.x + b.x, a.y + b.y, a.z, a.w);
        public static int4 operator +(int4 a, int3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z, a.w);
        public static int4 operator +(int4 a, int4 b) => new (a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        
        public static int4 operator -(int4 a, int2 b) => new (a.x - b.x, a.y - b.y, a.z, a.w);
        public static int4 operator -(int4 a, int3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z, a.w);
        public static int4 operator -(int4 a, int4 b) => new (a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        
        public static int4 operator *(int4 a, int b) => new (a.x * b, a.y * b, a.z * b, a.w * b);
        public static int4 operator /(int4 a, int b) => new (a.x / b, a.y / b, a.z / b, a.w / b);
    }
    
    public partial struct float2
    {
        private float v0, v1;
        
        public float x => v0;
        public float y => v1;
        
        public float r => v0;
        public float g => v1;

        public float2(float v1, float v2)
        {
            this.v0 = v1;
            this.v1 = v2;
        }
        
        public float2 xx => new (x, x);
        public float2 yy => new (y, y);
        
        public float2 yx => new (y, x);
        public float2 xy => new (x, y);
        
        public static float2 operator +(float2 a, float2 b) => new (a.x + b.x, a.y + b.y);
        public static float2 operator -(float2 a, float2 b) => new (a.x - b.x, a.y - b.y);
        
        
        public static float2 operator *(float2 a, int b) => new (a.x * b, a.y * b);
        public static float2 operator /(float2 a, int b) => new (a.x / b, a.y / b);
        
        public static float2 operator *(float2 a, float b) => new (a.x * b, a.y * b);
        public static float2 operator /(float2 a, float b) => new (a.x / b, a.y / b);
    }

    public partial struct float3
    {
        private float v0, v1, v2;
        
        public float x => v0;
        public float y => v1;
        public float z => v2;

        public float r => v0;
        public float g => v1;
        public float b => v2;

        public float3(float v1, float v2, float v3)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
        }

        public float2 xx => new (x, x);
        public float2 yy => new (y, y);
        public float2 zz => new (z, z);
        
        public float2 yx => new (y, x);
        public float2 yz => new (y, z);
        
        public float2 xy => new (x, y);
        public float2 xz => new (x, z);
        
        public float2 zy => new (z, y);
        public float2 zx => new (z, x);
        
        public float3 xxx => new(x, x, x);
        public float3 xxy => new(x, x, y);
        public float3 xxz => new(x, x, z);
        
        public float3 xyx => new(x, y, x);
        public float3 xyy => new(x, y, y);
        public float3 xyz => new(x, y, z);
        
        public float3 xzx => new(x, z, x);
        public float3 xzy => new(x, z, y);
        public float3 xzz => new(x, z, z);
        
        public float3 yxx => new(y, x, x);
        public float3 yxy => new(y, x, y);
        public float3 yxz => new(y, x, z);
        
        public float3 yyx => new(y, y, x);
        public float3 yyy => new(y, y, y);
        public float3 yyz => new(y, y, z);
        
        public float3 yzx => new(y, z, x);
        public float3 yzy => new(y, z, y);
        public float3 yzz => new(y, z, z);
        
        public float3 zxx => new(z, x, x);
        public float3 zxy => new(z, x, y);
        public float3 zxz => new(z, x, z);
        
        public float3 zyx => new(z, y, x);
        public float3 zyy => new(z, y, y);
        public float3 zyz => new(z, y, z);
        
        public float3 zzx => new(z, z, x);
        public float3 zzy => new(z, z, y);
        public float3 zzz => new(z, z, z);
        
        public static float3 operator +(float3 a, float2 b) => new (a.x + b.x, a.y + b.y, a.z);
        public static float3 operator +(float3 a, float3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z);
        
        public static float3 operator -(float3 a, float2 b) => new (a.x - b.x, a.y - b.y, a.z);
        public static float3 operator -(float3 a, float3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z);

        public static float3 operator *(float3 a, int b) => new (a.x * b, a.y * b, a.z * b);
        public static float3 operator /(float3 a, int b) => new (a.x / b, a.y / b, a.z / b);
        
        public static float3 operator *(float3 a, float b) => new (a.x * b, a.y * b, a.z * b);
        public static float3 operator /(float3 a, float b) => new (a.x / b, a.y / b, a.z / b);
    }

    public partial struct float4
    {
        private float v0, v1, v2, v3;
        
        public float x => v0;
        public float y => v1;
        public float z => v2;
        public float w => v3;
        
        public float r => v0;
        public float g => v1;
        public float b => v2;
        public float a => v3;

        public float4(float v1, float v2, float v3, float v4)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
            this.v3 = v4;
        }
        
        public static float4 operator +(float4 a, float2 b) => new (a.x + b.x, a.y + b.y, a.z, a.w);
        public static float4 operator +(float4 a, float3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z, a.w);
        public static float4 operator +(float4 a, float4 b) => new (a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        
        public static float4 operator -(float4 a, float2 b) => new (a.x - b.x, a.y - b.y, a.z, a.w);
        public static float4 operator -(float4 a, float3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z, a.w);
        public static float4 operator -(float4 a, float4 b) => new (a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        
        public static float4 operator *(float4 a, int b) => new (a.x * b, a.y * b, a.z * b, a.w * b);
        public static float4 operator /(float4 a, int b) => new (a.x / b, a.y / b, a.z / b, a.w / b);
        
        public static float4 operator *(float4 a, float b) => new (a.x * b, a.y * b, a.z * b, a.w * b);
        public static float4 operator /(float4 a, float b) => new (a.x / b, a.y / b, a.z / b, a.w / b);
    }
}
namespace INUlib.Core.Math
{
    public partial struct int2
    {
        #region Fields
        private int v0, v1;
        #endregion

        #region Properties
        public int x { get => v0; set => v0 = value; }
        public int y { get => v1; set => v1 = value; }
        public int r { get => v0; set => v0 = value; }
        public int g { get => v1; set => v1 = value; }
        #endregion

        #region Constructor
        public int2(int v0, int v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
        #endregion
        
        #region Operators
        public static int2 operator +(int2 a, int2 b) => new (a.x + b.x, a.y + b.y);
        public static int2 operator +(int2 a, int3 b) => new (a.x + b.x, a.y + b.y);
        public static int2 operator +(int2 a, int4 b) => new (a.x + b.x, a.y + b.y);
        
        public static int2 operator -(int2 a, int2 b) => new (a.x - b.x, a.y - b.y);
        public static int2 operator -(int2 a, int3 b) => new (a.x - b.x, a.y - b.y);
        public static int2 operator -(int2 a, int4 b) => new (a.x - b.x, a.y - b.y);

        public static int2 operator *(int2 a, int b) => new (a.x * b, a.y * b);
        public static int2 operator /(int2 a, int b) => new (a.x / b, a.y / b);
        #endregion
    }

    public partial struct int3
    {
        #region Fields
        private int v0, v1, v2;
        #endregion

        #region Properties
        public int x { get => v0; set => v0 = value; }
        public int y { get => v1; set => v1 = value; }
        public int z { get => v2; set => v2 = value; }
        
        public int r { get => v0; set => v0 = value; }
        public int g { get => v1; set => v1 = value; }
        public int b { get => v2; set => v2 = value; }
        public int3 xzy => new(x, z, y);
        #endregion

        #region Constructor        
        public int3(int v1, int v2, int v3)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
        }
        #endregion

        #region Operators
        public static int3 operator +(int3 a, int2 b) => new (a.x + b.x, a.y + b.y, a.z);
        public static int3 operator +(int3 a, int3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z);
        public static int3 operator +(int3 a, int4 b) => new (a.x + b.x, a.y + b.y, a.z + b.z);
        
        public static int3 operator -(int3 a, int2 b) => new (a.x - b.x, a.y - b.y, a.z);
        public static int3 operator -(int3 a, int3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z);
        public static int3 operator -(int3 a, int4 b) => new (a.x - b.x, a.y - b.y, a.z - b.z);
        
        public static int3 operator *(int3 a, int b) => new (a.x * b, a.y * b, a.z * b);
        public static int3 operator /(int3 a, int b) => new (a.x / b, a.y / b, a.z / b);
        #endregion
    }

    public partial struct int4
    {
        #region Fields
        private int v0, v1, v2, v3;
        #endregion

        #region Properties
        public int x { get => v0; set => v0 = value; }
        public int y { get => v1; set => v1 = value; }
        public int z { get => v2; set => v2 = value; }
        public int w { get => v3; set => v3 = value; }
        
        public int r { get => v0; set => v0 = value; }
        public int g { get => v1; set => v1 = value; }
        public int b { get => v2; set => v2 = value; }
        public int a { get => v3; set => v3 = value; }
        #endregion

        #region Constructor
        public int4(int v1, int v2, int v3, int v4)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
            this.v3 = v4;
        }
        #endregion

        #region Operators
        public static int4 operator +(int4 a, int2 b) => new (a.x + b.x, a.y + b.y, a.z, a.w);
        public static int4 operator +(int4 a, int3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z, a.w);
        public static int4 operator +(int4 a, int4 b) => new (a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        
        public static int4 operator -(int4 a, int2 b) => new (a.x - b.x, a.y - b.y, a.z, a.w);
        public static int4 operator -(int4 a, int3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z, a.w);
        public static int4 operator -(int4 a, int4 b) => new (a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        
        public static int4 operator *(int4 a, int b) => new (a.x * b, a.y * b, a.z * b, a.w * b);
        public static int4 operator /(int4 a, int b) => new (a.x / b, a.y / b, a.z / b, a.w / b);
        #endregion
    }
    
    public partial struct float2
    {
        #region Fields
        private float v0, v1;
        #endregion

        #region Properties
        public float x { get => v0; set => v0 = value; }
        public float y { get => v1; set => v1 = value; }
        public float r { get => v0; set => v0 = value; }
        public float g { get => v1; set => v1 = value; }
        #endregion

        #region Constructor
        public float2(float v1, float v2)
        {
            this.v0 = v1;
            this.v1 = v2;
        }
        #endregion
        
        #region Operators
        public static float2 operator +(float2 a, float2 b) => new (a.x + b.x, a.y + b.y);
        public static float2 operator -(float2 a, float2 b) => new (a.x - b.x, a.y - b.y);
        
        
        public static float2 operator *(float2 a, int b) => new (a.x * b, a.y * b);
        public static float2 operator /(float2 a, int b) => new (a.x / b, a.y / b);
        
        public static float2 operator *(float2 a, float b) => new (a.x * b, a.y * b);
        public static float2 operator /(float2 a, float b) => new (a.x / b, a.y / b);
        #endregion
    }

    public partial struct float3
    {
        #region Fields
        private float v0, v1, v2;
        #endregion

        #region Properties
        public float x { get => v0; set => v0 = value; }
        public float y { get => v1; set => v1 = value; }
        public float z { get => v2; set => v2 = value; }
        
        public float r { get => v0; set => v0 = value; }
        public float g { get => v1; set => v1 = value; }
        public float b { get => v2; set => v2 = value; }

        public float3 xzy => new(x, z, y);
        #endregion

        #region Constructor
        public float3(float v1, float v2, float v3)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
        }
        #endregion
        
        #region Operators
        public static float3 operator +(float3 a, float2 b) => new (a.x + b.x, a.y + b.y, a.z);
        public static float3 operator +(float3 a, float3 b) => new (a.x + b.x, a.y + b.y, a.z + b.z);
        
        public static float3 operator -(float3 a, float2 b) => new (a.x - b.x, a.y - b.y, a.z);
        public static float3 operator -(float3 a, float3 b) => new (a.x - b.x, a.y - b.y, a.z - b.z);

        public static float3 operator *(float3 a, int b) => new (a.x * b, a.y * b, a.z * b);
        public static float3 operator /(float3 a, int b) => new (a.x / b, a.y / b, a.z / b);
        
        public static float3 operator *(float3 a, float b) => new (a.x * b, a.y * b, a.z * b);
        public static float3 operator /(float3 a, float b) => new (a.x / b, a.y / b, a.z / b);
        #endregion
    }

    public partial struct float4
    {
        #region Fields
        private float v0, v1, v2, v3;
        #endregion

        #region Properties
        public float x { get => v0; set => v0 = value; }
        public float y { get => v1; set => v1 = value; }
        public float z { get => v2; set => v2 = value; }
        public float w { get => v3; set => v3 = value; }
        
        public float r { get => v0; set => v0 = value; }
        public float g { get => v1; set => v1 = value; }
        public float b { get => v2; set => v2 = value; }
        public float a { get => v3; set => v3 = value; }
        #endregion

        #region Constructor
        public float4(float v1, float v2, float v3, float v4)
        {
            this.v0 = v1;
            this.v1 = v2;
            this.v2 = v3;
            this.v3 = v4;
        }
        #endregion
        
        #region Operators
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
        #endregion
    }
}
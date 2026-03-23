// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static JoltPhysicsSharp.JoltApi;
using VRageMath;
using System.Runtime.CompilerServices;

namespace JoltPhysicsSharp;

public static class Matrix4x4Extensions
{
    internal static Matrix FromJolt(this Mat4 matrix)
    {
        // Transpose the matrix due to the different row/column major layout
        return Matrix.Transpose(Unsafe.As<Mat4, Matrix>(ref matrix));
    }

    internal static Mat4 ToJolt(this Matrix matrix)
    {
        // Transpose the matrix due to the different row/column major layout
        matrix = Matrix.Transpose(matrix);
        return Unsafe.As<Matrix, Mat4>(ref matrix);
    }

    public static Vector4 GetColumn(in this Matrix matrix, int j)
    {
        return new(matrix[0, j], matrix[1, j], matrix[2, j], matrix[3, j]);
    }

    public static void SetColumn(ref this Matrix matrix, int j, Vector4 value)
    {
        matrix[0, j] = value.X;
        matrix[1, j] = value.Y;
        matrix[2, j] = value.Z;
        matrix[3, j] = value.W;
    }
}

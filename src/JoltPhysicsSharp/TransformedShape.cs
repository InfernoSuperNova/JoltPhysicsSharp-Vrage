// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VRageMath;

namespace JoltPhysicsSharp;

public unsafe record struct TransformedShape
{
    public TransformedShape(in Vector3 positionCOM, in Quaternion rotation, Shape shape, in BodyID bodyID)
    {
        ShapePositionCOM = positionCOM;
        ShapeRotation = rotation;
        Shape = shape;
        BodyID = bodyID;
    }

    public Vector3 ShapePositionCOM { get; set; }
    public Quaternion ShapeRotation { get; set; }
    public Shape Shape { get; }
    public BodyID BodyID { get; }

    public Vector3 ShapeScale { get; set; } = Vector3.One;

    public readonly Matrix CenterOfMassTransform => Matrix.CreateFromQuaternion(ShapeRotation) * Matrix.CreateTranslation(ShapePositionCOM);
    public readonly Matrix InverseCenterOfMassTransform
    {
        get => Matrix.Invert(CenterOfMassTransform);
    }

    public Matrix WorldTransform
    {
        readonly get
        {
            Matrix transform = Matrix.CreateScale(ShapeScale) * Matrix.CreateFromQuaternion(ShapeRotation);
            transform.Translation = ShapePositionCOM - Vector3.Transform(Shape.CenterOfMass, transform);
            return transform;
        }
        set
        {
            Vector3 scale = value.Scale;
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(value);
            Vector3 translation = value.Translation;
            SetWorldTransform(translation, rotation, scale);
        }
    }

    public void SetWorldTransform(in Vector3 position, in Quaternion rotation, in Vector3 scale)
    {
        ShapePositionCOM = Vector3.Transform(position, rotation) * (scale * Shape.CenterOfMass);
        ShapeRotation = rotation;
        ShapeScale = scale;
    }

    public readonly BoundingBox WorldSpaceBounds => Shape.GetWorldSpaceBounds(CenterOfMassTransform, ShapeScale);

    public readonly void GetWorldSpaceBounds(out BoundingBox bounds)
    {
        Shape.GetWorldSpaceBounds(CenterOfMassTransform, ShapeScale, out bounds);
    }

    public readonly void GetWorldSpaceSurfaceNormal(in SubShapeID subShapeID, in Vector3 position, out Vector3 normal)
    {
        // MakeSubShapeIDRelativeToShape?
        Matrix inv_com = InverseCenterOfMassTransform;
        Vector3 shapePosition = Vector3.Transform(position, inv_com) / ShapeScale;
        Shape.GetSurfaceNormal(subShapeID, in shapePosition, out normal);

        // return inv_com.Multiply3x3Transposed(mShape->GetSurfaceNormal(MakeSubShapeIDRelativeToShape(inSubShapeID), Vec3(inv_com * inPosition) / scale) / scale).Normalized();
        normal = Vector3.Normalize(Vector3.Transform(normal / ShapeScale, Matrix.Transpose(inv_com)));
    }

    public readonly Vector3 GetWorldSpaceSurfaceNormal(in SubShapeID subShapeID, in Vector3 position)
    {
        GetWorldSpaceSurfaceNormal(in subShapeID, in position, out Vector3 result);
        return result;
    }

    #region CastRay
    public readonly bool CastRay(in Ray ray, out RayCastResult hit)
    {
        // Transform the ray to local space, note that this drops precision which is possible because we're in local space now
        Ray newRay = Ray.Transform(in ray, InverseCenterOfMassTransform);

        // Scale the ray
        Vector3 inv_scale = new Vector3(1.0f) / ShapeScale;
        newRay.Position *= inv_scale;
        newRay.Direction *= inv_scale;

        // Cast the ray on the shape
        //SubShapeIDCreator sub_shape_id(mSubShapeIDCreator);
        if (Shape.CastRay(newRay, out hit))
        {
            // Set body ID on the hit result
            hit.BodyID = BodyID;

            return true;
        }

        return false;
    }

    public readonly bool CastRay(
        in Ray ray,
        RayCastSettings settings,
        CollisionCollectorType collectorType,
        ICollection<RayCastResult> results,
        ShapeFilter? shapeFilter = default)
    {
        if (shapeFilter != null)
        {
            shapeFilter.BodyID2 = BodyID;
        }

        // Transform the ray to local space, note that this drops precision which is possible because we're in local space now
        Ray newRay = Ray.Transform(in ray, InverseCenterOfMassTransform);

        // Scale the ray
        Vector3 inv_scale = new Vector3(1.0f) / ShapeScale;
        newRay.Position *= inv_scale;
        newRay.Direction *= inv_scale;

        return Shape.CastRay(in newRay, in settings, collectorType, results, shapeFilter);

    }
    #endregion

    public readonly bool CollidePoint(in Vector3 point, ShapeFilter? shapeFilter = default)
    {
        if (shapeFilter != null)
        {
            shapeFilter.BodyID2 = BodyID;
        }

        // Transform and scale the point to local space
        Vector3 shapePoint = Vector3.Transform(point, InverseCenterOfMassTransform) / ShapeScale;

        // Do point collide on the shape
        return Shape!.CollidePoint(in shapePoint, shapeFilter);
    }

    public readonly bool CollidePoint(in Vector3 point, CollisionCollectorType collectorType, ICollection<CollidePointResult> result, ShapeFilter? shapeFilter = default)
    {
        if (shapeFilter != null)
        {
            shapeFilter.BodyID2 = BodyID;
        }

        // Transform and scale the point to local space
        Vector3 shapePoint = Vector3.Transform(point, InverseCenterOfMassTransform) / ShapeScale;

        return Shape!.CollidePoint(in shapePoint, collectorType, result, shapeFilter);
    }
}

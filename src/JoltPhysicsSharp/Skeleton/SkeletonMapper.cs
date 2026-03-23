// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VRageMath;
using static JoltPhysicsSharp.JoltApi;

namespace JoltPhysicsSharp;

public class SkeletonMapper : NativeObject
{
    internal SkeletonMapper(nint handle, bool owns = true)
       : base(handle, owns)
    {
    }

    public SkeletonMapper()
        : base(JPH_SkeletonMapper_Create())
    {
    }

    protected override void DisposeNative()
    {
        JPH_SkeletonMapper_Destroy(Handle);
    }

    public void Initialize(Skeleton skeleton1, in Matrix neutralPose1, nint skeleton2, in Matrix neutralPose2)
    {
        JPH_SkeletonMapper_Initialize(Handle, skeleton1.Handle, neutralPose1, skeleton2, neutralPose2);
    }

    public void LockAllTranslations(Skeleton skeleton2, in Matrix neutralPose2)
    {
        JPH_SkeletonMapper_LockAllTranslations(Handle, skeleton2.Handle, neutralPose2);
    }

    public unsafe void LockTranslations(Skeleton skeleton2, Bool8* lockedTranslations, in Matrix neutralPose2)
    {
        JPH_SkeletonMapper_LockTranslations(Handle, skeleton2.Handle, lockedTranslations, neutralPose2);
    }

    public void Map(in Matrix pose1ModelSpace, in Matrix pose2LocalSpace, out Matrix outPose2ModelSpace)
    {
        JPH_SkeletonMapper_Map(Handle, pose1ModelSpace, pose2LocalSpace, out outPose2ModelSpace);
    }

    public Matrix Map(in Matrix pose1ModelSpace, in Matrix pose2LocalSpace)
    {
        JPH_SkeletonMapper_Map(Handle, pose1ModelSpace, pose2LocalSpace, out Matrix outPose2ModelSpace);
        return outPose2ModelSpace;
    }

    public void MapReverse(in Matrix pose2ModelSpace, out Matrix outPose1ModelSpace)
    {
        JPH_SkeletonMapper_MapReverse(Handle, pose2ModelSpace, out outPose1ModelSpace);
    }

    public Matrix MapReverse(in Matrix pose2ModelSpace)
    {
        JPH_SkeletonMapper_MapReverse(Handle, pose2ModelSpace, out Matrix outPose1ModelSpace);
        return outPose1ModelSpace;
    }

    public int GetMappedJointIndex(int joint1Index) => JPH_SkeletonMapper_GetMappedJointIndex(Handle, joint1Index);
    public bool IsJointTranslationLocked(int joint2Index) => JPH_SkeletonMapper_IsJointTranslationLocked(Handle, joint2Index);

    internal static SkeletonMapper? GetObject(nint handle) => GetOrAddObject(handle, h => new SkeletonMapper(h, false));
}

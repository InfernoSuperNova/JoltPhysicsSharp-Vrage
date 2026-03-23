// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using VRageMath;
using NUnit.Framework;

namespace JoltPhysicsSharp.Tests;

[TestFixture(TestOf = typeof(Constraint))]
public class ConstraintsTests : BaseTest
{
    [Test]
    public static void TestHingeConstraintSettings()
    {
        HingeConstraintSettings settings = new();
    }
}

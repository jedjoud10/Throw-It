using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

//https://answers.unity.com/questions/182209/checking-for-quaternion-values-to-not-be-nan.html
public static class UnityExtensionMethods
{
    /// <summary>
    /// Determines whether the quaternion is safe for interpolation or use with transform.rotation.
    /// </summary>
    /// <returns><c>false</c> if using the quaternion in Quaternion.Lerp() will result in an error (eg. NaN values or zero-length quaternion).</returns>
    /// <param name="quaternion">Quaternion.</param>
    public static bool IsValid(this Quaternion quaternion)
    {
        bool isNaN = float.IsNaN(quaternion.x + quaternion.y + quaternion.z + quaternion.w);

        bool isZero = quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0 && quaternion.w == 0;

        return !(isNaN || isZero);
    }
}
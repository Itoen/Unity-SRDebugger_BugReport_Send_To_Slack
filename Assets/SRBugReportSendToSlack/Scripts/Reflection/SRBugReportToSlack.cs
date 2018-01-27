using UnityEngine;
using System.Reflection;
using SRDebugger.Internal;
using SRDebugger.Services;
using System;

public class SRBugReportToSlack : MonoBehaviour
{

    // Use this for initialization
    void Start ()
    {
        unsafe
        {
            // コンストラクタ差し替え
            var defaultConstructorPointer = typeof(BugReportApi).GetConstructor(new Type[] { typeof(BugReport), typeof(string) }).MethodHandle.Value.ToPointer();
            var originalConstructorPointer = typeof(MyBugReportApi).GetConstructor(new Type[] { typeof(BugReport), typeof(string) }).MethodHandle.Value.ToPointer();
            *((int*)new IntPtr(((int*)defaultConstructorPointer + 1)).ToPointer()) = *((int*)new IntPtr(((int*)originalConstructorPointer + 1)).ToPointer());

            var defaultSubmitMethodPointer = typeof(BugReportApi).GetMethod("Submit", BindingFlags.Public | BindingFlags.Instance).MethodHandle.Value.ToPointer();
            var originalSubmitMethodPointer = typeof(MyBugReportApi).GetMethod("Submit", BindingFlags.Public | BindingFlags.Instance).MethodHandle.Value.ToPointer();
            *((int*)new IntPtr(((int*)defaultSubmitMethodPointer + 1)).ToPointer()) = *((int*)new IntPtr(((int*)originalSubmitMethodPointer + 1)).ToPointer());
        }

    }
}

using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

/// <summary>
/// 计算每次修改脚本时Unity所需的编译时间
/// </summary>
[InitializeOnLoad]
public static class CompileTimer
{
    private static double _startTime;

    static CompileTimer()
    {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationStarted(object obj)
    {
        _startTime = EditorApplication.timeSinceStartup;
    }

    private static void OnCompilationFinished(object obj)
    {
        double endTime = EditorApplication.timeSinceStartup;
        double duration = endTime - _startTime;

        Debug.Log($"编译耗时：{duration:F2} s");
    }
}

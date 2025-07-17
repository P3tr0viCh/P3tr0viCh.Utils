using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;

namespace P3tr0viCh.Utils
{
    public class DebugWrite
    {
        [Conditional("DEBUG")]
        public static void Line(object o, [CallerMemberName] string memberName = "")
        {
            Debug.WriteLine($"{memberName}: {o}");
        }

        [Conditional("DEBUG")]
        public static void Error(Exception e, [CallerMemberName] string memberName = "")
        {
            if (e == null) return;

            Error(e.Message, memberName);

            Error(e.InnerException, memberName);
        }

        [Conditional("DEBUG")]
        public static void Error(string err, [CallerMemberName] string memberName = "")
        {
            Debug.WriteLine($"{memberName} fail: {err}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace CaeGlobals
{
    public class CaeException : Exception
    {
        public CaeException()
        {
        }

        public CaeException(string message)
            : base(message)
        {
        }

        public CaeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class ExceptionTools
    {
        public static void Show(object obj, Exception ex)
        {
            MessageBoxes.ShowError(FormatException(obj, ex));
        }

        public static string FormatException(object obj, Exception ex)
        {
            StackFrame frame = new StackFrame(2);
            var method = frame.GetMethod();
            var objectName = method.DeclaringType.Name;
            var mathodName = method.Name;

            string message = "";
            string indent = "  ";

            if (ex is CaeException) message = ex.Message;
            else
            {
                message = "Message: " + ex.Message + Environment.NewLine + Environment.NewLine;
                message += "Exception in object: " + objectName + " calling method: " + method.Name + Environment.NewLine + Environment.NewLine;
                message += ParseStackTrace(ex.StackTrace, indent) + Environment.NewLine;
                                       //+ ObjectContentParser(o, indent + indent);
            } 
            return message + Environment.NewLine;
        }

        private static string ParseStackTrace(string stackTrace, string indent)
        {
            string[] rows = stackTrace.Split(new string[] {"\n","\r"}, StringSplitOptions.RemoveEmptyEntries);
            string[] parts;

            string message = "Stack trace:" + Environment.NewLine;
            for (int i = 0; i < rows.Length; i++)
            {
                parts = rows[i].Split(new string[] { " at ", " in " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    message += indent + parts[1] + Environment.NewLine;
            }

            return message;
        }

        private static string ObjectContentParser(object o, string indent)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            // get all fields of this class
            FieldInfo[] allFields = o.GetType().GetFields(bindingFlags);

            string objectContents = "Object contents:" + Environment.NewLine;
            string value;
            foreach (FieldInfo fieldInfo in allFields)
            {
                // get field values of this class instance
                if (fieldInfo.GetValue(o) == null) value = "null";
                else value = fieldInfo.GetValue(o).ToString();

                objectContents += indent + fieldInfo.Name + ": " + value + Environment.NewLine;
            }

            return objectContents;
        }
    }
}

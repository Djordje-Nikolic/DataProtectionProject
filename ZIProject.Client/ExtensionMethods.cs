using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIProject.Client
{
    static class ExtensionMethods
    {
        public static string GetFullMessage(this Exception e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(e.Message);

            Exception inner = e.InnerException;
            while (inner != null)
            {
                stringBuilder.Append(" -> " + inner.Message);
                inner = inner.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}

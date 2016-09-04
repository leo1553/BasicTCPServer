using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BasicTCPServer {
    [AttributeUsage(AttributeTargets.Method)]
    public class ReceiveAttribute: Attribute {
        public static Dictionary<ReceiveID, MethodInfo> all = new Dictionary<ReceiveID, MethodInfo>();

        public ReceiveID informationType;
        public ReceiveAttribute(ReceiveID informationType) {
            this.informationType = informationType;
        }

        public static void Register() {
            Assembly assembly = Assembly.GetAssembly(typeof(Program));

            if(assembly == null)
                Log.WriteError("Assembly = null");

            foreach(MethodInfo method in assembly.GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes(typeof(ReceiveAttribute), false).Length > 0))
                all.Add(((ReceiveAttribute)method.GetCustomAttribute(typeof(ReceiveAttribute), false)).informationType, method);

            Console.WriteLine(all.Count() + " handlers found and registered.");
        }
    }

    public enum ReceiveID {
        A
    }
}

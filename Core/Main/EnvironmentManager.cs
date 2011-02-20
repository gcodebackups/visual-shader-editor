﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Core.Environment;

namespace Core.Main
{
    public class EnvironmentManager
    {
        public static readonly string PLUGIN_DIRECTORY = ".";//"./Plugins";

        public class PluginDescription
        {
            internal PluginDescription(string name, Type t)
            {
                Name = name;
                Type = t;
            }

            public readonly string Name;
            public IEnvironment CreatePlugin()
            {
                return (IEnvironment)Type.Assembly.CreateInstance(Type.FullName);
            }
            internal readonly Type Type;

            public override string ToString()
            {
                return string.Format("Environment: \"{0}\" [{1}", Name, Type.FullName);
            }
        }

        public void LoadPlugins()
        {
            StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "Begin Loading Plugins\n");
            StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "CurrentDirectory:\"{0}\"\n", Directory.GetCurrentDirectory());
            StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "Plugin Dir:\"{0}\"\n", PLUGIN_DIRECTORY);

            string[] files = Directory.GetFiles(PLUGIN_DIRECTORY);

            foreach (var f in files)
            {
                //prepare file name
                string fullFileName = Path.Combine(Directory.GetCurrentDirectory(), f);
                StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "Loading File:\"{0}\"\n", fullFileName);

                //try load
                Type[] types;
                try
                {
                    Assembly a = Assembly.LoadFile(fullFileName);
                    types = a.GetExportedTypes();
                }
                catch (Exception e)
                {
                    StaticBase.Singleton.Log.Write(Log.InfoType.GlobalError, "Loading Fail:{0}\n", e.Message);
                    continue;
                }

                //find plugin class
                StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "Loading...Done\n");
                StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "Begin Scaning for plugins\n");
                foreach (Type t in types)
                {
                    EnvironmentAttribute atr = Attribute.GetCustomAttribute(t, typeof(EnvironmentAttribute)) as EnvironmentAttribute;

                    if (atr != null)
                    {
                        StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "Plugin Found:\"{0}\" Class:\"{1}\"\n", atr.Name, t.AssemblyQualifiedName);
                        m_plugins.Add(new PluginDescription(atr.Name, t));
                    }
                }
                StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "End Scaning for plugins\n");
            }

            StaticBase.Singleton.Log.Write(Log.InfoType.GlobalInfo, "End Loading Plugins\n");
        }

        public int PluginCount
        {
            get { return m_plugins.Count; }
        }

        public PluginDescription GetPluginDescription(int id)
        {
            return m_plugins[id];
        }

        readonly List<PluginDescription> m_plugins = new List<PluginDescription>();
    }
}

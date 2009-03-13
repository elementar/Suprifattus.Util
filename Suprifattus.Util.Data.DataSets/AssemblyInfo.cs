using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

[assembly: AssemblyTitle("Suprifattus® Framework Data Library - DataSets implementation")]
[assembly: AssemblyDescription("Suprifattus Framework")]
[assembly: AssemblyConfiguration("retail")]
[assembly: AssemblyCompany("Suprifattus Consultoria e Tecnologia")]
[assembly: AssemblyProduct("Suprifattus® Framework")]
[assembly: AssemblyCopyright("Copyright © 2005 Suprifattus Consultoria e Tecnologia")]

[assembly: AssemblyVersion("1.0.0.8")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

#if !GENERICS
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("../../../Suprifattus.Util.snk")]
[assembly: AssemblyKeyName("")]
#endif

[assembly: ReflectionPermission(SecurityAction.RequestMinimum)]

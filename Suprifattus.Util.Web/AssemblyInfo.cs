using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

[assembly: AssemblyTitle("Suprifattus® Framework Web Utilitary Library")]
[assembly: AssemblyDescription("Suprifattus Framework")]
[assembly: AssemblyConfiguration("retail")]
[assembly: AssemblyCompany("Suprifattus Consultoria e Tecnologia")]
[assembly: AssemblyProduct("Suprifattus® Framework")]
[assembly: AssemblyCopyright("Copyright © 2004-2006 Suprifattus Consultoria e Tecnologia")]

#if !GENERICS
[assembly: AssemblyVersion("1.5.0.0")]
#else
[assembly: AssemblyVersion("2.0.0.0")]
#endif
[assembly: AssemblyFileVersion("1.2.2.206")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

#if !GENERICS
[assembly: AssemblyKeyFile("../../../Suprifattus.Util.snk")]
#endif

[assembly: ReflectionPermission(SecurityAction.RequestMinimum)]

#if GENERICS
[assembly: AllowPartiallyTrustedCallers]
#endif

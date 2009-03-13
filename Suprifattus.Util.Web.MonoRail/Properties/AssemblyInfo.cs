using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly : AssemblyTitle("Suprifattus® Framework Web Utilitary Library - MonoRail specific")]
[assembly : AssemblyDescription("Suprifattus Framework")]
[assembly : AssemblyConfiguration("retail")]
[assembly : AssemblyCompany("Suprifattus Consultoria e Tecnologia")]
[assembly : AssemblyProduct("Suprifattus® Framework")]
[assembly : AssemblyCopyright("Copyright © 2006 Suprifattus Consultoria e Tecnologia")]
[assembly : ComVisible(false)]
[assembly : Guid("df9dd194-ae7b-4d40-b860-f730affa99c9")]

#if !GENERICS
[assembly : AssemblyVersion("1.5.0.0")]
#else
	[assembly: AssemblyVersion("2.0.0.0")]
#endif

[assembly : AssemblyFileVersion("1.0.0.8")]

#if GENERICS
[assembly: AllowPartiallyTrustedCallers]
#endif

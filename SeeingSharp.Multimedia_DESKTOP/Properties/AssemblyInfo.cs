using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SeeingSharp")]
[assembly: AssemblyDescription("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("761c1f6e-0eca-42b8-a9af-86d0570b9ce8")]

// Custom namespace mapping for wpf
[assembly: System.Windows.Markup.XmlnsDefinition("http://www.rolandk.de/wp/seeingsharp/multimedia", "SeeingSharp.Multimedia.Views")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://www.rolandk.de/wp/seeingsharp/multimedia", "SeeingSharp.Multimedia.Components")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://www.rolandk.de/wp/seeingsharp/multimedia", "ssharpMulti")]
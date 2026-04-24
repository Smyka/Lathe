using System;
using System.IO;
using System.Linq;

// Append closing braces to Form1.cs
string path = @"C:\Users\Alicia\Downloads\Lathe-0.1\OutlastTrayTool\Form1.cs";
File.AppendAllText(path, "\n                flowLayoutPanel5.Controls.Add(panel);\n            }\n        }\n    }\n}\n");
Console.WriteLine("Appended closing braces to Form1.cs");

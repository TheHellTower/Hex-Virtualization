using dnlib.DotNet;
using Hex.VM.Core.Protections;
using Hex.VM.Core.Protections.Impl.Virtualization;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using System.Collections.Generic;
using System.Linq;

namespace Hex.VM.Core
{
    public class Context
    {
        public static Context Instance { get; private set; }
        public ModuleDefMD Module { get; }
        public ModuleDefMD RTModule { get; }
        public ModuleContext ModuleContext { get; }
        public Importer Importer { get; }

        public TypeDef theType = null;
        public MethodDef theMethod = null;
        
        public List<IProtection> Protections { get; }        
        public List<string> VirtualizedMethods = new List<string>();

        public Logger Log { get; }
        
        public Context(string name)
        {
            Log = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
            
            Instance = this;
            ModuleContext = ModuleDef.CreateModuleContext();
            Module = ModuleDefMD.Load(name, ModuleContext);
            RTModule = ModuleDefMD.Load("Hex.VM.Runtime.dll");
            Importer = new Importer(Module);
            Protections = new List<IProtection>()
            {
                new Virtualization()
            };

            this.theType = Context.Instance.RTModule.Types.Where(t => t.FullName.Contains("VirtualMachine")).First(); //VirtualMachine
            this.theMethod = theType.Methods.Where(m => m.ReturnType.ToString().Contains("Object")).First(); //RunVM, in case other methods are added.
        }
    }
}
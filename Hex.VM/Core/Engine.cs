using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Hex.VM.Core.Protections;
using System;
using System.IO;

namespace Hex.VM.Core
{
    public class Engine
	{
		private Context Context { get; set; }

		public void Run(string[] args)
		{
			string filePath = Path.GetFullPath(args[0]);
			Context = new Context(filePath);
			
			try
			{
				foreach(IProtection Protection in Context.Protections)
				{
                    Context.Log.Information($"{Protection.Name()} phase..");
                    Protection.Execute(Context);
                }
            }
			catch (Exception exc)
			{
				Context.Log.Error($"Something went wrong while applying virtualization: {exc.Message}");
			}

			string This = "[https://t.me/TheHellTower_Group]";

            if (Context.Module.Kind == ModuleKind.Dll)
                Context.Instance.Module.GlobalType.Name = This;
			else if (Context.Module.Kind == ModuleKind.Windows || Context.Module.Kind == ModuleKind.Console)
                Context.Instance.Module.EntryPoint.Name = This;

            Save(filePath.Insert(filePath.Length - 4, "-HexVM"));
		}

		private void Save(string sp)
		{
			Console.WriteLine();
			try
			{
				var opts = new ModuleWriterOptions(Context.Module) { Logger = DummyLogger.NoThrowInstance };
				
				opts.MetadataOptions.Flags = MetadataFlags.PreserveAll;
				
				Context.Module.Write(sp, opts);
				
				if (File.Exists(sp))
					Context.Log.Information($"Obfuscated file saved as {sp}");
			}
			catch (Exception exc)
			{
                Context.Log.Fatal("Error, could not save: " + exc.Message);
			}

			Console.ReadKey();
		}
	}
}
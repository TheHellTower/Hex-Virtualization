using dnlib.DotNet;
using dnlib.DotNet.Writer;
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
			
			Context.Log.Information("Virtualization phase..");
			try
			{
				Context.Protections[0].Execute(Context);
			}
			catch (Exception exc)
			{
				Context.Log.Error($"Something went wrong while applying virtualization: {exc.Message}");
			}

			Context.Instance.Module.EntryPoint.Name = "[https://t.me/TheHellTower_Group]";

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
# [Hex Virtualization](https://github.com/TheHellTower/Hex-Virtualization) [![MIT license](https://img.shields.io/badge/License-MIT-blue.svg)](https://lbesson.mit-license.org/)
Developed by <a href="https://github.com/hexck">Hexk</a>
<br>

## :guardsman: Why do you need Hex Virtualization ? 

Hex-Virtualization was built so that people can learn from it, for real-world use, other obfuscation techniques such as anti-tampering, mutations, controlflow, and renamer would bring this vm to its full potential.

<a href="https://help.gapotchenko.com/eazfuscator.net/30/virtualization#Virtualization_Introduction"> Eazfuscator</a> describes perfectly what a vm does:<br>
_"Many of us consider particular pieces of code especially important. May it be a license code check algorithm implementation, an innovative optimization method, or anything else equally important so we would want to protect it by any means possible. As we know, the traditional obfuscation techniques basically do renaming of symbols and encryption, thus leaving the actual algorithms — cycles, conditional branches and arithmetics potentially naked to eye of the skilled intruder._

_Here a radical approach may be useful: to remove all the .NET bytecode instructions from an assembly, and replace it with something completely different and unknown to an external observer, but functionally equivalent to the original algorithm during runtime — this is what the code virtualization actually is."_

<br>

## :bug: Known "issues" ?

- Missing `ref/out` keyword support(`any method virtualized containing one will throw`)
- Missing EH (`try{}catch{}finally{}`) support.
- Handlers are not perfect.
- Performances hits up to 1,5 seconds with Runtime obfuscation in a resource needy/intensive speed-test.(`not really noticiable in standard tested programs.`)
- Xor
- Using a whole thing(`XXHash`) just for naming.
<br>

## :star: How does it work ?

- MSIL to VMIL
- Methods are stored as resources
- Bytes are encrypted with xor cipher
<br>

## :fire: What does it do ?

- [x] Virtualize code into instructions which only Hex.VM can understand
- [x] Support for a decent amount of opcodes, as said, this is made for educational purposes and as such I believe these opcodes are enough for people to learn and build on
- [x] Easy to use, understand, and build on

<br>

## 🎥 Preview

[View Video](https://youtu.be/hIUg9JYsdOk)
[![](https://i.imgur.com/LO6m8Ge.jpeg)](https://youtu.be/hIUg9JYsdOk)

## :bookmark_tabs: Examples

<details>
  <summary> <strong>Before</strong> </summary>
  
  
  ```cs
  using System;
using System.Runtime.CompilerServices;

namespace Hex.VM.Tests
{
	public class Maths
	{
		public int Sum { get; set; }
		
		public Maths(int x, int y)
		{
			this._x = x;
			this._y = y;
			this.Sum = this._x + this._y;
		}
		
		public int Add()
		{
			return this._x + this._y;
		}
		
		public int Subtract()
		{
			return this._x - this._y;
		}

		public int Multiply()
		{
			return this._x * this._y;
		}

		public int Divide()
		{
			return this._x / this._y;
		}
		
		private int <Sum>k__BackingField;
		
		private int _x;
		
		private int _y;
	}
}
```
</details>

<details>
  <summary> <strong>After</strong> </summary>
  
  
  ```cs
  using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hex.VM.Tests
{
	public class Maths
	{
		public int Sum
		{
			[CompilerGenerated]
			get
			{
				int num = calli(System.Int32(), ldftn({A586DBFE-F476-402B-B003-7E070D33AF8B}));
				object[] array = new object[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))];
				array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
				return (int)calli(System.Object(System.Int32,System.Object[],System.String), num, array, Encoding.UTF8.GetString(null.{30332EE9-2ADA-43EF-8657-A9D0C4A731C8}), ldftn({84254139-6195-4422-967F-5A70E84992B3}));
			}
			[CompilerGenerated]
			set
			{
				int num = calli(System.Int32(), ldftn({66A3A30C-3172-41A1-AE2A-C95779761AD7}));
				object[] array = new object[calli(System.Int32(), ldftn({928EABDE-0520-47F9-908A-449B147E475E}))];
				array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
				array[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))] = value;
				object obj = calli(System.Object(System.Int32,System.Object[],System.String), num, array, Encoding.UTF8.GetString(null.{EDF81B16-C478-45DD-B9F1-3563ABE3C2F5}), ldftn({84254139-6195-4422-967F-5A70E84992B3}));
			}
		}
		
		public Maths(int x, int y)
		{
			int num = calli(System.Int32(), ldftn({F40013D7-ECE4-4313-84D8-35C2147B46F3}));
			object[] array = new object[calli(System.Int32(), ldftn({E59CE633-867C-47F3-AC29-D82AC012BE94}))];
			array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
			array[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))] = x;
			array[calli(System.Int32(), ldftn({928EABDE-0520-47F9-908A-449B147E475E}))] = y;
			object obj = calli(System.Object(System.Int32,System.Object[],System.String), num, array, "100663299", ldftn({84254139-6195-4422-967F-5A70E84992B3}));
		}
		
		public int Add()
		{
			int num = calli(System.Int32(), ldftn({7D87F09E-E20F-457B-AF56-73F9878E8130}));
			object[] array = new object[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))];
			array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
			return (int)calli(System.Object(System.Int32,System.Object[],System.String), num, array, Encoding.UTF8.GetString(null.{2BD8B0CE-9AC1-4BB6-81EC-2828FC5D8C16}), ldftn({84254139-6195-4422-967F-5A70E84992B3}));
		}
		
		public int Subtract()
		{
			int num = calli(System.Int32(), ldftn({074AF9BD-EACA-443B-AC25-D36C361C9442}));
			object[] array = new object[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))];
			array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
			return (int)calli(System.Object(System.Int32,System.Object[],System.String), num, array, Encoding.UTF8.GetString(null.{8F28EAA7-6D42-4026-AC72-F55842D07D28}), ldftn({84254139-6195-4422-967F-5A70E84992B3}));
		}
		
		public int Multiply()
		{
			int num = calli(System.Int32(), ldftn({DFBFEB9C-F74F-4961-B8F2-13E6342A02DA}));
			object[] array = new object[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))];
			array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
			return (int)calli(System.Object(System.Int32,System.Object[],System.String), num, array, Encoding.UTF8.GetString(null.{49697F00-A6F0-414B-96DF-ABD6B3D7BCFF}), ldftn({84254139-6195-4422-967F-5A70E84992B3}));
		}
		
		public int Divide()
		{
			int num = calli(System.Int32(), ldftn({7887B604-26C8-46F1-9A31-076B3079417F}));
			object[] array = new object[calli(System.Int32(), ldftn({0BC6BDCA-B12C-431A-A06E-25AFD7485DD4}))];
			array[calli(System.Int32(), ldftn({1ACC3772-1D8F-4338-A7EE-19E65A64615B}))] = this;
			return (int)calli(System.Object(System.Int32,System.Object[],System.String), num, array, Encoding.UTF8.GetString(null.{85D9BB1A-3C37-4BE6-8D18-EA58A244D39D}), ldftn({84254139-6195-4422-967F-5A70E84992B3}));
		}
		//....
	}
}

```
</details>


## Resources
https://www.ecma-international.org/publications/files/ECMA-ST/ECMA-334.pdf <br>

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes?view=netframework-4.8

## Credits

<a href="https://github.com/hexck">Hexk</a> for <a href="https://github.com/hexck/Hex-Virtualization">HexVM(Hex-Virtualization)</a>
<br>
<a href="https://github.com/CursedLand">CursedLand</a> for <a href="https://github.com/CursedLand/ILVirtualization">ILVirtualization</a> | [Factory](https://github.com/CursedLand/ILVirtualization/blob/master/Runtime/Factory.cs) & [Cast](https://github.com/CursedLand/ILVirtualization/blob/master/Runtime/ILValue.cs#L27)
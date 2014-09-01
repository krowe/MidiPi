using System;
using Midi;
using System.Threading;
using System.IO;

namespace MidiExamples {
	class PiTest01 : PiBase {
		public PiTest01() : base("PiTest01.cs", "Pad the data space with the other octets.") { }

		public override void Run() {
			OutputDevice outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
			char inp;
			int dx;
			Pitch[] notes=new Pitch[10] { Pitch.F3, Pitch.A4, Pitch.B4, Pitch.C4, Pitch.D4, Pitch.E4, Pitch.F4, Pitch.G4, Pitch.A5, Pitch.B6 };
			if(outputDevice==null) {
				Console.WriteLine("\nNo output devices, so can't run this example.");
				ExampleUtil.PressAnyKeyToContinue();
				return;
			}
			if(!File.Exists(PiPath)) {
				Console.WriteLine("\nCould not find the data file: {0}", PiPath);
				ExampleUtil.PressAnyKeyToContinue();
				return;
			}
			Console.WriteLine("This version simply pads the extra data space with notes from adjacent octaves.\n");
			Console.WriteLine("This version many will say is the most accurately reproduction of Pi. That may be true but "+
				"at the same time every way of doing this involves a lot of fiddling with the numbers anyway. I wouldn't "+
				"be surprised if there are people out there who could listen to this and write down the digits. That "+
				"is probably not true of most of the other methods.\n\n");
			Console.WriteLine("The follow 10 notes will be used:");
			for(dx=0; dx<10 && Console.KeyAvailable==false; dx++) {
				Console.WriteLine("\tDigit {0} is represented by a {1} note.", dx, notes[dx].ToString());
			}
			Console.WriteLine("Interpreting Pi...Press any key to stop...\n\n");
			outputDevice.Open();
			// outputDevice.SendProgramChange(Channel.Channel1, Instrument.AltoSax); 
			outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
			try {
				using(StreamReader sr=new StreamReader(PiPath)) {
					while(sr.Peek()>=0 && Console.KeyAvailable==false) {
						inp=(char)sr.Read();
						if(Char.IsNumber(inp)) { // Skip over non numbers.
							Console.Write(inp);
							outputDevice.SendNoteOn(Channel.Channel1, notes[(int)Char.GetNumericValue(inp)], 80);
							Thread.Sleep(200);
							outputDevice.SendNoteOff(Channel.Channel1, notes[(int)Char.GetNumericValue(inp)], 80);
							Thread.Sleep(100);
						}
					}
				}
			} catch(FieldAccessException e) {
				Console.WriteLine("\nError: Could not access file {0}\n\nThe exception was: {1}\n", PiPath, e.Message);
			} catch(Exception e) {
				Console.WriteLine("\nError: {1}\n", PiPath, e.Message);
			}
			outputDevice.Close();
			while(Console.KeyAvailable) {Console.ReadKey(false);}
			Console.WriteLine();
			ExampleUtil.PressAnyKeyToContinue();
		}
	}
}

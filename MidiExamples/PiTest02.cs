using System;
using Midi;
using System.Threading;
using System.IO;

namespace MidiExamples {
	class PiTest02 : PiBase {
		public PiTest02() : base("PiTest02.cs", "Pi with drums and piano.") { }

		public override void Run() {
			OutputDevice outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
			char inp;
			int dx;
			Pitch[] notes=new Pitch[7] { Pitch.A4, Pitch.B4, Pitch.C4, Pitch.D4, Pitch.E4, Pitch.F4, Pitch.G4 };
			Percussion[] drums=new Percussion[3] { Percussion.BassDrum1, Percussion.MidTom1, Percussion.CrashCymbal1 };
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
			Console.WriteLine("This didn't turn out well at all. A back beat may help it but I'm moving on for now.\n\n");
			Console.WriteLine("The follow 10 notes will be used:");
			for(dx=0; dx<10 && Console.KeyAvailable==false; dx++) {
				if(dx>=notes.Length) {
					Console.WriteLine("\tDigit {0} is represented by a {1} drum.", dx, drums[dx-notes.Length].ToString());
				} else {
					Console.WriteLine("\tDigit {0} is represented by a {1} note.", dx, notes[dx].ToString());
				}
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
							dx=(int)Char.GetNumericValue(inp);
							if(dx>=notes.Length) outputDevice.SendPercussion(drums[dx-notes.Length], 90);
							else outputDevice.SendNoteOn(Channel.Channel1, notes[dx], 80);
							Thread.Sleep(500);
						}
					}
				}
			} catch(FieldAccessException e) {
				Console.WriteLine("\nError: Could not access file {0}\n\nThe exception was: {1}\n", PiPath, e.Message);
			} catch(Exception e) {
				Console.WriteLine("\nError: {1}\n", PiPath, e.Message);
			}
			outputDevice.Close();
			while(Console.KeyAvailable) { Console.ReadKey(false); }
			Console.WriteLine();
			ExampleUtil.PressAnyKeyToContinue();
		}
	}
}

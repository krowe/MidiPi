using System;
using Midi;
using System.Threading;
using System.IO;

namespace MidiExamples {
	class PiTest03 : PiBase {
		int[] _odds=new int[] {
			25, 11, 9,
			8, 7, 7,
			6, 6, 4,
			3, 2, 2
		};
		Pitch[] _notes=new Pitch[] { 
			Pitch.C4, Pitch.G4, Pitch.DSharp4, 
			Pitch.F4, Pitch.D4, Pitch.A4, 
			Pitch.E4, Pitch.CSharp4, Pitch.ASharp4, 
			Pitch.GSharp4, Pitch.B4, Pitch.FSharp4
		};

		public PiTest03() : base("PiTest03.cs", "Note dispersion model.") { }
		public override void Run() {
			OutputDevice outputDevice=ExampleUtil.ChooseOutputDeviceFromConsole();
			Pitch note;
			int read_val, rest_odds=0;
			if(outputDevice==null) {
				Console.WriteLine("\nERROR: No output devices, so can't run this example.");
				ExampleUtil.PressAnyKeyToContinue();
				return;
			}
			if(!File.Exists(PiPath)) {
				Console.WriteLine("\nERROR: Could not find the data file: {0}", PiPath);
				ExampleUtil.PressAnyKeyToContinue();
				return;
			}
			if(_odds.Length!=_notes.Length) {
				Console.WriteLine("\nERROR: _odds and _notes should have the same number of values.");
				ExampleUtil.PressAnyKeyToContinue();
				return;
			}
			Console.WriteLine("This is the most basic dispersion test I could think of. If you don't like "+
				"the rests then you can increase the total of _odds[] to something >= 100.\n");
			Console.WriteLine("You will notice from the output that this is reading two digits at a time "+
				"instead of just one. This gives us a number between 00-99 which is handy because we are "+
				"working with percentages. Below is a table of what the input will result in:\n\n");

			for(int dx=0; dx<_odds.Length; dx++) {
				rest_odds+=_odds[dx];
				Console.WriteLine("\t{0} numbers less than {1} represented by a {2} note.",
					dx==0?"All":"Remaining", rest_odds, _notes[dx].ToString());
			}
			Console.WriteLine("\tRemaining rest odds are {0}%.\n\n", 100-rest_odds);
			Console.WriteLine("Interpreting Pi...Press any key to stop...\n\n");

			outputDevice.Open();
			// outputDevice.SendProgramChange(Channel.Channel1, Instrument.AltoSax); 
			outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
			try {
				using(StreamReader sr=new StreamReader(PiPath)) {
					while((read_val=GetStreamDigits(sr))>=0 && Console.KeyAvailable==false) {
						Console.Write("{0:D2}", read_val);
						if(read_val<rest_odds) {
							if((note=GetNotePitch(read_val))==Pitch.ANeg1) {
								Console.WriteLine("\nError: Could not find an appropriate note for: {0}\n", read_val);
								continue;
							}
							outputDevice.SendNoteOn(Channel.Channel1, note, 80);
							Thread.Sleep(200);
							outputDevice.SendNoteOff(Channel.Channel1, note, 80);
						} else Thread.Sleep(200);						
						Thread.Sleep(100);
					}
				}
			} catch(FieldAccessException e) {
				Console.WriteLine("\nError: Could not access file {0}\n\nThe exception was: {1}\n", PiPath, e.Message);
			} catch(Exception e) {
				Console.WriteLine("\nError: {1}\n", PiPath, e.Message);
			}
			outputDevice.Close();
			while(Console.KeyAvailable) Console.ReadKey(false);
			Console.WriteLine();
			ExampleUtil.PressAnyKeyToContinue();
		}


		public Pitch GetNotePitch(int percentile) {
			for(int dx=0, sumOdds=_odds[dx]; dx<_odds.Length; dx++, sumOdds+=_odds[dx])
				if(percentile<=sumOdds) return _notes[dx];
			return Pitch.ANeg1;
		}
	}
}

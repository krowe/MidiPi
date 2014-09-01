using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace MidiExamples {
	/// PiBase Class
	/// <remarks>NoDescriptionGiven.</remarks>
	abstract class PiBase : ExampleBase {
		#region Public Methods
		/// <summary>The default constructor.</summary>
		public PiBase(string fileName, string description) : base(fileName, description) {}
		public string PiPath { get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\pi.txt"; } }
		public int GetStreamDigits(StreamReader sr, int count=2) {
			char inp; string nums="";
			if(count<1||sr==null||sr.Peek()<0) return -1;
			while(nums.Length<count && sr.Peek()>=0) {
				do { inp=(char)sr.Read(); } while(!Char.IsDigit(inp) && sr.Peek()>=0);
				if(Char.IsDigit(inp)) nums+=inp;
			}
			return nums.Length>0?int.Parse(nums):-1;
		}
		#endregion
	} // End PiBase Class
}

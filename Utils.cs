using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SCTIGR
{
	public class Utils
	{
		public static string[] Shotgun(string sequence, int shots, int length, int deviation)
		{
			var random = new Random();
			var shotsArray = new string[shots];
			
			for (int i = 0; i < shots; ++i)
			{
				int shotLength = random.Next(length - deviation, length + deviation);
				int shotBegin = random.Next(0, sequence.Length - shotLength);
				shotsArray[i] = sequence.Substring(shotBegin, shotLength);
			}
			
			return shotsArray;
		}
		
		public static string RandomSequence(int length)
		{
			var sb = new StringBuilder(length);
			var random = new Random();
			var letters = new [] {'A', 'T', 'G', 'C'};
			
			for (int i = 0; i < length; ++i)
			{
				sb.Append(letters[random.Next(4)]);
			}
			
			return sb.ToString();
		}
		
		public static string RepeatChar(char c, int num)
		{
			return string.Concat(Enumerable.Repeat(c, num));
		}
		
		public static string[] ReadFasta(TextReader reader) 
		{
			var sequences = new List<string>();
			StringBuilder sb = null;
			var regex = new Regex("^[ATCG]+$");
			
			string line;
			while ((line = reader.ReadLine()) != null) 
			{
				if (line.Length == 0) continue;
				
				if (line[0] == '>')
				{
					if (sb != null)
					{
						sequences.Add(sb.ToString());
					}
					
					sb = new StringBuilder();
				}
				else 
				{
					if (regex.IsMatch(line))
					{
						if (sb == null)
						{
							throw new FormatException();
						}
						
						sb.Append(line);
					}
					else 
					{
						throw new FormatException();
					}
				}
			}
			
			if (sb != null)
			{
				sequences.Add(sb.ToString());
			}
			
			return sequences.ToArray();
		}
	}
}


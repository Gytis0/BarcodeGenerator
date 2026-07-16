using System.Text;
using System.Text.RegularExpressions;

namespace Core;

public static class SequenceHelper
{
	public static bool IsValid(string sequence)
	{
		if (string.IsNullOrWhiteSpace(sequence)) return false;

		sequence = sequence.ToUpper();

		for (int i = 0; i < sequence.Length; i++)
			if (sequence[i] != 'A' && sequence[i] != 'C' && sequence[i] != 'G' && sequence[i] != 'T')
				return false;

		return true;
	}

	public static string ExtractSequenceFromSnapGene(string path)
	{
		string text = Encoding.ASCII.GetString(File.ReadAllBytes(path));

		Match match = Regex.Match(text, "[ACGT]{20,}", RegexOptions.IgnoreCase);

		if (!match.Success)
			throw new InvalidDataException("No DNA sequence found.");

		return match.Value;
	}

	public static string ExtractSequenceFromGenBank(string path)
	{
		string text = File.ReadAllText(path);

		int start = text.IndexOf("ORIGIN", StringComparison.OrdinalIgnoreCase);
		if (start < 0)
			throw new InvalidDataException("ORIGIN section not found.");

		int end = text.IndexOf("//", start, StringComparison.Ordinal);
		if (end < 0)
			end = text.Length;

		StringBuilder sequence = new();

		foreach (char c in text.AsSpan(start + "ORIGIN".Length, end - start - "ORIGIN".Length))
		{
			switch (char.ToUpperInvariant(c))
			{
				case 'A':
				case 'C':
				case 'G':
				case 'T':
				case 'a':
				case 'c':
				case 'g':
				case 't':
					sequence.Append(char.ToUpperInvariant(c));
					break;
			}
		}

		return sequence.ToString();
	}

	public static string ExtractSequenceFromFasta(string path)
	{
		StringBuilder sequence = new();

		foreach (string line in File.ReadLines(path))
		{
			if (line.StartsWith(">"))
				continue;

			foreach (char c in line)
			{
				switch (char.ToUpperInvariant(c))
				{
					case 'A':
					case 'C':
					case 'G':
					case 'T':
					case 'a':
					case 'c':
					case 'g':
					case 't':
						sequence.Append(char.ToUpperInvariant(c));
						break;
				}
			}
		}

		return sequence.ToString();
	}

	public static string ExtractSequence(string path)
	{
		string extension = Path.GetExtension(path).ToLowerInvariant();

		string sequence = extension switch
		{
			".dna" => ExtractSequenceFromSnapGene(path),
			".gb" or ".gbk" or ".genbank" => ExtractSequenceFromGenBank(path),
			".fa" or ".fasta" or ".fna" => ExtractSequenceFromFasta(path),
			".txt" => File.ReadAllText(path).Trim(),
			_ => throw new NotSupportedException(string.Format("Unsupported file type: {0}", extension))
		};

		if(!IsValid(sequence))
			throw new InvalidDataException(string.Format("The extracted sequence in file [{0}] is not valid.", path));

		return sequence;
	}
}

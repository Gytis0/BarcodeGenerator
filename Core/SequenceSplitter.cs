namespace Core;

public static class SequenceSplitter
{
	public static (List<string> output, HashSet<string> existingEndings) SplitSequence_Bipartite_OffsetBy4(string sequence, int parts, int maxLengthDifference, HashSet<string>? existingEndings = null)
	{
		const int basesCount = 4;

		if (string.IsNullOrEmpty(sequence) || parts <= 0 || sequence.Length < parts * basesCount)
			throw new ArgumentException();

		List<int> idealBoundaries = new() { 0 };

		int firstExtra = basesCount;
		int remainingLength = sequence.Length - firstExtra;
		int normalPartLength = remainingLength / parts;
		int remainder = remainingLength % parts;

		int position = 0;
		for (int i = 0; i < parts; i++)
		{
			int partLength = normalPartLength + (i < remainder ? 1 : 0);
			if (i == 0) partLength += firstExtra;
			position += partLength;
			idealBoundaries.Add(position);
		}
		idealBoundaries[^1] = sequence.Length;

		int internalCount = parts - 1; // boundaries between fragments (excludes 0 and sequence.Length)

		var reserved = new HashSet<string>(existingEndings ?? Enumerable.Empty<string>());

		// The very last fragment's ending is fixed (it's just the tail of the sequence).
		string lastEnding = sequence.Substring(sequence.Length - basesCount, basesCount);
		if (reserved.Contains(lastEnding))
			throw new InvalidOperationException($"Unable to find unique ending for part {parts}.");
		reserved.Add(lastEnding); // reserve it so no internal boundary can collide with it

		var candidatesPerBoundary = new List<List<(int position, string ending)>>();

		for (int b = 1; b < idealBoundaries.Count - 1; b++)
		{
			int ideal = idealBoundaries[b];
			int prevIdeal = idealBoundaries[b - 1];
			int nextIdeal = idealBoundaries[b + 1];

			var seenEndings = new HashSet<string>();
			var list = new List<(int, string)>();

			for (int distance = 0; distance <= maxLengthDifference; distance++)
			{
				foreach (int offset in distance == 0 ? new[] { 0 } : new[] { -distance, distance })
				{
					int candidate = ideal + offset;

					if (candidate < prevIdeal + basesCount) continue;
					if (candidate > nextIdeal - basesCount) continue;
					if (candidate < basesCount || candidate > sequence.Length - basesCount) continue;

					string ending = sequence.Substring(candidate - basesCount, basesCount);

					if (reserved.Contains(ending)) continue;   // already used elsewhere (existingEndings/last)
					if (!seenEndings.Add(ending)) continue;    // keep only the closest position per ending

					list.Add((candidate, ending));
				}
			}

			candidatesPerBoundary.Add(list);
		}

		var matchedPositionForBoundary = new int[internalCount];
		var matchedEndingForBoundary = new string?[internalCount];
		var endingOwner = new Dictionary<string, int>();

		bool TryAugment(int b, HashSet<string> visited)
		{
			foreach (var (pos, ending) in candidatesPerBoundary[b])
			{
				if (!visited.Add(ending)) continue;

				if (!endingOwner.TryGetValue(ending, out int owner) || TryAugment(owner, visited))
				{
					endingOwner[ending] = b;
					matchedEndingForBoundary[b] = ending;
					matchedPositionForBoundary[b] = pos;
					return true;
				}
			}
			return false;
		}

		for (int b = 0; b < internalCount; b++)
		{
			var visited = new HashSet<string>();
			if (!TryAugment(b, visited))
				throw new InvalidOperationException($"Unable to find unique ending for part {b + 1}.");
		}

		List<int> boundaries = new() { 0 };
		for (int b = 0; b < internalCount; b++)
			boundaries.Add(matchedPositionForBoundary[b]);
		boundaries.Add(sequence.Length);

		var finalEndings = new HashSet<string>(existingEndings ?? Enumerable.Empty<string>());
		foreach (var e in matchedEndingForBoundary) finalEndings.Add(e!);
		finalEndings.Add(lastEnding);

		List<string> result = new();
		for (int i = 0; i < parts; i++)
			result.Add(sequence.Substring(boundaries[i], boundaries[i + 1] - boundaries[i]));

		return (result, finalEndings);
	}

	public static List<string> ManuallyAddBases(List<string> sequences)
	{
		string lastEnding = sequences[0].Substring(sequences[0].Length - 4, 4);
		for (int i = 1; i < sequences.Count; i++)
		{
			sequences[i] = string.Format("{0}{1}", lastEnding, sequences[i]);
			lastEnding = sequences[i].Substring(sequences[i].Length - 4, 4);
		}

		return sequences;
	}
}

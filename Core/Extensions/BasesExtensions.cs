using System.Text;

namespace Core.Extensions;

internal static class BasesExtensions
{
	public static char Complement(this char bases)
	{
		if (bases == 'C') return 'G';
		if (bases == 'G') return 'C';
		if (bases == 'T') return 'A';
		if (bases == 'A') return 'T';
		return default;
	}

	public static string Complement(this string bases)
	{
		StringBuilder sb = new();

		for (var i = 0; i < bases.Length; i++)
			sb.Append(bases[i].Complement());

		return sb.ToString();
	}
}

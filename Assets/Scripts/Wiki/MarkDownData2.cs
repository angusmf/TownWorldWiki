namespace TownWorldWiki
{
	public class MarkdownData2
	{
		public MarkdownData2(string data = null)
		{
			if (data != null) markdownData1 = data;
		}

		public string Data { get { return markdownData1; } }

		private string markdownData1 = @"

* [Tubing](./Activities/Tubing.markdown)
* [Spider-Silk Highways](./Activities/Spider_Silk_Highways.markdown)
* [Shooting Star Dodging and Collecting](./Activities/Shooting_Star_Dodging_and_Collecting.markdown)
  
";
	}
}

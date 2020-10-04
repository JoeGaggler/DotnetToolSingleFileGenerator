using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Jmg.VsixProject.Yaml
{
	class Spec
	{
		[YamlMember(Alias = "files")]
		public List<File> Files { get; set; }
	}

	class File
	{
		[YamlMember(Alias = "file")]
		public String FileName { get; set; }

		[YamlMember(Alias = "tool")]
		public String Tool { get; set; }

		[YamlMember(Alias = "extension")]
		public String Extension { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVPacker {
	interface IStep {
		bool IsReady { get; }
	}
}

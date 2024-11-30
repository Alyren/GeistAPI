using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistTools.Tweaks.Core.Tweaks
{
    internal interface ITweak
    {
        string Name { get; }
        void Awake(TweakLoadContainer container);
    }
}

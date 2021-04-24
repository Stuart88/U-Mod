using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace U_Mod.Custom
{
    public interface ICustomVideoControl
    {
        public MediaElement VideoPlayer  { get;set;}
        public bool VideoPlaying { get; set; }
        void SetOverlayBackgroundBlack();
    }
}

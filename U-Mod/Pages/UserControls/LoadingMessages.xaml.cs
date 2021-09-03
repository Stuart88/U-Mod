using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace U_Mod.Pages.UserControls
{
    /// <summary>
    /// Interaction logic for LoadingMessages.xaml
    /// </summary>
    public partial class LoadingMessages : TextBlock
    {
        private Timer Timer { get; set; } = new Timer() { Interval = 4000 };
        private Random Rand { get; set; } = new Random();

        private string[] FalloutMessges = new string[]
        {
            "Clearing roach infestations",
            "Decontaminating vault",
            "Bombing Megaton",
            "Grabbing Lincoln's Repeater",
            "Wasting Capital Wasteland",
            "Making the Grisly Diner a bit more grisly",
            "Making the sewers tepid",
            "Adding Strontium to Nuka-Cola",
            "Abandoning the Abandoned Camp",
            "Curing some ant meat",
            "Adding alpha particles to Mississippi quantum pie",
            "Accidentally irradiating RadAway",
            "Diluting stimpacks",
            "Stiching jumpsuits",
            "Drawing schematics",
            "Priming Rock-It Launcher",
            "Freeing Anchorage",
            "Feeding Dogmeat",
            "Opening Vault",
            "Making bread sandwiches",
            "Pissing off Mirelurks",
            "Readying V.A.T.S",
            "Inhaling jet violently"
        };

        private string[] NewVegasMessages = new string[]
       {
         // TODO  
       };

        private string[] OblivionMessages = new string[]
        {
            "Performing the ritual of purification",
            "Recruiting members of the Imperial Legion",
            "Hiring Moth Priests to read the Elder Scrolls",
            "Populating Oblivion with Daedra",
            "Forging the Ring of Burden",
            "Buying a Blackwood Ring for everybody",
            "Populating Kvatch",
            "Preparing Daedric Invasion",
            "Outfitting Mythic Dawn Agents"
        };

        public LoadingMessages()
        {
            Timer.Tick += CycleMessageText;

            InitializeComponent();
        }

        private void CycleMessageText(object s, EventArgs e)
        {
            MessageText.Text = Static.StaticData.CurrentGame switch
            {
                Shared.Enums.GamesEnum.Oblivion => OblivionMessages[Rand.Next(OblivionMessages.Length)],
                Shared.Enums.GamesEnum.Fallout => FalloutMessges[Rand.Next(FalloutMessges.Length)],
                Shared.Enums.GamesEnum.NewVegas => NewVegasMessages[Rand.Next(NewVegasMessages.Length)], 
                Shared.Enums.GamesEnum.Unknown => throw new NotImplementedException(),
                Shared.Enums.GamesEnum.None => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            } + "...";
        }

        public void StartCyclingMessages()
        {
            Timer.Start();
        }

        public void StopCyclingMessages()
        {
            Timer.Stop();
        }
    }
}

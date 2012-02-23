using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PartyClient
{
    public partial class PartyClientForm : UserControl
    {
        protected App app
        {
            get
            {
                return (App)Application.Current;
            }
        }

        protected WebClient webClient;

        public PartyClientForm()
        {
            InitializeComponent();
            System.Windows.Controls.Theming.ShinyBlueTheme.SetIsApplicationTheme(Application.Current, true);
       }
    }
}

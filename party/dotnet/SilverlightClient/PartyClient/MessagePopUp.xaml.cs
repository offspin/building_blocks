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
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PartyClient
{
    public partial class MessagePopUp : ChildWindow
    {
        public enum MessagePopUpType
        {
            OK, OKCancel, YesNo, YesNoCancel
        }

        public enum MessagePopUpResult
        {
            OK, Cancel, Yes, No, Undefined
        }

        private MessagePopUpType popUpType = MessagePopUpType.OK;
        private MessagePopUpResult popUpResult = MessagePopUpResult.Undefined;

        public MessagePopUpResult Result
        {
            get
            {
                return this.popUpResult;
            }
        }

        public MessagePopUp()
        {
            InitializeComponent();
            SetButtons();
        }

        public MessagePopUp(string message, MessagePopUpType popUpType)
        {
            InitializeComponent();
            this.popUpType = popUpType;
            this.messageTextBlock.Text = message;
            SetButtons();
   
        }

        private void SetButtons()
        {
            List<Button> buttons = null;

            foreach (Control c in ButtonPanel.Children)
            {
                if (c is Button)
                {
                    c.Visibility = Visibility.Collapsed;
                }
            }
            
            switch (popUpType)
            {
                case MessagePopUpType.OK:
                    buttons = new List<Button> {OKButton};
                    break;
                case MessagePopUpType.OKCancel:
                    buttons = new List<Button> {OKButton, CancelButton};
                    break;
                case MessagePopUpType.YesNo:
                    buttons = new List<Button> {YesButton, NoButton};
                    break;
                case MessagePopUpType.YesNoCancel:
                    buttons = new List<Button> {YesButton, NoButton, CancelButton};
                    break;
            }

            foreach (Button b in buttons)
            {
                b.Visibility = Visibility.Visible;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.popUpResult = MessagePopUpResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.popUpResult = MessagePopUpResult.Cancel;
            this.Close();
        }
        
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.popUpResult = MessagePopUpResult.Yes;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.popUpResult = MessagePopUpResult.No;
            this.Close();
        }
    }
}


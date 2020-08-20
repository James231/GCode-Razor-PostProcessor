using GCodeRazor.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GCodeRazor.UserControls
{
    /// <summary>
    /// Interaction logic for ExCard.xaml
    /// </summary>
    public partial class ExCard : UserControl
    {
        private Settings settings;

        public ExCard(CardControlModel model, Settings appSettings)
        {
            InitializeComponent();
            settings = appSettings;

            Card.Margin = new Thickness(0, model.MarginAbove, 0, model.MarginBelow);
            AddChildren(model.Content);
        }

        public void AddChildren(IExampleControlModel[] children)
        {
            if (children.Length == 0)
            {
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                switch(children[i].Type)
                {
                    case "heading":
                        Parent.Children.Add(new ExHeading((HeadingControlModel)children[i]));
                        break;
                    case "subheading":
                        Parent.Children.Add(new ExSubheading((SubHeadingControlModel)children[i]));
                        break;
                    case "label":
                        Parent.Children.Add(new ExLabel((LabelControlModel)children[i]));
                        break;
                    case "code":
                        Parent.Children.Add(new ExCode((CodeControlModel)children[i], settings));
                        break;
                    case "card":
                        Parent.Children.Add(new ExCard((CardControlModel)children[i], settings));
                        break;
                }
            }
        }
    }
}

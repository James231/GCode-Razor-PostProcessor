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
    /// Interaction logic for ExHeading.xaml
    /// </summary>
    public partial class ExHeading : UserControl
    {
        public ExHeading(HeadingControlModel model)
        {
            InitializeComponent();

            Heading.Margin = new Thickness(0, model.MarginAbove, 0, model.MarginBelow);
            Heading.Content = model.Content;
        }
    }
}

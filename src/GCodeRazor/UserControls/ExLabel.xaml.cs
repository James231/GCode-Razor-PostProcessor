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
    /// Interaction logic for ExLabel.xaml
    /// </summary>
    public partial class ExLabel : UserControl
    {
        public ExLabel(LabelControlModel model)
        {
            InitializeComponent();

            Label.Margin = new Thickness(5, model.MarginAbove, 5, model.MarginBelow);
            Label.Text = model.Content;
        }
    }
}

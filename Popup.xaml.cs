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
using System.Windows.Shapes;

namespace Epidemic_Models {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Popup : Window {
        public Popup() {
            InitializeComponent();
        }
        public event EventHandler CancelHappened;

        private void Cancel_Button_Click(object sender, RoutedEventArgs e) {
            if (CancelHappened != null) {
                CancelHappened(this, e);
            }          
        }

    }
}

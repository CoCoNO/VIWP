using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoyIteso.Class
{
    class Progress
    {
        public ProgressIndicator progressIndicator;

        public Progress()
        {
            progressIndicator = new ProgressIndicator();
        }

        public void showProgressIndicator(System.Windows.DependencyObject searchPage, string message)
        {
            progressIndicator.Text = message;
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;
            SystemTray.SetProgressIndicator(searchPage, progressIndicator);
        }

        public void hideProgressIndicator(System.Windows.DependencyObject searchPage)
        {
            progressIndicator.IsVisible = false;
            SystemTray.SetProgressIndicator(searchPage, progressIndicator);
        }
    }
}

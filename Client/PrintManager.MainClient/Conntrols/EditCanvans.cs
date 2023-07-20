using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PrintManager.MainClient.Conntrols
{
    public class EditCanvans : Canvas
    {
        public EditCanvans()
        {

        }


        // UIElement > Visu > Other components
        public void AddVisual(UIElement uIElement)
        {
            //base.AddLogicalChild(control);
            this.Children.Add(uIElement);
        }

        public void DeleteVisual(UIElement uIElement)
        {
            base.RemoveVisualChild(uIElement);
        }

        public void DeletAllVisual()
        {
            base.Children.Clear();
        }


    }
}

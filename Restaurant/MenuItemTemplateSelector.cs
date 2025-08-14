using System.Windows;
using System.Windows.Controls;
using Restaurant.Model;

namespace Restaurant
{
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProductTemplate { get; set; }
        public DataTemplate BundleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Product) return ProductTemplate;
            if (item is Bundle) return BundleTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}

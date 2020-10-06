using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TCC;
using TeraDataLite;

namespace TeraTinder
{
    public class NullToVisColl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RaceToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Race r))
            {
                return null;
            }

            var filename = r switch
            {
                Race.HumanMale => "human_male",
                Race.HumanFemale => "human_female",
                Race.ElfMale => "elf_male",
                Race.ElfFemale => "elf_female",
                Race.AmanMale => "aman_male",
                Race.AmanFemale => "aman_female",
                Race.CastanicMale => "castanic_male",
                Race.CastanicFemale => "castanic_female",
                Race.Popori => "popori_male",
                Race.Elin => "popori_female",
                Race.Baraka => "baraka",
                _ => ""
            };

            return Path.Combine(App.ResourcesPath, $"images/paperdoll/{filename}.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringLenToVisColl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RateLevelToOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rate = (double)value;
            if (parameter != null && parameter.ToString().IndexOf("like") != -1)
            {
                if (rate > 0) return rate;
            }
            else if (parameter != null && parameter.ToString().IndexOf("nope") != -1)
            {

                if (rate < 0) return -rate;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class TinderCard : UserControl
    {
        public TinderCard()
        {
            InitializeComponent();
        }



        public TinderWindow.Status Rate
        {
            get => (TinderWindow.Status)GetValue(RateProperty);
            set => SetValue(RateProperty, value);
        }

        public static readonly DependencyProperty RateProperty = DependencyProperty.Register("Rate", typeof(TinderWindow.Status), typeof(TinderCard));

        public double RateLevel
        {
            get => (double)GetValue(RateLevelProperty);
            set => SetValue(RateLevelProperty, value);
        }

        public static readonly DependencyProperty RateLevelProperty = DependencyProperty.Register("RateLevel", typeof(double), typeof(TinderCard), new PropertyMetadata(0d));



    }
}

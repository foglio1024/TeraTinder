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
            string gender = "";
            string race = "";
            switch ((Race)value)
            {
                case Race.HumanMale:
                    gender = "male";
                    race = "human";
                    break;
                case Race.HumanFemale:
                    gender = "female";
                    race = "human";
                    break;
                case Race.ElfMale:
                    gender = "male";
                    race = "elf";
                    break;
                case Race.ElfFemale:
                    gender = "female";
                    race = "elf";
                    break;
                case Race.AmanMale:
                    gender = "male";
                    race = "aman";
                    break;
                case Race.AmanFemale:
                    gender = "female";
                    race = "aman";
                    break;
                case Race.CastanicMale:
                    gender = "male";
                    race = "castanic";
                    break;
                case Race.CastanicFemale:
                    gender = "female";
                    race = "castanic";
                    break;
                case Race.Popori:
                    gender = "male";
                    race = "popori";
                    break;
                case Race.Elin:
                    gender = "female";
                    race = "popori";
                    break;
                case Race.Baraka:
                    return Path.Combine(App.ResourcesPath,"images/paperdoll/baraka.png");
            }

                    return Path.Combine(App.ResourcesPath, $"images/paperdoll/{race}_{gender}.png");
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

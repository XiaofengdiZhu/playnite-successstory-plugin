﻿using Playnite.SDK;
using SuccessStory.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace SuccessStory.Views.Interface
{
    /// <summary>
    /// Logique d'interaction pour SuccessStoryAchievementsList.xaml
    /// </summary>
    public partial class SuccessStoryAchievementsList : UserControl
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        public SuccessStoryAchievementsList(List<Achievements> ListAchievements)
        {
            InitializeComponent();

            List<listAchievements> ListBoxAchievements = new List<listAchievements>();

            for (int i = 0; i < ListAchievements.Count; i++)
            {
                DateTime? dateUnlock;
                BitmapImage iconImage = new BitmapImage();
                FormatConvertedBitmap ConvertBitmapSource = new FormatConvertedBitmap();

                bool isGray = false;

                iconImage.BeginInit();
                if (ListAchievements[i].DateUnlocked == default(DateTime) || ListAchievements[i].DateUnlocked == null)
                {
                    dateUnlock = null;
                    if (ListAchievements[i].UrlLocked == "" || ListAchievements[i].UrlLocked == ListAchievements[i].UrlUnlocked)
                    {
                        iconImage.UriSource = new Uri(ListAchievements[i].UrlUnlocked, UriKind.RelativeOrAbsolute);
                        isGray = true;
                    }
                    else
                    {
                        iconImage.UriSource = new Uri(ListAchievements[i].UrlLocked, UriKind.RelativeOrAbsolute);
                    }
                }
                else
                {
                    iconImage.UriSource = new Uri(ListAchievements[i].UrlUnlocked, UriKind.RelativeOrAbsolute);
                    dateUnlock = ListAchievements[i].DateUnlocked;
                }
                iconImage.EndInit();

                ConvertBitmapSource.BeginInit();
                ConvertBitmapSource.Source = iconImage;
                if (isGray)
                {
                    ConvertBitmapSource.DestinationFormat = PixelFormats.Gray32Float;
                }
                ConvertBitmapSource.EndInit();

                string NameAchievement = ListAchievements[i].Name;

                // Achievement without unlocktime but achieved = 1
                if (dateUnlock == new DateTime(1982, 12, 15, 0, 0, 0, 0))
                {
                    dateUnlock = null;
                }

                ListBoxAchievements.Add(new listAchievements()
                {
                    Name = NameAchievement,
                    DateUnlock = dateUnlock,
                    Icon = ConvertBitmapSource,
                    Description = ListAchievements[i].Description,
                    Percent = ListAchievements[i].Percent
                });

                iconImage = null;
            }



            // Sorting default.
            lbAchievements.ItemsSource = ListBoxAchievements;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lbAchievements.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("DateUnlock", ListSortDirection.Descending));
        }

        /// <summary>
        /// Show or not the ToolTip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string Text = ((TextBlock)sender).Text;
            TextBlock textBlock = (TextBlock)sender;

            Typeface typeface = new Typeface(
                textBlock.FontFamily,
                textBlock.FontStyle,
                textBlock.FontWeight,
                textBlock.FontStretch);

            FormattedText formattedText = new FormattedText(
                textBlock.Text,
                System.Threading.Thread.CurrentThread.CurrentCulture,
                textBlock.FlowDirection,
                typeface,
                textBlock.FontSize,
                textBlock.Foreground,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            if (formattedText.Width > textBlock.DesiredSize.Width)
            {
                ((ToolTip)((TextBlock)sender).ToolTip).Visibility = Visibility.Visible;
            }
            else
            {
                ((ToolTip)((TextBlock)sender).ToolTip).Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Resize ListBox on parent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbAchievements_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = ((FrameworkElement)((FrameworkElement)lbAchievements.Parent).Parent);
            lbAchievements.Height = parent.MaxHeight;
        }
    }

    public class SetColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Brushes.Transparent.Color;

            if ((float)value > 30)
            {
                return null;
            }

            if ((float)value <= 30)
            {
                color = Brushes.DarkGray.Color;
            }
            if ((float)value <= 10)
            {
                color = Brushes.Gold.Color;
            }

            Color newColor = new Color();
            newColor.ScR = (float)color.R / 255;
            newColor.ScG = (float)color.G / 255;
            newColor.ScB = (float)color.B / 255;

            return newColor;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

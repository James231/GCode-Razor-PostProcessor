﻿using GCodeRazor;
using GCodeRazor.Examples;
using GCodeRazor.UserControls;
using HL.Interfaces;
using HL.Manager;
using MaterialDesignThemes.Wpf;
using Microsoft.CodeAnalysis.CSharp;
using RazorLight;
using RazorLight.Compilation;
using System;
using System.ComponentModel;
using System.IO;
using System.Management.Instrumentation;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.XPath;

namespace CustomWindow
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settings;
        private bool hasChanged = true;
        private bool isLoading = false;
        private string oldText = null;
        private bool isShowingExamples;

        public MainWindow()
        {
            InitializeComponent();
            settings = Serializer.GetSettings();
            if (settings.FontSize < 1)
            {
                settings.FontSize = 20;
            }

            IThemedHighlightingManager hm = ThemedHighlightingManager.Instance;
            hm.SetCurrentTheme("VS2019_Dark");
            InputArea.SyntaxHighlighting = hm.GetDefinitionByExtension(".grazor");
            OutputArea.SyntaxHighlighting = hm.GetDefinitionByExtension(".grazor");

            InputArea.FontSize = settings.FontSize;
            OutputArea.FontSize = settings.FontSize;

            if (!settings.LineNumbers)
            {
                InputArea.ShowLineNumbers = false;
                OutputArea.ShowLineNumbers = false;
            }
            else
            {
                InputArea.Loaded += (object send, RoutedEventArgs e) => SetLineNumberMargin(InputArea);
                OutputArea.Loaded += (object send, RoutedEventArgs e) => SetLineNumberMargin(OutputArea);
            }

            InputArea.WordWrap = settings.WordWrap;
            OutputArea.WordWrap = settings.WordWrap;

            InputArea.Loaded += (object send, RoutedEventArgs e) => SetContextMenuIcons(InputArea);
            OutputArea.Loaded += (object send, RoutedEventArgs e) => SetContextMenuIcons(OutputArea);

            var heightDescriptor = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(ItemsControl));
            heightDescriptor.AddValueChanged(TopRow, SplitterHeightChanged);

            BuildExample(Serializer.GetExamples());
        }

        private void SeeExamplesButtonPressed(object sender, EventArgs e)
        {
            isShowingExamples = !isShowingExamples;

            if (isShowingExamples)
            {
                SeeExamplesLabel.Content = "Back to Code";
                CodeView.Visibility = Visibility.Collapsed;
                ExamplesView.Visibility = Visibility.Visible;
            }
            else
            {
                SeeExamplesLabel.Content = "See Examples";
                CodeView.Visibility = Visibility.Visible;
                ExamplesView.Visibility = Visibility.Collapsed;
            }
        }

        private void InputTextChanged(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }

            if (oldText == null)
            {
                if (!hasChanged)
                {
                    GenerateOverlay.Visibility = Visibility.Visible;
                    hasChanged = true;
                }
                return;
            }

            if (oldText == InputArea.Text)
            {
                GenerateOverlay.Visibility = Visibility.Collapsed;
                hasChanged = false;
                return;
            }

            if (hasChanged == false)
            {
                GenerateOverlay.Visibility = Visibility.Visible;
                hasChanged = true;
            }
        }

        private void GenerateButtonPressed(object sender, EventArgs e)
        {
            GenerateOverlay.Visibility = Visibility.Collapsed;
            LoadingOverlay.Visibility = Visibility.Visible;
            isLoading = true;

            DoStuff();
        }

        private void DoStuff()
        {
            string templateContent = "@{\r\n    DisableEncoding = true;\r\n}" + InputArea.Text;
            if (settings.FilesToConcat != null)
            {
                foreach (string addFilePath in settings.FilesToConcat)
                {
                    string fullFilePath = GetFullPath(addFilePath);
                    if (File.Exists(fullFilePath))
                    {
                        StreamReader reader = new StreamReader(fullFilePath);
                        string secondFileContent = reader.ReadToEnd();
                        reader.Close();
                        if (settings.TrimOutput)
                        {
                            templateContent += "\n" + secondFileContent.Trim('\r', '\n', ' ');
                        }
                        else
                        {
                            templateContent += "\n" + secondFileContent;
                        }
                    }
                }
            }

            Task.Run(async () => await Compile(templateContent));
        }

        private string GetFullPath(string path)
        {
            if (path.Length < 2)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            Regex rx = new Regex(@"[A-Za-z]:", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (rx.IsMatch(path.Substring(0, 2))) {
                return path;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        private async Task Compile(string templateContent)
        {
            var engine = GCodeRazor.EngineBuilder.Builder.GetBuilder();

            bool isError = false;
            string result = string.Empty;
            try
            {
                result = await engine.CompileRenderStringAsync("templateKey", templateContent, new EmptyModel());
            }
            catch (Exception ex)
            {
                isError = true;
                result = "Error: " + ex.Message;
            }

            if (settings.TrimOutput)
            {
                result = result.Trim('\r', '\n', ' ');
            }

            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateUiResult(result, isError);
            }));
        }

        private void UpdateUiResult(string result, bool isError)
        {
            OutputArea.Text = result;

            IThemedHighlightingManager hm = ThemedHighlightingManager.Instance;
            hm.SetCurrentTheme("VS2019_Dark");
            if (isError)
            {
                OutputArea.SyntaxHighlighting = hm.GetDefinitionByExtension(".txt");
            } else
            {
                OutputArea.SyntaxHighlighting = hm.GetDefinitionByExtension(".grazor");
            }


            if (!isError && settings.OpenFile)
            {
                string savedFilePath = Serializer.SaveTemporaryFile(result);
                System.Diagnostics.Process.Start(savedFilePath);
            }

            isLoading = false;
            hasChanged = false;
            LoadingOverlay.Visibility = Visibility.Collapsed;
            oldText = result;
        }

        public class Result
        {
            public string ResultString { get; set; }

            public bool IsError { get; set; }
        }

        private void SplitterUpButtonPressed(object sender, EventArgs e)
        {
            double totalHeight = TopRow.ActualHeight + BottomRow.ActualHeight;
            TopRow.Height = new GridLength(10, GridUnitType.Star);
            BottomRow.Height = new GridLength(totalHeight - 10, GridUnitType.Star);
            SplitterButtonUp.Visibility = Visibility.Collapsed;
            SplitterButtonDown.Visibility = Visibility.Visible;
        }

        private void SplitterDownButtonPressed(object sender, EventArgs e)
        {
            double totalHeight = TopRow.ActualHeight + BottomRow.ActualHeight;
            TopRow.Height = new GridLength(totalHeight - 10, GridUnitType.Star);
            BottomRow.Height = new GridLength(10, GridUnitType.Star);
            SplitterButtonUp.Visibility = Visibility.Visible;
            SplitterButtonDown.Visibility = Visibility.Collapsed;
        }

        private void SplitterHeightChanged(object sender, EventArgs e)
        {
            SplitterButtonUp.Visibility = TopRow.ActualHeight < 50 ? Visibility.Collapsed : Visibility.Visible;
            SplitterButtonDown.Visibility = BottomRow.ActualHeight < 50 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetLineNumberMargin(TextEditLib.TextEdit editor)
        {         
            DependencyObject border = GetChildOfType<Border>(editor);
            DependencyObject scrollViewer = GetChildOfType<ScrollViewer>(border);
            DependencyObject grid = GetChildOfType<Grid>(scrollViewer);
            DependencyObject scrollContentPresenter = GetChildOfType<ScrollContentPresenter>(grid);
            DependencyObject textarea = GetChildOfType<ICSharpCode.AvalonEdit.Editing.TextArea>(scrollContentPresenter);
            DependencyObject dockPanel = GetChildOfType<DockPanel>(textarea);
            DependencyObject itemsControl = GetChildOfType<ItemsControl>(dockPanel);  
            ItemsControl ic = (ItemsControl)itemsControl;
            ic.Padding = new Thickness(0, 0, 7, 0);
        }

        private void SetContextMenuIcons(TextEditLib.TextEdit editor)
        {
            Brush white = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            int numItems = editor.ContextMenu.Items.Count;
            for (int i = 0; i < numItems; i++)
            {
                var item = editor.ContextMenu.Items[i];
                if (item is MenuItem)
                {
                    MenuItem mi = (MenuItem)item;
                    mi.Icon = new PackIcon()
                    {
                        Kind = IconFromContextMenuHeader(mi.Header.ToString()),
                        Foreground = white,
                    };
                }
            }
        }

        private void BuildExample(IExampleControlModel[] children)
        {
            if (children.Length == 0)
            {
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                switch (children[i].Type)
                {
                    case "heading":
                        ExParent.Children.Add(new ExHeading((HeadingControlModel)children[i]));
                        break;
                    case "subheading":
                        ExParent.Children.Add(new ExSubheading((SubHeadingControlModel)children[i]));
                        break;
                    case "label":
                        ExParent.Children.Add(new ExLabel((LabelControlModel)children[i]));
                        break;
                    case "code":
                        ExParent.Children.Add(new ExCode((CodeControlModel)children[i], settings));
                        break;
                    case "card":
                        ExParent.Children.Add(new ExCard((CardControlModel)children[i], settings));
                        break;
                }
            }
        }

        private PackIconKind IconFromContextMenuHeader(string header)
        {
            switch(header)
            {
                case "Copy":
                    return PackIconKind.ContentCopy;
                case "Cut":
                    return PackIconKind.ContentCut;
                case "Paste":
                    return PackIconKind.ContentPaste;
                case "Undo":
                    return PackIconKind.Undo;
                case "Redo":
                    return PackIconKind.Redo;
                case "Delete":
                    return PackIconKind.Delete;
                default:
                    return PackIconKind.ContentCopy;
            }
        }

        public static T GetChildOfType<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}

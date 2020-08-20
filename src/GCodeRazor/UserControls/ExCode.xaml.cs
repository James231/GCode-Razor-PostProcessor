using GCodeRazor.Examples;
using HL.Interfaces;
using HL.Manager;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
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
    /// Interaction logic for ExCode.xaml
    /// </summary>
    public partial class ExCode : UserControl
    {
        private CodeControlModel model;
        private Settings settings;

        private SolidColorBrush whiteBrush;
        private SolidColorBrush greenBrush;

        public enum EditorTab
        {
            Razor,
            GCode
        }

        private EditorTab selectedTab;

        public ExCode(CodeControlModel codeModel, Settings appSettings)
        {
            InitializeComponent();
            model = codeModel;
            settings = appSettings;
            if (settings.FontSize < 1)
            {
                settings.FontSize = 20;
            }

            Parent.Margin = new Thickness(0, model.MarginAbove, 0, model.MarginBelow);

            whiteBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            greenBrush = new SolidColorBrush(Color.FromRgb(129, 237, 104));

            CodeEditor.Loaded += (object send, RoutedEventArgs e) => EditorLoadedEvent();
            CodeEditor.WordWrap = settings.WordWrap;

            // Set text area fields content type to use JSON (js) highlighting
            IThemedHighlightingManager hm = ThemedHighlightingManager.Instance;
            hm.SetCurrentTheme("VS2019_Dark");
            CodeEditor.SyntaxHighlighting = hm.GetDefinitionByExtension(".grazor");
        }

        private void EditorLoadedEvent()
        {
            if (!settings.LineNumbers)
            {
                CodeEditor.ShowLineNumbers = false;
            }
            else
            {
                SetLineNumberMargin(CodeEditor);
            }

            CodeEditor.FontSize = settings.FontSize;
            SetContextMenuIcons(CodeEditor);
            LoadTab(EditorTab.Razor);
        }

        private void LoadTab(EditorTab tab)
        {
            if (tab != selectedTab)
            {
                selectedTab = tab;
            }

            switch (tab)
            {
                case EditorTab.Razor:
                    CodeEditor.Text = model.RazorCode;
                    RazorButton.Foreground = greenBrush;
                    GcodeButton.Foreground = whiteBrush;
                    break;

                case EditorTab.GCode:
                    CodeEditor.Text = model.GCode;
                    RazorButton.Foreground = whiteBrush;
                    GcodeButton.Foreground = greenBrush;
                    break;
            }
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
            if (ic != null)
            {
                ic.Padding = new Thickness(0, 0, 7, 0);
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

        private PackIconKind IconFromContextMenuHeader(string header)
        {
            switch (header)
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

        private void RazorButtonPressed(object sender, RoutedEventArgs args)
        {
            LoadTab(EditorTab.Razor);
        }

        private void GcodeButtonPressed(object sender, RoutedEventArgs args)
        {
            LoadTab(EditorTab.GCode);
        }

        private void CopyButtonPressed(object sender, RoutedEventArgs args)
        {
            Clipboard.SetText(model.RazorCode);
        }

        private void CodeEditor_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }
    }
}

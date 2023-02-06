using Anyline.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Controls;

using TreeViewNode = Microsoft.UI.Xaml.Controls.TreeViewNode;
using TreeView = Microsoft.UI.Xaml.Controls.TreeView;
using Anyline.SDK.Models;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Anyline.SDK.PluginResults;

namespace AnylineExamples
{
    public class ValueTreeViewNode : TreeViewNode
    {
        public object Value { get; set; } = null;        
    }

    public sealed partial class ResultPage : Page
    {
        public ResultPage()
        {
            this.InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            var scanResults = args.Parameter as List<ScanResult>;

            ResultTreeView.RootNodes.Clear();
            foreach (var scanResult in scanResults)
            {
                ValueTreeViewNode node = new ValueTreeViewNode();
                node.Content = "ScanResult";
                node.Value = scanResult;

                PropertyInfo[] properties = scanResult.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    AddNodes(node, property.Name, property.GetValue(scanResult));
                }

                ResultTreeView.RootNodes.Add(node);
            }

            foreach(var node in ResultTreeView.RootNodes)
            {
                ExpandAll(ResultTreeView, node);
            }            

            ResultTreeView.ItemInvoked += ResultTreeView_ItemInvoked;
        }

        private async void ResultTreeView_ItemInvoked(TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
        {
            ValueImage.Source = null;
            ValueText.Text = "";
            ValueType.Text = "";
            
            var selectedNode = args.InvokedItem as ValueTreeViewNode;
            if (selectedNode?.Value != null)
            {
                if (selectedNode.Value is AnylineImage image)
                {
                    ValueImage.Source = await image.GetBitmapAsync();
                    ValueText.Text = $"{image.Width} x {image.Height} pixels";
                }
                else if (selectedNode.Value is JObject json)
                {
                    ValueText.Text = json.ToString();
                } else
                {
                    ValueText.Text = selectedNode.Value.ToString();
                }

                ValueType.Text = selectedNode.Value.GetType().Name;
            }
        }

        private void ExpandAll(TreeView treeview, TreeViewNode root)
        {
            root.IsExpanded = true;
            foreach(var node in root.Children)
            {
                ExpandAll(treeview, node);
            }
        }

        private void AddNodes(ValueTreeViewNode root, string name, object obj)
        {
            ValueTreeViewNode node = new ValueTreeViewNode();
            node.Content = name;

            if (obj is JToken jsonNode)
            {                
                if (jsonNode is JObject jsonObject)
                {
                    foreach(var item in jsonObject)
                    {
                        AddNodes(node, item.Key, item.Value);
                    }
                    node.Value = jsonObject;
                }
                else if (jsonNode is JArray jsonArray)
                {
                    int i = 0;
                    foreach(var item in jsonArray)
                    {
                        AddNodes(node, $"[{i++}]", item);
                    }
                    node.Value = jsonArray;
                }
                else
                {
                    node.Value = JsonToObjectValue(jsonNode);
                }
            }
            else if (obj is AnylineImage image)
            {
                node.Value = image;
            }
            else if (obj is PluginResult pluginResult)
            {                
                node.Value = pluginResult.ToString();
            }
            else
            {
                node.Value = obj.ToString();
            }

            root.Children.Add(node);
        }

        private object JsonToObjectValue(JToken json)
        {
            if (json.Type == JTokenType.Boolean)
                return (bool)json;
            if (json.Type == JTokenType.String)
                return (string)json;
            if (json.Type == JTokenType.Integer)
                return (int)json;
            if (json.Type == JTokenType.Float)
                return (float)json;
            
            return json;
        }
    }
}

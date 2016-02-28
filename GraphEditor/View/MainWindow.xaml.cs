﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (x, y) => Keyboard.Focus(graphArea);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var p1 = e.GetPosition(null);
            var p = e.GetPosition(graphArea);
            Coordinates.Text = $"X: {p.X}, Y: {p.Y}";
            Counter.Text = $"Elements: {graphArea.Children.Count}, Selected Elements: {graphArea.GetSelectedElements}";
            base.OnMouseMove(e);
        }
    }
}

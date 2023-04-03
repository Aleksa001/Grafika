using Projekat1.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Projekat1.Command
{
    public class AddCommand : ICommand
    {
        private readonly Canvas _canvas;
        private readonly UIElement _element;

        public AddCommand(Canvas canvas, UIElement element)
        {
            _canvas = canvas;
            _element = element;
        }

        public void Execute()
        {
            _canvas.Children.Add(_element);
        }

        public void Undo()
        {
            _canvas.Children.Remove(_element);
        }
    }
}

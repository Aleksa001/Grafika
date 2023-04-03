using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Projekat1.Interface;

namespace Projekat1.Command
{
    public  class RemoveCommand: ICommand
    {
        private readonly Canvas _canvas;
        private readonly UIElement _element;

        public RemoveCommand(Canvas canvas, UIElement element)
        {
            _canvas = canvas;
            _element = element;
        }

        public void Execute()
        {
            _canvas.Children.Remove(_element);
        }

        public void Undo()
        {
            _canvas.Children.Add(_element);
        }
    }
}

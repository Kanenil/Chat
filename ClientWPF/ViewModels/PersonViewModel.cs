using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWPF.ViewModels
{
    public class PersonViewModel : ViewModelBase
    {
        public string Name { get; }

        public PersonViewModel(string name)
        {
            Name = name;
        }
    }
}
